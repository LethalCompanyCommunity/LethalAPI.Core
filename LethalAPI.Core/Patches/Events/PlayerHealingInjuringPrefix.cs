// -----------------------------------------------------------------------
// <copyright file="PlayerHealingInjuringPrefix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Patches.Events;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Core.Events.Handlers;
using GameNetcodeStuff;
using HarmonyTools;
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
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        List<CodeInstruction> newInstructions = instructions.ToList();

        EventTranspilerInjector.InjectDeniableEvent<CriticallyInjureEventArgs>(ref newInstructions, ref generator, ref original, 2);
        EventTranspilerInjector.InjectDeniableEvent<HealingEventArgs>(ref newInstructions, ref generator, ref original, 2);

        for (int i = 0; i < newInstructions.Count; i++)
            yield return newInstructions[i];
    }
}