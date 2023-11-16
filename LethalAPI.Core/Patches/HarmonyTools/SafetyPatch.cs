// ReSharper disable InconsistentNaming
#pragma warning disable SA1201
// -----------------------------------------------------------------------
// <copyright file="SafetyPatch.cs" company="LethalAPI Modding Community">
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
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

/// <summary>
/// Allows for safely patching a method to include a try catch. This is the implementation.
/// </summary>
public static class SafetyPatch
{
    /// <summary>
    /// Applies a safety patch onto a method, injecting a try-catch block.
    /// </summary>
    /// <param name="harmony">The harmony instance to apply this patch with.</param>
    /// <param name="type">The type of the method to patch.</param>
    /// <param name="name">The name of the method to patch.</param>
    /// <param name="handler">The handler to invoke upon an exception in the specified method.</param>
    /// <exception cref="NullReferenceException">Thrown when the method could not be found.</exception>
    public static void ApplySafetyPatch(this Harmony harmony, Type type, string name, Action<Exception> handler)
    {
        MethodInfo method = AccessTools.Method(type, name) ?? throw new NullReferenceException("Method could not be found.");

        harmony.ApplySafetyPatch(method, handler);
    }

    /// <summary>
    /// Applies a safety patch onto a method, injecting a try-catch block.
    /// </summary>
    /// <param name="harmony">The harmony instance to apply this patch with.</param>
    /// <param name="method">The method to patch.</param>
    /// <param name="handler">The handler to invoke upon an exception in the specified method.</param>
    /// <exception cref="ArgumentNullException">Thrown when the handler is null.</exception>
    // ReSharper disable once MemberCanBePrivate.Global
    public static void ApplySafetyPatch(this Harmony harmony, MethodInfo method, Action<Exception> handler)
    {
        if (handler is null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        SafetyPatch.handleException = handler;

        harmony.Patch(method, null, null, TranspilerMethod);
    }

    /// <summary>
    /// Contains the action to execute on an exception.
    /// </summary>
    private static Action<Exception>? handleException;

    /// <summary>
    /// Gets the transpiler method to use.
    /// </summary>
    private static HarmonyMethod TranspilerMethod => new(typeof(SafetyPatch), nameof(Transpiler));

    /// <summary>
    /// The transpiler to execute.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase method, ILGenerator generator)
    {
        if (method is not MethodInfo info)
        {
            throw new InvalidCastException($"Could not convert method to MethodInfo. This is required to determine the return type of the method.");
        }

        if (handleException is null)
        {
            throw new InvalidOperationException($"The exception handler delegate is null.");
        }

        if (!handleException.Method.IsStatic)
        {
            throw new InvalidOperationException($"The exception handler delegate must target a static method.");
        }

        instructions.BeginTranspiler(out List<CodeInstruction> newInstructions);

        // Behaviour changes depending on whether the method returns void.
        if (info.ReturnType == typeof(void))
        {
            HandleVoidReturn(newInstructions, generator);
        }
        else
        {
            HandleReturn(newInstructions, generator, info.ReturnType);
        }

        return newInstructions.FinishTranspiler();
    }

    private static void HandleVoidReturn(List<CodeInstruction> instructions, ILGenerator generator)
    {
        Label exitExceptionBlock = generator.DefineLabel();

        instructions[0].blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock));

        // Catch the exception, call the delegate, then return.
        instructions.AddRange(new[]
        {
                new CodeInstruction(OpCodes.Call, handleException!.Method)
                    .WithBlocks(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, typeof(Exception))),
                new CodeInstruction(OpCodes.Leave_S, exitExceptionBlock)
                    .WithBlocks(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock)),

                new CodeInstruction(OpCodes.Ret)
                    .WithLabels(exitExceptionBlock),
        });

        // Replace ret instructions with leave instructions.
        for (int i = 0; i < instructions.Count - 1; i++)
        {
            CodeInstruction code = instructions[i];

            if (code.opcode != OpCodes.Ret)
                continue;

            code.opcode = OpCodes.Leave_S;
            code.operand = exitExceptionBlock;
        }
    }

    private static void HandleReturn(List<CodeInstruction> instructions, ILGenerator generator, Type returnType)
    {
        Label exitExceptionBlock = generator.DefineLabel();
        LocalBuilder returnLocal = generator.DeclareLocal(returnType);

        instructions[0].blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock));

        // Catch the exception, call the delegate, assign the result to default, then return.
        instructions.AddRange(new[]
        {
                new CodeInstruction(OpCodes.Call, handleException!.Method)
                    .WithBlocks(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, typeof(Exception))),

                // returnLocal = default;
                new(OpCodes.Ldloca_S, returnLocal),
                new(OpCodes.Initobj, returnType),

                new CodeInstruction(OpCodes.Leave_S, exitExceptionBlock)
                    .WithBlocks(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock)),

                // return returnLocal;
                new CodeInstruction(OpCodes.Ldloc_S, returnLocal)
                    .WithLabels(exitExceptionBlock),
                new(OpCodes.Ret),
        });

        // Replace ret instructions with leave.
        // instructions and assign the return local.
        for (int i = 0; i < instructions.Count - 1; i++)
        {
            CodeInstruction code = instructions[i];

            if (code.opcode != OpCodes.Ret)
                continue;

            code.opcode = OpCodes.Stloc_S;
            code.operand = returnLocal;

            instructions.Insert(++i, new CodeInstruction(OpCodes.Leave_S, exitExceptionBlock));
        }
    }
}
