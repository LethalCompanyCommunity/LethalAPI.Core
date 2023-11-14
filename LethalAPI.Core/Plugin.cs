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
    internal static Harmony Harmony;

    private void Awake()
    {
        Log = Logger;

        Log.LogInfo($"{PluginInfo.PLUGIN_GUID} is being loaded...");
        Harmony = new(PluginInfo.PLUGIN_GUID);

        Harmony.PatchAll(typeof(Plugin).Assembly);
    }
}