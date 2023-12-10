// -----------------------------------------------------------------------
// <copyright file="PluginHelper.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using Attributes;
using Interfaces;
using Loader;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
///     Helper class for plugin related functions.
/// </summary>
internal static class PluginHelper
{
    /// <summary>
    ///     Check if a plugin is required.
    /// </summary>
    /// <param name="plugin"> The plugin to check. </param>
    /// <returns> True if the plugin is required, false otherwise. </returns>
    internal static bool IsPluginRequired(IPlugin<IConfig> plugin)
    {
        return plugin.RootInstance.GetType().GetCustomAttributes(typeof(LethalRequiredPluginAttribute), false).Any();
    }

    /// <summary>
    ///     Get all required plugins in the <see cref="PluginInfoRecord" /> format.
    /// </summary>
    /// <returns> An IEnumerable of required plugins in the <see cref="PluginInfoRecord" /> format. </returns>
    internal static IEnumerable<PluginInfoRecord> GetAllRequiredPluginInfo()
    {
        return GetAllPluginInfo().Where(plugin => plugin.IsRequired);
    }

    /// <summary>
    ///     Creates a json string containing the metadata of all plugins, to add to the lobby.
    /// </summary>
    /// <returns> A json string containing the metadata of all plugins. </returns>
    internal static string GetLobbyPluginsMetadata()
    {
        return JsonConvert.SerializeObject(GetAllRequiredPluginInfo().ToList(), new VersionConverter());
    }

    /// <summary>
    ///     Parses a json string containing the metadata of all plugins.
    /// </summary>
    /// <param name="json"> The json string to parse. </param>
    /// <returns> A list of plugins in the APIPluginInfo format. </returns>
    internal static IEnumerable<PluginInfoRecord> ParseLobbyPluginsMetadata(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<PluginInfoRecord>>(json, new VersionConverter()) ??
                   new List<PluginInfoRecord>();
        }
        catch (Exception e)
        {
            Log.Warn("Failed to parse lobby plugins metadata.");
            Log.Exception(e);
            throw;
        }
    }

    /// <summary>
    ///     Checks if the client has all plugins labeled as required.
    /// </summary>
    /// <param name="targetPluginInfo"> The plugin info of the target. </param>
    /// <returns> True if the client has all plugins labeled as required, false otherwise. </returns>
    internal static bool MatchesTargetRequirements(IEnumerable<PluginInfoRecord> targetPluginInfo)
    {
        List<PluginInfoRecord> clientPluginInfo = GetAllPluginInfo().ToList();

        return targetPluginInfo.Where(pluginInfo => pluginInfo.IsRequired).All(pluginInfo =>
            clientPluginInfo.Exists(plugin => plugin.GUID == pluginInfo.GUID && plugin.Version >= pluginInfo.Version));
    }

    /// <summary>
    ///     Get all plugins in the <see cref="PluginInfoRecord" /> format.
    /// </summary>
    /// <returns> An IEnumerable of plugins in the <see cref="PluginInfoRecord" /> format. </returns>
    private static IEnumerable<PluginInfoRecord> GetAllPluginInfo()
    {
        return PluginLoader.Plugins.Values.Select(plugin => plugin.Info);
    }
}