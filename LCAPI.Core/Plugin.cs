// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core;

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
#pragma warning disable SA1401
    public static Plugin Singleton;
#pragma warning restore SA1401

    /// <summary>
    /// Gets the <see cref="Logger"/>.
    /// </summary>
    internal static ManualLogSource Log;

    /// <summary>
    /// The internal harmony instance.
    /// </summary>
    internal static Harmony _harmony;

    private void Awake()
    {
        Singleton = this;

        // Plugin startup logic
        this.Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        Log = Logger;

        Log.LogInfo($"{PluginInfo.PLUGIN_GUID} is being loaded...");
        _harmony = new(PluginInfo.PLUGIN_GUID);

        _harmony.PatchAll(typeof(Plugin).Assembly);
    }
}