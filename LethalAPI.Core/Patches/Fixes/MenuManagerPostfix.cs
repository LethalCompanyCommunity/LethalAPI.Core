// -----------------------------------------------------------------------
// <copyright file="MenuManagerPostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Fixes;

using HarmonyLib;

/// <summary>
/// Patches the menu manager to show a 'MOD' mark next to the main menu version display, informing the user they're running mods.
/// </summary>
// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(MenuManager), "Awake")]
[HarmonyWrapSafe]
internal static class MenuManagerPostfix
{
    [HarmonyPostfix]
    private static void Postfix(MenuManager __instance)
    {
        if (__instance != null && __instance.versionNumberText != null)
        {
            __instance.versionNumberText.text = $"{GameNetworkManager.Instance.gameVersionNum}\nMOD";
        }
    }
}