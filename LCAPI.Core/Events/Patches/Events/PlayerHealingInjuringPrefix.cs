// -----------------------------------------------------------------------
// <copyright file="PlayerHealingInjuringPrefix.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LCAPI.Core.Events.Patches.Events;

using Attributes;
using EventArgs.Player;
using GameNetcodeStuff;

/// <summary>
///     Patches the <see cref="Handlers.Player.Healing"/> and <see cref="Handlers.Player.CriticallyInjure"/> event.
/// </summary>
[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.MakeCriticallyInjured))]
[EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.CriticallyInjure))]
[EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Healing))]
internal static class PlayerHealingInjuringPrefix
{
    [HarmonyPrefix]
    private static bool Prefix(PlayerControllerB __instance, bool enable)
    {
        if (enable)
        {
            CriticallyInjureEventArgs injuring = new (__instance);
            Handlers.Player.OnCriticalInjury(injuring);
            return injuring.IsAllowed;
        }
        else
        {
            HealingEventArgs healing = new (__instance);
            Handlers.Player.OnHealing(healing);
            return healing.IsAllowed;
        }
    }
}