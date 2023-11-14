// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

/// <inheritdoc />
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// The base logger.
    /// </summary>
    internal static ManualLogSource Log;
    
    /// <summary>
    /// The harmony instance.
    /// </summary>
    internal static Harmony _harmony;

    private void Awake()
    {
        Log = Logger;

        Log.LogInfo($"{PluginInfo.PLUGIN_GUID} is being loaded...");
        _harmony = new(PluginInfo.PLUGIN_GUID);

        _harmony.PatchAll(typeof(Plugin).Assembly);
    }
}