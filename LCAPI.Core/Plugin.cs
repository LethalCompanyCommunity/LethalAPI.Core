// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

/// <inheritdoc />
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    internal static Harmony _harmony;

    private void Awake()
    {
        Log = Logger;

        Log.LogInfo("LCAPI.Core is being loaded");
        _harmony = new(PluginInfo.PLUGIN_GUID);

        _harmony.PatchAll(typeof(Plugin).Assembly);
    }
}