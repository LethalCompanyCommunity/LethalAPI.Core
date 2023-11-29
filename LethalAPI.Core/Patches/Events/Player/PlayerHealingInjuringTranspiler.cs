// -----------------------------------------------------------------------
// <copyright file="PlayerHealingInjuringTranspiler.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Patches.Events.Player;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using GameNetcodeStuff;
using HarmonyTools;
using LethalAPI.Core.Events.Attributes;
using LethalAPI.Core.Events.EventArgs.Player;

/// <summary>
///     Patches the <see cref="HandlersPlayer.Healing"/> and <see cref="HandlersPlayer.CriticallyInjure"/> event.
/// </summary>
[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.MakeCriticallyInjured))]
[EventPatch(typeof(HandlersPlayer), nameof(HandlersPlayer.CriticallyInjure))]
[EventPatch(typeof(HandlersPlayer), nameof(HandlersPlayer.Healing))]
internal static class PlayerHealingInjuringTranspiler
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        List<CodeInstruction> newInstructions = instructions.ToList();

        int index = newInstructions.FindNthInstruction(2, instruction => instruction.opcode == OpCodes.Ret);
        EventTranspilerInjector.InjectDeniableEvent<HealingEventArgs>(ref newInstructions, ref generator, ref original, index + 1);
        EventTranspilerInjector.InjectDeniableEvent<CriticallyInjureEventArgs>(ref newInstructions, ref generator, ref original, 2);

        foreach (CodeInstruction? instruction in newInstructions)
            yield return instruction;
    }
}