// -----------------------------------------------------------------------
// <copyright file="InstructionSearchTemplates.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.HarmonyTools;

using System.Collections.Generic;
using System.Reflection.Emit;

/// <summary>
/// Provides tools and templates to use to search and find ilcode in an index.
/// </summary>
public static class InstructionSearchTemplates
{
    /// <summary>
    /// Contains search templates for various type of 'for' loops.
    /// </summary>
    public static class ForLoop
    {
        /// <summary>
        /// Gets <see cref="InstructionsToSearch"/> information for 'for' loops that refer to a instance.
        /// </summary>
        /// <example>
        /// <code>
        /// public int MaxIndex { get; set; } = 4;
        /// for (int i = 0; i &lt; Instance.MaxIndex; i++)
        /// </code>
        /// </example>
        public static InstructionsToSearch NonStatic
        {
            get
            {
                InstructionsToSearch start = Static;
                start.InstructionsToFind.InsertRange(1, new List<List<OpCode>>()
                {
                    new()
                    {
                        OpCodes.Ldfld, OpCodes.Ldflda, OpCodes.Call, OpCodes.Callvirt,
                    },
                });
                return start;
            }
        }

        /// <summary>
        /// Gets <see cref="InstructionsToSearch"/> information for 'for' loops that refer to static instances.
        /// </summary>
        /// <example>
        /// <code>
        /// public static int GetMaxIndex() => 4;
        /// for (int i = 0; i &lt; MaxIndex; i++)
        /// </code>
        /// </example>
        public static InstructionsToSearch Static =>
            new (new List<List<OpCode>>()
            {
                new()
                { // Any Variation of LdLoc
                    OpCodes.Ldloc, OpCodes.Ldloc_0, OpCodes.Ldloc_1, OpCodes.Ldloc_2,
                    OpCodes.Ldloc_3, OpCodes.Ldloca, OpCodes.Ldloc_S, OpCodes.Ldloca_S,
                },
                new()
                { // Any Variation of LDC_I4 (load int)
                    OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_1, OpCodes.Ldc_I4_2, OpCodes.Ldc_I4_3,
                    OpCodes.Ldc_I4_4, OpCodes.Ldc_I4_5, OpCodes.Ldc_I4_6, OpCodes.Ldc_I4_7,
                    OpCodes.Ldc_I4_8, OpCodes.Ldc_I4_S, OpCodes.Ldc_I4_M1, OpCodes.Ldc_I4,
                    OpCodes.Ldfld, OpCodes.Ldflda, OpCodes.Call, OpCodes.Callvirt,
                },
                new() { OpCodes.Clt, OpCodes.Clt_Un },
                new()
                { // Any Variation of Stloc
                    OpCodes.Stloc, OpCodes.Stloc_0, OpCodes.Stloc_1, OpCodes.Stloc_2,
                    OpCodes.Stloc_3, OpCodes.Stloc_S,
                },
                new()
                { // Any Variation of LdLoc
                    OpCodes.Ldloc, OpCodes.Ldloc_0, OpCodes.Ldloc_1, OpCodes.Ldloc_2,
                    OpCodes.Ldloc_3, OpCodes.Ldloca, OpCodes.Ldloc_S, OpCodes.Ldloca_S,
                },
                new()
                {
                    OpCodes.Brtrue, OpCodes.Brtrue_S, OpCodes.Brfalse, OpCodes.Brfalse_S,
                    OpCodes.Br, OpCodes.Br_S,
                },
            });
    }
}
