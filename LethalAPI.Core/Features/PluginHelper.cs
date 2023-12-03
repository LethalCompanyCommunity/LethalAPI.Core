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
using BepInEx;
using BepInEx.Bootstrap;
using Interfaces;
using Loader;
using MelonLoader;
using Models;
using Newtonsoft.Json;

/// <summary>
///     Helper class for plugin related functions.
/// </summary>
internal static class PluginHelper
{
    /// <summary>
    ///     Gets all MelonLoader plugins (if any exist / if MelonLoader is present).
    /// </summary>
    /// <returns> IEnumerable of all MelonLoader plugins. </returns>
    private static IEnumerable<MelonMod> GetMelonLoaderPlugins()
    {
        return PluginLoader.MelonLoaderFound
            ? GetMelonLoaderPluginsTypeLoadSafe()
            : new List<MelonMod>();
    }

    /// <summary>
    ///     Gets all BepInEx plugins (if any exist / if BepInEx is present).
    /// </summary>
    /// <returns>IEnumerable of all BepInEx plugins.</returns>
    private static IEnumerable<PluginInfo> GetBepInExPlugins()
    {
        return PluginLoader.BepInExFound
            ? GetBepInExPluginsTypeLoadSafe()
            : new List<PluginInfo>();
    }

    /// <summary>
    ///     Getter for MelonLoader plugins. Prevents TypeLoad exceptions due to being called a level deeper.
    /// </summary>
    /// <returns>IEnumerable of all MelonLoader plugins.</returns>
    private static IEnumerable<MelonMod> GetMelonLoaderPluginsTypeLoadSafe()
    {
        return MelonMod.RegisteredMelons;
    }

    /// <summary>
    ///     Getter for BepInEx plugins. Prevents TypeLoad exceptions due to being called a level deeper.
    /// </summary>
    /// <returns>IEnumerable of all BepInEx plugins.</returns>
    private static IEnumerable<PluginInfo> GetBepInExPluginsTypeLoadSafe()
    {
        return Chainloader.PluginInfos.Values;
    }

    /// <summary>
    ///     Get all plugins in the <see cref="IPlugin{TConfig}" /> format.
    /// </summary>
    /// <returns> An IEnumerable of plugins in the <see cref="IPlugin{TConfig}" /> format. </returns>
    internal static IEnumerable<IPlugin<IConfig>> GetLethalPlugins()
    {
        return PluginLoader.Plugins.Values;
    }

    /// <summary>
    ///     Check if a plugin is required.
    /// </summary>
    /// <param name="plugin"> The plugin to check. </param>
    /// <returns> True if the plugin is required, false otherwise. </returns>
    internal static bool IsPluginRequired(object plugin)
    {
        return plugin.GetType().GetCustomAttributes(typeof(LethalRequiredPluginAttribute), false).Any();
    }

    private static void AddMelonLoaderPlugins(ref List<PluginInfoRecord> plugins)
    {
        plugins.AddRange(GetMelonLoaderPlugins().Select(plugin =>
            new PluginInfoRecord(plugin.Info.Name, new Version(plugin.Info.Version), IsPluginRequired(plugin))));
    }

    private static void AddBepInExPlugins(ref List<PluginInfoRecord> plugins)
    {
        plugins.AddRange(GetBepInExPlugins().Select(plugin =>
            new PluginInfoRecord(plugin.Metadata.Name, plugin.Metadata.Version, IsPluginRequired(plugin))));
    }

    /// <summary>
    ///     Get all plugins in the <see cref="PluginInfoRecord" /> format.
    /// </summary>
    /// <returns> An IEnumerable of plugins in the <see cref="PluginInfoRecord" /> format. </returns>
    internal static IEnumerable<PluginInfoRecord> GetAllPluginInfo()
    {
        List<PluginInfoRecord> plugins = new();

        if (PluginLoader.MelonLoaderFound)
            AddMelonLoaderPlugins(ref plugins);

        if (PluginLoader.BepInExFound)
            AddBepInExPlugins(ref plugins);

        plugins.AddRange(GetLethalPlugins().Select(plugin =>
            new PluginInfoRecord(plugin.Name, plugin.Version, IsPluginRequired(plugin))));

        return plugins;
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
        return JsonConvert.SerializeObject(GetAllRequiredPluginInfo().ToList());
    }

    /// <summary>
    ///     Parses a json string containing the metadata of all plugins.
    /// </summary>
    /// <param name="json"> The json string to parse. </param>
    /// <returns> A list of plugins in the APIPluginInfo format. </returns>
    internal static IEnumerable<PluginInfoRecord> ParseLobbyPluginsMetadata(string json)
    {
        return JsonConvert.DeserializeObject<List<PluginInfoRecord>>(json) ?? new List<PluginInfoRecord>();
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
}