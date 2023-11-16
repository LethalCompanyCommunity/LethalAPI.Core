// -----------------------------------------------------------------------
// <copyright file="InitializeGamePatch.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches;

using HarmonyLib;

/// <summary>
///     Patches <see cref="InitializeGame" />.
/// </summary>
[HarmonyPatch(typeof(InitializeGame))]
internal static class InitializeGamePatch
{
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    private static void AwakePostfix(InitializeGame __instance)
    {
        PluginManager.PopulateRequiredPlugins();
    }
}