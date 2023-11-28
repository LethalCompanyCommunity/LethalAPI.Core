// -----------------------------------------------------------------------
// <copyright file="EventTranspilerInjector.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.HarmonyTools;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using LethalAPI.Core.Events.Features;
using LethalAPI.Core.Events.Interfaces;

using static AccessTools;

using OpCodes = System.Reflection.Emit.OpCodes;

/// <summary>
/// Provides methods for injecting events in various transpilers.
/// </summary>
// ReSharper disable InconsistentNaming
public static class EventTranspilerInjector
{
    private static List<TypeInfo>? types;

    /// <summary>
    /// Injects a deniable event into a transpiler add the given index.
    /// </summary>
    /// <param name="instructions">The original method instructions.</param>
    /// <param name="generator">The ILGenerator in use.</param>
    /// <param name="baseMethod">The base method of the patch.</param>
    /// <param name="index">The index to inject the instructions at.</param>
    /// <param name="prefixInstructions">Optional instructions that can be inject prior to the event constructor, that allow loading the stack with parameters for the constructor.</param>
    /// <param name="autoInsertConstructorParameters">When set to true, the injector will auto-determine the parameters for a patch, and add them.</param>
    /// <typeparam name="T">The <see cref="IDeniableEvent"/> to inject.</typeparam>
    public static void InjectDeniableEvent<T>(ref List<CodeInstruction> instructions, ref ILGenerator generator, ref MethodBase baseMethod, int index, List<CodeInstruction>? prefixInstructions = null, bool autoInsertConstructorParameters = true)
        where T : IDeniableEvent
    {
        PropertyInfo? propertyInfo = GetEventPropertyInfo<T>();

        if (propertyInfo is null)
        {
            Log.Warn($"Could not find an acceptable event handler for event {typeof(T).FullName}! No injection will occur for this event.");
            return;
        }

        CodeInstruction originalInstruction = instructions[index];

        List<CodeInstruction> parameterStack = autoInsertConstructorParameters
            ? CreateEventParameters<T>(baseMethod)
            : new List<CodeInstruction>();

        Label rtn = generator.DefineLabel();

        List<CodeInstruction> opcodes = new()
        {
            parameterStack,
            CreateEventArgsObject<T>(),
            new(OpCodes.Dup),
            CreateEventAction<T>(),
            CreateEventDenyReturn<T>(rtn),
        };
        if (prefixInstructions is { Count: > 0 })
        {
            instructions.InsertRange(index, prefixInstructions);
            index += prefixInstructions.Count;
        }

        instructions.InsertRange(index, opcodes);
        instructions[instructions.Count - 1].WithLabels(rtn);

        if (originalInstruction.labels.Count > 0)
        {
            instructions[index].labels.AddRange(originalInstruction.labels);
            originalInstruction.labels.Clear();
        }
    }

    /// <summary>
    /// Create instructions which return if the given <see cref="IDeniableEvent"/> is not allowed.
    /// </summary>
    /// <param name="ret">The return label to branch to.</param>
    /// <typeparam name="T">An <see cref="IDeniableEvent"/>; event.</typeparam>
    /// <returns>A list of <see cref="CodeInstruction"/>s that return if the event is not allowed.</returns>
    internal static IEnumerable<CodeInstruction> CreateEventDenyReturn<T>(Label ret)
        where T : IDeniableEvent
    {
        return new List<CodeInstruction>
        {
            new(OpCodes.Callvirt, PropertyGetter(typeof(T), nameof(IDeniableEvent.IsAllowed))),
            new(OpCodes.Brfalse, ret),
        };
    }

    /// <summary>
    /// Create instructions to construct the given <see cref="ILethalApiEvent"/>.
    /// </summary>
    /// <typeparam name="T">An <see cref="ILethalApiEvent"/>; event.</typeparam>
    /// <returns>A <see cref="CodeInstruction"/> that constructs the event.</returns>
    internal static CodeInstruction CreateEventArgsObject<T>()
        where T : ILethalApiEvent
    {
        return new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(T))[0]);
    }

    /// <summary>
    /// Create instruction to call the delegate for the given <see cref="ILethalApiEvent"/>.
    /// </summary>
    /// <typeparam name="T">An <see cref="ILethalApiEvent"/>; event.</typeparam>
    /// <returns>A <see cref="CodeInstruction"/> that calls the event delegate.</returns>
    /// <exception cref="Exception">Thrown if the event delegate could not be found.</exception>
    internal static CodeInstruction CreateEventAction<T>()
        where T : ILethalApiEvent
    {
        PropertyInfo? propertyInfo = GetEventPropertyInfo<T>();
        if (propertyInfo?.GetValue(null) is not Event<T> @event)
        {
            throw new Exception($"Failed to get event {typeof(T).Name}!");
        }

        return Transpilers.EmitDelegate((Action<T>)Action);

        void Action(T eventArgs) => @event.InvokeSafely(eventArgs);
    }

    /// <summary>
    /// Creates instructions to load event parameters for the given <see cref="ILethalApiEvent"/>.
    /// </summary>
    /// <param name="originalMethod">The original method being transpiled.</param>
    /// <typeparam name="T">An <see cref="ILethalApiEvent"/> event.</typeparam>
    /// <returns>A list of <see cref="CodeInstruction"/>s that load the event parameters.</returns>
    internal static List<CodeInstruction> CreateEventParameters<T>(MethodBase originalMethod)
        where T : ILethalApiEvent
    {
        ParameterInfo[] originalMethodParameters = originalMethod.GetParameters();
        ParameterInfo[] eventConstructorParameters = GetDeclaredConstructors(typeof(T))[0].GetParameters();

        List<CodeInstruction> parameterStack = new();
        for (int i = 0; i < eventConstructorParameters.Length; i++)
        {
            ParameterInfo parameter = eventConstructorParameters[i];

            if (i == 0 && parameter.ParameterType == originalMethod.DeclaringType && !originalMethod.IsStatic)
            {
                parameterStack.Insert(parameterStack.Count, new CodeInstruction(OpCodes.Ldarg_0));
                continue;
            }

            if (i == eventConstructorParameters.Length - 1 && parameter.ParameterType == typeof(bool))
            {
                parameterStack.Insert(parameterStack.Count, new CodeInstruction(OpCodes.Ldc_I4_1));
                continue;
            }

            for (int j = 0; j < originalMethodParameters.Length; j++)
            {
                ParameterInfo originalMethodParameter = originalMethodParameters[j];
                if (originalMethodParameter.ParameterType != parameter.ParameterType ||
                    originalMethodParameter.Name != parameter.Name)
                {
                    continue;
                }

                CodeInstruction instruction = (j + (originalMethod.IsStatic ? 0 : 1)) switch
                    {
                        0 => new CodeInstruction(OpCodes.Ldarg_0),
                        1 => new CodeInstruction(OpCodes.Ldarg_1),
                        2 => new CodeInstruction(OpCodes.Ldarg_2),
                        3 => new CodeInstruction(OpCodes.Ldarg_3),
                        var n => new CodeInstruction(OpCodes.Ldarg_S, n),
                    };

                parameterStack.Insert(parameterStack.Count, instruction);
            }
        }

        return parameterStack;
    }

    /// <summary>
    /// Finds the <see cref="Event{T}"/> property for the given <see cref="ILethalApiEvent"/>.
    /// </summary>
    /// <typeparam name="T">An <see cref="ILethalApiEvent"/> event.</typeparam>
    /// <returns>The <see cref="Event{T}"/> property info for the given <see cref="ILethalApiEvent"/>.</returns>
    private static PropertyInfo? GetEventPropertyInfo<T>()
        where T : ILethalApiEvent
    {
        types ??= typeof(Log).Assembly.DefinedTypes
            .Where(x => x.FullName?.StartsWith("LethalAPI.Core.Events.Handlers") ?? false).ToList();

        return types
            .Select(type => type
                .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(pty => pty.PropertyType == typeof(Event<T>)))
            .FirstOrDefault(propertyInfo => propertyInfo is not null);
    }
}