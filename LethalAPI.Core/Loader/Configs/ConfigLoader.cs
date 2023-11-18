// -----------------------------------------------------------------------
// <copyright file="ConfigLoader.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs;

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

using Interfaces;
using UnityEngine.UIElements;

/// <summary>
/// Provides the implementation for loading configs.
/// </summary>
public sealed class ConfigLoader
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
    /// Loads the config for all plugin instances.
    /// </summary>
    public static void LoadAllConfigs()
    {
        if (LoadSeperatedConfigFiles)
        {
            LoadAllConfigsCombined();
            return;
        }

        foreach (IPlugin<IConfig> plugin in PluginLoader.Plugins.Values)
        {
            LoadConfig(plugin);
        }
    }

    /// <summary>
    /// Loads the config for a specific plugin instance.
    /// </summary>
    /// <param name="plugin">The plugin instance to load the config for.</param>
    public static void LoadConfig(IPlugin<IConfig> plugin)
    {
        if (LoadSeperatedConfigFiles)
            plugin.Config = GetSeparatedConfigValue(plugin);
        else
            plugin.Config = GetConfigValueForPlugin(plugin);
    }

    [Pure]
    private static Dictionary<string, IConfig> GetLatestCombinedConfig()
    {
        string path = Path.Combine(ConfigDirectory, "Config.yml");
        using StreamReader reader = new(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
        Dictionary<string, IConfig> pluginConfigs =
            Serialization.Deserializer.Deserialize<Dictionary<string, IConfig>>(reader.ReadToEnd());
        reader.Close();

        bool changed = false;
        foreach (IPlugin<IConfig> plugin in PluginLoader.Plugins.Values)
        {
            if (!pluginConfigs.ContainsKey(plugin.Name.ToSnakeCase()))
            {
                Log.Info($"Plugin config for plugin '{plugin.Name}' is missing! Generating new config.");
                pluginConfigs.Add(plugin.Name.ToSnakeCase(), plugin.Config);
                changed = true;
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
            using StreamWriter writer = new (File.Open(path, FileMode.Truncate, FileAccess.Write, FileShare.Write));
            writer.Write(Serialization.Serializer.Serialize(pluginConfigs));
            writer.Close();
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
        using StreamReader reader = new (File.Open(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read));
        IConfig conf = Serialization.Deserializer.Deserialize<IConfig>(reader.ReadToEnd());

        if (!conf.GetType().IsSubclassOf(plugin.Config.GetType()))
        {
            StreamWriter writer = new(File.Open(path, FileMode.Truncate, FileAccess.Write, FileShare.Write));
            writer.Write(Serialization.Serializer.Serialize(conf));
            writer.Close();
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

            plugin.Config = latestConf[plugin.Name.ToSnakeCase()];
        }
    }
}