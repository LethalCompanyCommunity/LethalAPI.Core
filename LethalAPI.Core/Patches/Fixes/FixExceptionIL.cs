// -----------------------------------------------------------------------
// <copyright file="FixExceptionIL.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Fixes;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using MonoMod.Cil;

/// <summary>
/// Patches the stacktrace to prevent the module id from showing up.
/// </summary>
[HarmonyPatch(typeof(StackTrace), "AddFrames")]
[HarmonyPriority(Priority.First)]
internal static class FixExceptionIL
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> list = instructions as List<CodeInstruction> ?? instructions.ToList();
        int index = list.FindLastIndex(x => x.opcode == OpCodes.Ldstr);
        int ogILReplacement = list.FindLastIndex(x => x.opcode == OpCodes.Ldstr && (string)x.operand == " [0x{0:x5}]");
        list[ogILReplacement].operand = string.Empty;
        list[index].operand = " at {1} ";
        list[index + 1] = new CodeInstruction(OpCodes.Ldstr, string.Empty);
        for (int i = 0; i < list.Count; i++)
        {
            yield return list[i];
        }
    }

    /// <summary>
    /// Hooks the IL Code and modifies it.
    /// </summary>
    /// <param name="il">The IL Context.</param>
    internal static void IlHook(ILContext il)
    {
        ILCursor cursor = new (il);
        cursor.GotoNext(x => x.MatchCallvirt(typeof(StackFrame).GetMethod("GetFileLineNumber", BindingFlags.Instance | BindingFlags.Public)));

        cursor.RemoveRange(2);
        cursor.EmitDelegate<Func<StackFrame, string>>(GetLineOrIL);
    }

    /// <summary>
    /// Gets the new IL label string.
    /// </summary>
    /// <param name="instance">The stack fram instance.</param>
    /// <returns>Returns the new IL label string.</returns>
    internal static string GetLineOrIL(StackFrame instance)
    {
        int line = instance.GetFileLineNumber();
        if (line is StackFrame.OFFSET_UNKNOWN or 0)
        {
            return Log.Templates["LineLocNotFound"].Insert(23, instance.GetILOffset().ToString("X4"));
        }

        return Log.Templates["LineLocFound"]
            .Insert(16, instance.GetILOffset().ToString("X4"))
            .Insert(7, line.ToString());
    }
}