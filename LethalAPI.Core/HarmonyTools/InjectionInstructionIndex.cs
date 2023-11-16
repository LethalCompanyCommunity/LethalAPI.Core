// -----------------------------------------------------------------------
// <copyright file="InjectionInstructionIndex.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.HarmonyTools;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Tools to inject instructions at an index.
/// </summary>
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
public class InjectionInstructionIndex
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InjectionInstructionIndex"/> class.
    /// </summary>
    public InjectionInstructionIndex()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InjectionInstructionIndex"/> class.
    /// </summary>
    /// <param name="index">The index the instructions will be injected at.</param>
    /// <param name="instructions">The instructions to inject.</param>
    public InjectionInstructionIndex(ushort index, IEnumerable<CodeInstruction> instructions)
    {
        this.InstructionsToInject = instructions;
        this.Index = index;
    }

    /// <summary>
    /// Gets the instructions that will be injected.
    /// </summary>
    public IEnumerable<CodeInstruction> InstructionsToInject { get; init; } = null!;

    /// <summary>
    /// Gets the index which the instructions will be injected at.
    /// </summary>
    public ushort Index { get; init; }

    /// <summary>
    /// Injects the instructions.
    /// </summary>
    /// <param name="instructions">The instructions that the new instructions will be injected into.</param>
    /// <param name="offset">The offset to account for (to consider previous instructions).</param>
    /// <returns>The instructions with the new instructions injected.</returns>
    public IEnumerable<CodeInstruction> Inject(IEnumerable<CodeInstruction> instructions, int offset = 0)
    {
        List<CodeInstruction> res = instructions.ToList();
        res.InsertRange(this.Index + offset, this.InstructionsToInject);
        return res;
    }
}