// -----------------------------------------------------------------------
// <copyright file="FixExceptionIL.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Fixes;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;

using HarmonyTools;

/// <summary>
/// Patches the stacktrace to prevent the module id from showing up.
/// </summary>
[HarmonyPatch(typeof(StackTrace), "AddFrames")]
internal static class FixExceptionIL
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> list = instructions as List<CodeInstruction> ?? instructions.ToList();
        int index = list.FindNthInstructionReverse(1, x => x.opcode == OpCodes.Ldstr);
        list[index].operand = " in {1} ";
        list.RemoveRange(index + 1, 1);
        for (int i = 0; i < list.Count; i++)
            yield return list[i];
    }
}