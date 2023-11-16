// -----------------------------------------------------------------------
#pragma warning disable SA1636
// <copyright file="TranspilerHelper.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the MIT License. License:
// https://github.com/o5zereth/ZerethAPI/blob/master/LICENSE.txt
// Made by O5Zereth (an absolute legend). Check out his work here:
// https://github.com/o5zereth/ZerethAPI/
// </copyright>
// -----------------------------------------------------------------------
namespace LethalAPI.Core.Patches.HarmonyTools;

using System;
using System.Collections.Generic;
using System.Linq;

using HarmonyLib;

/// <summary>
/// A utility class to assist in creating HarmonyLib transpilers.
/// </summary>
public static class TranspilerHelper
{
    /// <summary>
    /// Creates a code instruction list from the list pool using the given instructions.
    /// </summary>
    /// <param name="instructions">The original instructions.</param>
    /// <param name="newInstructions">The new list of code instructions to rent.</param>
    public static void BeginTranspiler(this IEnumerable<CodeInstruction> instructions, out List<CodeInstruction> newInstructions)
    {
        newInstructions = instructions.ToList();
    }

    /// <summary>
    /// Returns an enumerable of code instructions and returns the instructions list to the pool.
    /// </summary>
    /// <param name="newInstructions">The instructions list to return to the pool.</param>
    /// <returns>Enumerable representing the finished code of the transpiler.</returns>
    public static IEnumerable<CodeInstruction> FinishTranspiler(this List<CodeInstruction> newInstructions)
    {
        // ReSharper disable once ForCanBeConvertedToForeach
        for (int i = 0; i < newInstructions.Count; i++)
        {
            yield return newInstructions[i];
        }
    }

    /// <summary>
    /// Adds an enumerable of code instructions to a code instruction list.
    /// </summary>
    /// <param name="list">The list to append the instructions to.</param>
    /// <param name="instructions">The instructions to append to the list.</param>
    /// <remarks>This is an extension intended for collection initialization purposes.</remarks>
    public static void Add(this List<CodeInstruction> list, IEnumerable<CodeInstruction> instructions)
    {
        list.AddRange(instructions);
    }

    /// <summary>
    /// Finds the Nth instruction index that matches the specified predicate.
    /// </summary>
    /// <param name="instructions">The instructions to search.</param>
    /// <param name="n">The Nth value to find.</param>
    /// <param name="predicate">The predicate to match for.</param>
    /// <returns>The index of the Nth instruction that matches the predicate, or -1 if not found.</returns>
    public static int FindNthInstruction(this List<CodeInstruction> instructions, int n, Func<CodeInstruction, bool> predicate)
    {
        if (n <= 0)
        {
            return -1;
        }

        for (int i = 0; n < instructions.Count; i++)
        {
            if (!predicate(instructions[i]))
            {
                continue;
            }

            if (--n == 0)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Finds the Nth instruction index that matches the specified predicate, in reverse.
    /// </summary>
    /// <param name="instructions">The instructions to search.</param>
    /// <param name="n">The Nth value to find in reverse.</param>
    /// <param name="predicate">The predicate to match for.</param>
    /// <returns>The index of the Nth instruction that matches the predicate in reverse order, or -1 if not found.</returns>
    public static int FindNthInstructionReverse(this List<CodeInstruction> instructions, int n, Func<CodeInstruction, bool> predicate)
    {
        if (n <= 0)
        {
            return -1;
        }

        for (int i = instructions.Count - 1; n >= 0; i--)
        {
            if (!predicate(instructions[i]))
            {
                continue;
            }

            if (--n == 0)
            {
                return i;
            }
        }

        return -1;
    }
}