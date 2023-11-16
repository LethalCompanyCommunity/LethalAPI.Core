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
using System;

using BepInEx;
using BepInEx.Logging;
using global::MEC;
using HarmonyLib;

/// <inheritdoc />
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// Gets the singleton for a plugin.
    /// </summary>
    public static Plugin Singleton = null!;

    /// <summary>
    /// Gets the <see cref="BepInEx.Logging.Logger"/>.
    /// </summary>
    /// <summary>
    /// The base logger.
    /// </summary>
#pragma warning disable SA1309
    internal static ManualLogSource _Logger = null!;
#pragma warning restore SA1309

    /// <summary>
    /// The harmony instance.
    /// </summary>
    internal static Harmony Harmony = null!;

    private void Awake()
    {
        _Logger = this.Logger;
        Harmony = new(PluginInfo.PLUGIN_GUID);
        Harmony.PatchAll(typeof(Plugin).Assembly);
        Singleton = this;

        Log.Info($"{PluginInfo.PLUGIN_GUID} is being loaded...");

        Timing.Instance = this.gameObject.AddComponent<Timing>();
        Timing.Instance.name = "Timing Controller";
        Timing.Instance.OnException += OnError;
    }

    // ReSharper disable once ParameterHidesMember
    private void OnError(Exception exception, string tag)
    {
        Log.Error($"Timings has caught an error during the execution of a coroutine{(tag == "Unknown" ? string.Empty : $" [{tag}]")}. Exception: \n" + exception.Message, "MEC Timings");
    }
}