// -----------------------------------------------------------------------
// <copyright file="PlayerHealingInjuringPrefix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Patches.Events;

using Core.Events.Handlers;
using GameNetcodeStuff;
using LethalAPI.Core.Events.Attributes;
using LethalAPI.Core.Events.EventArgs.Player;

/// <summary>
///     Patches the <see cref="Player.Healing"/> and <see cref="Player.CriticallyInjure"/> event.
/// </summary>
[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.MakeCriticallyInjured))]
[EventPatch(typeof(Player), nameof(Player.CriticallyInjure))]
[EventPatch(typeof(Player), nameof(Player.Healing))]
internal static class PlayerHealingInjuringPrefix
{
    [HarmonyPrefix]
    private static bool Prefix(PlayerControllerB __instance, bool enable)
    {
        if (enable)
        {
            CriticallyInjureEventArgs injuring = new (__instance);
            Player.OnCriticalInjury(injuring);
            return injuring.IsAllowed;
        }
        else
        {
            HealingEventArgs healing = new (__instance);
            Player.OnHealing(healing);
            return healing.IsAllowed;
        }
    }
}