// -----------------------------------------------------------------------
// <copyright file="InstructionsToSearch.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.HarmonyTools;

using System.Collections.Generic;
using System.Linq;

using OpCode = System.Reflection.Emit.OpCode;

/// <summary>
/// A helper tool to find code at an index.
/// </summary>
// ReSharper disable MemberCanBePrivate.Global
public readonly struct InstructionsToSearch
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionsToSearch"/> struct.
    /// </summary>
    public InstructionsToSearch()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InstructionsToSearch"/> struct.
    /// </summary>
    /// <param name="instructionsToFind">The instructions to find.</param>
    /// <param name="index">The index to find.</param>
    public InstructionsToSearch(IEnumerable<List<OpCode>> instructionsToFind, int index = 0)
    {
        this.InstructionsToFind = instructionsToFind as List<List<OpCode>> ?? instructionsToFind.ToList();
        this.Index = index;
    }

    /// <summary>
    /// Gets the instructions that are being searched for.
    /// </summary>
    public List<List<OpCode>> InstructionsToFind { get; init; } = null!;

    /// <summary>
    /// Gets the index of these instructions to find.
    /// </summary>
    public int Index { get; init; } = 0;

    /// <summary>
    /// Gets the index of the instructions from a set of instructions.
    /// </summary>
    /// <param name="instructions">The instructions to search.</param>
    /// <returns>The index of the instructions or -1 if the instructions weren't found.</returns>
    public int FindIndex(IEnumerable<CodeInstruction> instructions)
    {
        int startIndex = -1;
        List<CodeInstruction> list = instructions as List<CodeInstruction> ?? instructions.ToList();

        int totalFoundInstances = 0;
        int foundIndex = 0;
        bool trailFound = false;
        List<OpCode> firstItem = this.InstructionsToFind[0];

        for (int i = 0; i < list.Count; i++)
        {
            if (trailFound)
            {
                if (foundIndex >= this.InstructionsToFind.Count)
                {
                    if (totalFoundInstances == this.Index)
                    {
                        return startIndex;
                    }

                    totalFoundInstances++;
                    startIndex = -1;
                    foundIndex = 0;
                    trailFound = false;
                    continue;
                }

                if (!this.InstructionsToFind[foundIndex].Contains(list[i].opcode))
                {
                    // debug += $"incorrect index {foundIndex}\n";
                    i = startIndex;
                    startIndex = -1;
                    foundIndex = 0;
                    trailFound = false;
                    continue;
                }

                foundIndex++;
                continue;
            }

            if (firstItem.Contains(list[i].opcode))
            {
                trailFound = true;
                startIndex = i;
                foundIndex++;
            }
        }

        return -1;
    }
}