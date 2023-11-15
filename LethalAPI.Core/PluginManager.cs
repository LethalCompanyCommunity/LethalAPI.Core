// -----------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core;

using System.Collections.Generic;
using System.Linq;

using BepInEx.Bootstrap;
using Models;
using UnityEngine;

/// <summary>
///     General manager for plugins.
/// </summary>
internal static class PluginManager
{
    /// <summary>
    ///     Gets a list of plugin guids that are required to be loaded for all players.
    /// </summary>
    private static List<string> RequiredPluginGuids { get; } = new();

    /// <summary>
    ///     Adds a plugin guid to the required plugin guid list.
    /// </summary>
    /// <param name="pluginGuid"> The plugin guid to add. </param>
    public static void AddRequiredPluginGuid(string pluginGuid)
    {
        RequiredPluginGuids.Add(pluginGuid);
    }

    /// <summary>
    ///     Gets a list of plugins in the APIPluginInfo format.
    /// </summary>
    /// <returns> A list of plugins in the APIPluginInfo format. </returns>
    public static List<APIPluginInfo> GetAPIPluginInfoList()
    {
        return (from plugin in Chainloader.PluginInfos
            let required = RequiredPluginGuids.Contains(plugin.Value.Metadata.GUID)
            select new APIPluginInfo(plugin.Value.Metadata.GUID, plugin.Value.Metadata.Version, required)).ToList();
    }

    /// <summary>
    ///     Creates a json string containing the metadata of all plugins, to add to the lobby.
    /// </summary>
    /// <returns> A json string containing the metadata of all plugins. </returns>
    internal static string GetLobbyPluginsMetadata()
    {
        List<APIPluginInfo> plugins = GetAPIPluginInfoList();
        return JsonUtility.ToJson(plugins);
    }

    /// <summary>
    ///     Parses a json string containing the metadata of all plugins.
    /// </summary>
    /// <param name="json"> The json string to parse. </param>
    /// <returns> A list of plugins in the APIPluginInfo format. </returns>
    internal static List<APIPluginInfo> ParseLobbyPluginsMetadata(string json)
    {
        return JsonUtility.FromJson<List<APIPluginInfo>>(json);
    }

    /// <summary>
    ///     Checks if the client has all plugins labeled as required.
    /// </summary>
    /// <param name="targetPluginInfo"> The plugin info of the target. </param>
    /// <returns> True if the client has all plugins labeled as required, false otherwise. </returns>
    internal static bool MatchesTargetRequirements(List<APIPluginInfo> targetPluginInfo)
    {
        List<APIPluginInfo> clientPluginInfo = GetAPIPluginInfoList();

        return targetPluginInfo.Where(pluginInfo => pluginInfo.Required).All(pluginInfo =>
            clientPluginInfo.Exists(plugin =>
                plugin.Guid == pluginInfo.Guid && plugin.PluginVersion.Major == pluginInfo.PluginVersion.Major &&
                plugin.PluginVersion.Minor >= pluginInfo.PluginVersion.Minor));
    }
}