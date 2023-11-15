// -----------------------------------------------------------------------
// <copyright file="RemovableInstructionIndex.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.HarmonyTools;

/// <summary>
/// Tools to remove instructions at an index.
/// </summary>
// ReSharper disable UnusedAutoPropertyAccessor.Global
public struct RemovableInstructionIndex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemovableInstructionIndex"/> struct.
    /// </summary>
    public RemovableInstructionIndex()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RemovableInstructionIndex"/> struct.
    /// </summary>
    /// <param name="index">The index of the ships.</param>
    /// <param name="instructionsToRemove">The amount of instructions to remove.</param>
    public RemovableInstructionIndex(ushort index, ushort instructionsToRemove)
    {
        this.Index = index;
        this.InstructionsToRemove = instructionsToRemove;
    }

    /// <summary>
    /// Gets the index to start removing instructions at.
    /// </summary>
    public ushort Index { get; init; }

    /// <summary>
    /// Gets a count of how many instructions will be removed.
    /// </summary>
    public ushort InstructionsToRemove { get; init; }
}