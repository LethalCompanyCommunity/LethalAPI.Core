// -----------------------------------------------------------------------
// <copyright file="TranspilerExtensions.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.HarmonyTools;

#pragma warning disable SA1201 // enum should not follow a field.

using System;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

/// <summary>
/// Contains a set of extensions for use with making transpilers.
/// </summary>
public static class TranspilerExtensions
{
    /// <summary>
    /// Logs a transpiler code instruction.
    /// </summary>
    /// <param name="instruction">The instruction to log.</param>
    /// <param name="index">The index it's being injected at.</param>
    /// <param name="injectedIndex">The additional index the instruction is being added at (if new instruction).</param>
    /// <param name="enable">An integrated if return statement.</param>
    /// <param name="debug">Should the output have advanced logging features. This can clutter the console so it's disabled by default.</param>
    /// <returns>The original instruction (unmodified).</returns>
    public static CodeInstruction Log(this CodeInstruction instruction, int index = 0, int injectedIndex = -1, bool enable = true, bool debug = false)
    {
        if (!enable)
            return instruction;
        Core.Log.Debug(GetOpcodeDebugLabel(instruction, index, injectedIndex, debug ? OutputIncludes.Debug : OutputIncludes.Basic));
        return instruction;
    }

    /// <summary>
    /// Logs a transpiler code instruction.
    /// </summary>
    /// <param name="instruction">The instruction to log.</param>
    /// <param name="index">The index it's being injected at.</param>
    /// <param name="injectedIndex">The additional index the instruction is being added at (if new instruction).</param>
    /// <param name="output">The output flags to be added to the output.</param>
    /// <returns>The original instruction (unmodified).</returns>
    public static string GetOpcodeDebugLabel(this CodeInstruction instruction, int index, int injectedIndex = -1, OutputIncludes output = OutputIncludes.Basic)
    {
        if (output == OutputIncludes.None)
            return string.Empty;

        string labelOperand = string.Empty;
        if (output.HasFlag(OutputIncludes.LabelOperand))
        {
            // if opcode has a label operand
            if (instruction.operand is Label label)
                labelOperand += $"-> ({label.GetHashCode()})";
        }

        if (output.HasFlag(OutputIncludes.FieldOperand))
        {
            // if opcode has a field operand
            if (instruction.operand is FieldInfo field)
                labelOperand += $"-> (<{field.FieldType.Name}> {field.Name})";
        }

        if (output.HasFlag(OutputIncludes.MethodOperand))
        {
            // if opcode has a method operand
            if (instruction.operand is MethodInfo method)
            {
                string paramList = string.Empty;
                foreach (ParameterInfo param in method.GetParameters())
                {
                    paramList += $" [{param.ParameterType.Name}]";
                }

                labelOperand += $"-> (<{method.ReturnType.Name}> {method.Name}{paramList})";
            }
        }

        if (output.HasFlag(OutputIncludes.ArgOperand))
        {
            // if opcode has a argument operand
            if ((instruction.opcode == OpCodes.Ldarg_S || instruction.opcode == OpCodes.Ldarga_S || instruction.opcode == OpCodes.Starg_S || instruction.opcode == OpCodes.Ldarg || instruction.opcode == OpCodes.Starg_S)
                && instruction.operand is int arg)
            {
                labelOperand += $"-> ({arg})";
            }
        }

        if (output.HasFlag(OutputIncludes.ValueOperand))
        {
            // if opcode has a value operand
            if (instruction.operand?.GetType().IsValueType ?? false)
            {
                if (instruction.operand is not Label)
                    labelOperand += $"-> (<{instruction.operand.GetType().Name}> {instruction.operand})";
            }
        }

        // adds labels
        string notedLabels = string.Empty;
        if (output.HasFlag(OutputIncludes.Labels))
        {
            notedLabels = instruction.labels.Count > 0 ? "   " : string.Empty;
            foreach (Label x in instruction.labels)
            {
                notedLabels += $" [{x.GetHashCode()}]";
            }
        }

        string injectedString = injectedIndex < 0 ? "   " : $"+{injectedIndex:00}";
        string indexes = string.Empty;
        if (output.HasFlag(OutputIncludes.Index))
            indexes += $"[{index:000}{(output.HasFlag(OutputIncludes.InjectedIndex) ? $" {injectedString}" : string.Empty)}]";

        return $"{indexes} {instruction.opcode,-10}{labelOperand,-7}{notedLabels}";
    }

    /// <summary>
    /// The information to include in the output.
    /// </summary>
    [Flags]
    public enum OutputIncludes
    {
        /// <summary>
        /// No output will be present.
        /// </summary>
        None = 0,

        /// <summary>
        /// All output information is present.
        /// </summary>
        All = ~0,

        /// <summary>
        /// Includes basic output information. Good for basic troubleshooting.
        /// </summary>
        Basic = 1 | 2 | 4 | 8,

        /// <summary>
        /// All output information is present. Good for intense troubleshooting.
        /// </summary>
        Debug = All,

        /// <summary>
        /// The index of the opcode will be shown. (Not the IL label, as it is not yet generated.)
        /// </summary>
        Index = 1,

        /// <summary>
        /// The injected index will be shown (only if applicable).
        /// </summary>
        InjectedIndex = 2,

        /// <summary>
        /// Skip location labels will be shown.
        /// </summary>
        Labels = 4,

        /// <summary>
        /// All operand information will be shown.
        /// </summary>
        AllOperands = 8 | 16 | 32 | 64 | 128,

        /// <summary>
        /// Label information will be shown.
        /// </summary>
        LabelOperand = 8,

        /// <summary>
        /// Field information will be shown.
        /// </summary>
        FieldOperand = 16,

        /// <summary>
        /// Method information will be shown.
        /// </summary>
        MethodOperand = 32,

        /// <summary>
        /// Argument information will be shown.
        /// </summary>
        ArgOperand = 64,

        /// <summary>
        /// Value Type information will be shown.
        /// </summary>
        ValueOperand = 128,
    }
}