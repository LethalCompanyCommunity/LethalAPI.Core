// -----------------------------------------------------------------------
// <copyright file="KeyItemPatch.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Events.Patches.Events;

using Attributes;
using UnityEngine;

/// <summary>
///     Patches the <see cref="Handlers.Player.UsingKey"/> event.
/// </summary>
[EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsingKey))]
[HarmonyPatch(typeof(KeyItem), nameof(KeyItem.ItemActivate))]
internal sealed class KeyItemPatch
{
    [HarmonyPrefix]
    private static bool Prefix(KeyItem __instance, bool used, bool buttonDown = true)
    {
        // This needs to become a transpiler.
        if (!(__instance.playerHeldBy == null) && __instance.IsOwner && Physics.Raycast(new Ray(__instance.playerHeldBy.gameplayCamera.transform.position, __instance.playerHeldBy.gameplayCamera.transform.forward), out RaycastHit hitInfo, 3f, 2816))
        {
            DoorLock component = hitInfo.transform.GetComponent<DoorLock>();
            if (component != null && component.isLocked && !component.isPickingLock)
            {
                component.UnlockDoorSyncWithServer();
                __instance.playerHeldBy.DespawnHeldObject();
            }
        }

        return false;
    }
}