// -----------------------------------------------------------------------
// <copyright file="Events.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
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

/// <summary>
/// The main events api for patching and un-patching methods.
/// </summary>
public sealed class Events
{
    /// <summary>
    /// Gets the plugin instance.
    /// </summary>
    public static Events Instance { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not dynamic patching will be used.
    /// </summary>
    public static bool UseDynamicPatching { get; set; } = true;

    /// <summary>
    /// Gets the <see cref="Features.Patcher"/> used to employ all patches.
    /// </summary>
    public Patcher Patcher { get; private set; }

    /// <summary>
    /// Patches all methods.
    /// </summary>
    public void OnEnabled()
    {
        Instance = this;

        Stopwatch watch = Stopwatch.StartNew();
        Patch();
        watch.Stop();
        Plugin.Singleton.Log.LogInfo($"All patches completed in {watch.Elapsed}");

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
            Patcher.PatchAll(out int failedPatch);

            if (failedPatch == 0)
                Plugin.Singleton.Log.LogDebug("Events patched successfully!");
            else
                Plugin.Singleton.Log.LogError($"Patching failed! There are {failedPatch} broken patches.");
        }
        catch (Exception exception)
        {
            Plugin.Singleton.Log.LogError($"Patching failed!\n{exception}");
        }
    }

    /// <summary>
    /// Unpatches all events.
    /// </summary>
    public void Unpatch()
    {
        Plugin.Singleton.Log.LogDebug("Unpatching events...");
        Patcher.UnpatchAll();
        Patcher = null;
        Plugin.Singleton.Log.LogDebug("All events have been unpatched complete. Goodbye!");
    }
}