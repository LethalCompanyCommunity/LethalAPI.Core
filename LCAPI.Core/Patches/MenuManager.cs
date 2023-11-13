// -----------------------------------------------------------------------
// <copyright file="MenuManager.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Patches;

using HarmonyLib;

[HarmonyPatch(typeof(MenuManager))]
internal class MenuManagerPatches
{

    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    private static void Awake_Postfix(MenuManager __instance)
    {
        if (__instance != null && __instance.versionNumberText != null)
        {
            __instance.versionNumberText.text = $"{GameNetworkManager.Instance.gameVersionNum}\nMOD";
        }
    }
}