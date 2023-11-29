// -----------------------------------------------------------------------
// <copyright file="UsingItemPrefix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Patches.Events.Player;

using LethalAPI.Core.Events.Attributes;
using LethalAPI.Core.Events.EventArgs.Player;

/// <summary>
///     Patches the <see cref="HandlersPlayer.UsingItem"/> event.
/// </summary>
[EventPatch(typeof(HandlersPlayer), nameof(HandlersPlayer.UsingKey))]
[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.ItemActivate))]
internal static class UsingItemPrefix
{
    [HarmonyPrefix]
    private static bool Prefix(GrabbableObject __instance, bool used, bool buttonDown = true)
    {
        // This needs to become a transpiler.
        UsingItemEventArgs ev = new UsingItemEventArgs(__instance);
        HandlersPlayer.UsingItem.InvokeSafely(ev);
        return ev.IsAllowed;
    }
}