// -----------------------------------------------------------------------
// <copyright file="MenuManagerPostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches;

using HarmonyLib;

/// <summary>
/// Patches the menu manager to prevent non-modded players from joining modded games.
/// </summary>
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