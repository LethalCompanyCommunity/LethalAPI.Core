// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable SA1401 // field should be made private

using BepInEx;
using BepInEx.Logging;
using BepInEx.Logging;
using HarmonyLib;

/// <inheritdoc />
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// Gets the singleton for a plugin.
    /// </summary>
    public static Plugin Singleton;

    /// <summary>
    /// Gets the <see cref="Logger"/>.
    /// </summary>
    /// <summary>
    /// The base logger.
    /// </summary>
    internal static ManualLogSource Log;

    /// <summary>
    /// The harmony instance.
    /// </summary>
    internal static Harmony Harmony;

    private void Awake()
    {
        Singleton = this;
        Log = this.Logger;

        // Plugin startup logic
        this.Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        Log.LogInfo($"{PluginInfo.PLUGIN_GUID} is being loaded...");
        Harmony = new(PluginInfo.PLUGIN_GUID);

        Harmony.PatchAll(typeof(Plugin).Assembly);
    }
}