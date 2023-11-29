// -----------------------------------------------------------------------
// <copyright file="Events.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments, and potential removed properties.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events;

using System;
using System.Diagnostics;

using Features;
using UnityEngine.SceneManagement;

#pragma warning disable SA1401 // field should be private. (it shouldn't)

/// <summary>
/// The main events api for patching and un-patching methods.
/// </summary>
public sealed class Events
{
    /// <summary>
    /// Indicates whether debug patches and debug patch logs should be enabled.
    /// </summary>
    internal const bool DebugPatches = false;

    /// <summary>
    /// Indicates whether or not events should be logged on execution.
    /// </summary>
    internal const bool LogEvent = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Events"/> class.
    /// </summary>
    internal Events()
    {
        Instance = this;
        OnEnabled();
    }

    /// <summary>
    /// Gets the plugin instance.
    /// </summary>
    public static Events Instance { get; private set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether or not dynamic patching will be used.
    /// </summary>
    public static bool UseDynamicPatching { get; set; } = true;

    /// <summary>
    /// Gets the <see cref="Features.Patcher"/> used to employ all patches.
    /// </summary>
    public Patcher Patcher { get; private set; } = null!;

    /// <summary>
    /// Patches all methods.
    /// </summary>
    public void OnEnabled()
    {
        Stopwatch watch = Stopwatch.StartNew();
        Patch();
        watch.Stop();
        Log.Info($"All patches completed in {watch.Elapsed}");

        SceneManager.sceneUnloaded += Handlers.Internal.SceneUnloaded.OnSceneUnloaded;
    }

    /// <summary>
    /// Unpatches all methods.
    /// </summary>
    public void OnDisabled()
    {
        Unpatch();
        SceneManager.sceneUnloaded -= Handlers.Internal.SceneUnloaded.OnSceneUnloaded;
    }

    /// <summary>
    /// Patches all events.
    /// </summary>
    public void Patch()
    {
        try
        {
            Patcher = new Patcher();
            Patcher.PatchAll(out int failedPatch, out int totalPatches);

            if (failedPatch == 0)
                Log.Debug($"Events patched successfully! [{totalPatches} total patches]");
            else
                Log.Error($"Patching failed! There are {failedPatch} broken patches [{totalPatches} total patches].");
        }
        catch (Exception exception)
        {
            Log.Error($"Patching failed!\n{exception}");
        }
    }

    /// <summary>
    /// Unpatches all events.
    /// </summary>
    public void Unpatch()
    {
        Log.Debug("Unpatching events...");
        Patcher.UnpatchAll();
        Log.Debug("All events have been unpatched complete. Goodbye!");
    }
}