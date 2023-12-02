// -----------------------------------------------------------------------
// <copyright file="InheritedSaveHandler.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using System;

using Attributes;
using Interfaces;

/// <inheritdoc />
internal sealed class InheritedSaveHandler : PropertySaveHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InheritedSaveHandler"/> class.
    /// </summary>
    /// <param name="plugin">The plugin to get the save instance from.</param>
    internal InheritedSaveHandler(IPlugin<IConfig> plugin)
    {
        this.Plugin = plugin;
        this.PluginInstance = plugin.RootInstance;
        Type? saveInterface = null;
        foreach (Type pluginInterface in plugin.GetType().GetInterfaces())
        {
            Log.Debug($"{pluginInterface.Name} {(pluginInterface.GenericTypeArguments.Length > 0 ? pluginInterface.GenericTypeArguments[0] : string.Empty)}");
            Log.Debug($"{pluginInterface.IsGenericType} {pluginInterface.GetGenericTypeDefinition()?.Name} {(pluginInterface.GetGenericTypeDefinition()?.GenericTypeArguments.Length > 0 ? pluginInterface.GetGenericTypeDefinition().GenericTypeArguments[0] : string.Empty)}");
            if (pluginInterface.IsGenericType && pluginInterface.GetGenericTypeDefinition() == typeof(ISavePlugin<>))
            {
                saveInterface = pluginInterface;
            }
        }

        if (saveInterface is null)
        {
            throw new ArgumentNullException(nameof(plugin), "Plugin does not inherit ISavePlugin.");
        }

        this.Plugin = plugin;
        this.Settings = (SaveDataAttribute)saveInterface.GetProperty("SaveSettings")!.GetValue(plugin);
        this.Property = saveInterface.GetProperty("SaveData")!;
    }
}