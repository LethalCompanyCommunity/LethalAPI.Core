// -----------------------------------------------------------------------
// <copyright file="InheritedSaveHandler.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using System;
using System.Reflection;

using Interfaces;

/// <inheritdoc />
internal sealed class InheritedSaveHandler : PropertySaveHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InheritedSaveHandler"/> class.
    /// </summary>
    /// <param name="plugin">The plugin to get the save instance from.</param>
    /// <param name="searchGlobal">Indicates whether or not to search globally.</param>
    internal InheritedSaveHandler(IPlugin<IConfig> plugin, bool searchGlobal = false)
        : base(searchGlobal)
    {
        this.IsGlobalSave = searchGlobal;
        this.Plugin = plugin;
        this.PluginInstance = plugin.RootInstance;
        Type? saveInterface = null;
        foreach (Type pluginInterface in this.PluginInstance.GetType().GetInterfaces())
        {
            if (!pluginInterface.IsGenericType)
                continue;

            Type type = pluginInterface.GetGenericTypeDefinition();
            if(searchGlobal)
            {
                if (type != typeof(IGlobalDataSave<>))
                    continue;

                saveInterface = pluginInterface;
                break;
            }

            if (type != typeof(ILocalDataSave<>))
                continue;

            saveInterface = pluginInterface;
            break;
        }

        if (saveInterface is null)
        {
            throw new ArgumentNullException(nameof(plugin), $"Plugin does not inherit I{(searchGlobal ? "Global" : "Local")}SaveData.");
        }

        // nameof(ILocalDataSave<>.LocalSaveSettings)
        // nameof(IGlobalDataSave<>.GlobalSaveSettings)
        this.Plugin = plugin;
        this.Settings = (SaveDataSettings)this.PluginInstance.GetType().GetProperty((searchGlobal ? "Global" : "Local") + "SaveSettings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(plugin);
        this.Property = this.PluginInstance.GetType().GetProperty((searchGlobal ? "Global" : "Local") + "SaveData")!;

        if(Settings.AutoSave)
            HookAutoSave();
        Log.Debug($"Successfully created inherited save handler for plugin {plugin.Name} [{Settings.AutoSave}, {Settings.AutoLoad}].");
    }
}