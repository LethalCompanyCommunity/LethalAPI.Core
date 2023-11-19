// -----------------------------------------------------------------------
// <copyright file="ConfigLoader.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs;

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;

using Interfaces;
using MonoMod.Utils;
using Resources;
using UnityEngine.UIElements;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

/// <summary>
/// Provides the implementation for loading configs.
/// </summary>
public static class ConfigLoader
{
    /// <summary>
    /// Gets the directory where the configs live.
    /// </summary>
    public static string ConfigDirectory { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether a single combined config file should be used, or separate smaller config files should be used.
    /// </summary>
    public static bool LoadSeperatedConfigFiles { get; private set; } = true;

    /// <summary>
    /// Copy all properties from the source class to the target one.
    /// </summary>
    /// <param name="target">The target object.</param>
    /// <param name="source">The source object to copy properties from.</param>
    public static void CopyProperties(this object target, object source)
    {
        Type type = target.GetType();

        if (type != source.GetType())
            throw new Exception("Target and source type mismatch!");

        foreach (PropertyInfo sourceProperty in type.GetProperties())
        {
            if (type.GetProperty(sourceProperty.Name) is { } property && property.GetCustomAttribute<YamlIgnoreAttribute>() is null && property.SetMethod is not null)
                property.SetValue(target, sourceProperty.GetValue(source, null), null);
        }
    }

    /// <summary>
    /// Loads the config for all plugin instances.
    /// </summary>
    public static void LoadAllConfigs()
    {
        Log.Debug($"deps loaded: {PluginLoader.Dependencies.Count}");

        try
        {
            if (!LoadSeperatedConfigFiles)
            {
                LoadAllConfigsCombined();
                return;
            }

            foreach (IPlugin<IConfig> plugin in PluginLoader.Plugins.Values)
            {
                LoadConfig(plugin);
            }
        }
        catch (TypeLoadException e)
        {
            if (EmbeddedResourceLoader.Debug)
            {
                e.LogDetailed();
            }
        }
        catch (Exception e)
        {
            Log.Error($"Could not load configs due to an error!", "LethalAPI-Loader");
            Log.Debug($"Exception: \n{e}", EmbeddedResourceLoader.Debug, "LethalAPI-Loader");
        }
    }

    /// <summary>
    /// Loads the config for a specific plugin instance.
    /// </summary>
    /// <param name="plugin">The plugin instance to load the config for.</param>
    public static void LoadConfig(IPlugin<IConfig> plugin)
    {
        if (LoadSeperatedConfigFiles)
            plugin.UpdateConfig(GetSeparatedConfigValue(plugin));
        else
            plugin.UpdateConfig(GetConfigValueForPlugin(plugin));
    }

    [Pure]
    private static Dictionary<string, IConfig> GetLatestCombinedConfig()
    {
        string path = Path.Combine(ConfigDirectory, "lethal-api-config.yml");
        Dictionary<string, IConfig>? pluginConfigs = null;
        try
        {
            if(File.Exists(path))
                pluginConfigs = Serialization.Deserializer.Deserialize<Dictionary<string, IConfig>>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            Log.Warn("Old config was invalid. A backup of the old config will be made.");
            Log.Debug($"{e}");
            MakeBackupOfConfig(path);
        }

        pluginConfigs ??= new();

        bool changed = false;
        foreach (IPlugin<IConfig> plugin in PluginLoader.Plugins.Values)
        {
            if (!pluginConfigs.ContainsKey(plugin.Name.ToSnakeCase()))
            {
                Log.Info($"Plugin config for plugin '{plugin.Name}' is missing! Generating new config.");
                pluginConfigs.Add(plugin.Name.ToSnakeCase(), plugin.Config);
                changed = true;
                continue;
            }

            IConfig conf = pluginConfigs[plugin.Name.ToSnakeCase()];
            if (!conf.GetType().IsSubclassOf(plugin.Config.GetType()))
            {
                pluginConfigs[plugin.Name.ToSnakeCase()] = plugin.Config;
                Log.Warn($"Plugin config for plugin '{plugin.Name}' is invalid or missing. Generating default values.");
                changed = true;
            }
        }

        if (changed)
        {
            MakeBackupOfConfig(path);
            File.WriteAllText(path, Serialization.Serializer.Serialize(pluginConfigs));
        }

        return pluginConfigs;
    }

    [Pure]
    private static IConfig GetConfigValueForPlugin(IPlugin<IConfig> plugin)
        => GetLatestCombinedConfig()[plugin.Name.ToSnakeCase()];

    [Pure]
    private static IConfig GetSeparatedConfigValue(IPlugin<IConfig> plugin)
    {
        string path = Path.Combine(ConfigDirectory, plugin.Name + ".yml");
        IConfig? conf = null;
        try
        {
            if(File.Exists(path))
                conf = Serialization.Deserializer.Deserialize<IConfig>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            if (e is YamlException yE)
                yE.LogDetailed();
            Log.Warn($"Could not load the config for plugin '{plugin.Name}'. Exception: \n{e}");
        }

        if (conf is null || !conf.GetType().IsSubclassOf(plugin.Config.GetType()))
        {
            conf ??= plugin.Config;
            MakeBackupOfConfig(path);
            File.WriteAllText(path, Serialization.Serializer.Serialize(conf));
            Log.Warn($"Plugin config for plugin '{plugin.Name}' is invalid or missing. Generating default values.");
        }

        return conf;
    }

    private static void LoadAllConfigsCombined()
    {
        Dictionary<string, IConfig> latestConf = GetLatestCombinedConfig();
        foreach (IPlugin<IConfig> plugin in PluginLoader.Plugins.Values)
        {
            if (!latestConf.ContainsKey(plugin.Name.ToSnakeCase()))
            {
                Log.Warn($"Missing config for '{plugin.Name}'.");
                continue;
            }

            plugin.UpdateConfig(latestConf[plugin.Name.ToSnakeCase()]);
        }
    }

    private static void MakeBackupOfConfig(string path)
    {
        if (!File.Exists(path))
            return;
        try
        {
            List<string> files = new()
            {
                "-old.yml",
                "-old-1.yml",
                "-old-2.yml",
            };

            string curFile = string.Empty;
            for (int i = files.Count; i > 0; i--)
            {
                curFile = path.Replace(".yml", files[i - 1]);
                string nextFile = i == 1 ? string.Empty : path.Replace(".yml", files[i - 2]);

                // copied by the prev iteration
                if (File.Exists(curFile))
                    File.Delete(curFile);

                if (nextFile == string.Empty)
                {
                    continue;
                }

                if (!File.Exists(nextFile))
                    continue;

                File.Copy(nextFile, curFile);
            }

            File.Copy(path, curFile);
        }
        catch (Exception e)
        {
            Log.Warn($"A backup file could not be made of the invalid config due to an error. Error: \n{e}");
        }
    }
}