// -----------------------------------------------------------------------
// <copyright file="EventTranspilerInjector.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.HarmonyTools;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Events.Features;
using Events.Interfaces;

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
    /// Injects a deniable event into a transpiler add the given index, as well as any prefix args defined in <see cref="prefixInstructions"/>.
    /// </summary>
    /// <param name="instructions">The original method instructions.</param>
    /// <param name="generator">The ILGenerator in use.</param>
    /// <param name="baseMethod">The base method of the patch.</param>
    /// <param name="index">The index to inject the instructions at.</param>
    /// <param name="prefixInstructions">Optional instructions that can be inject prior to the event constructor, that allow loading the stack with parameters for the constructor.</param>
    /// <param name="autoInsertConstructorParameters">When set to true, the injector will auto-determine the parameters for a patch, and add them.</param>
    /// <typeparam name="T">The <see cref="IDeniableEvent"/> to inject.</typeparam>
    public static void InjectDeniableEvent<T>(ref IEnumerable<CodeInstruction> instructions, ref ILGenerator generator, ref MethodBase baseMethod, int index, List<CodeInstruction>? prefixInstructions = null, bool autoInsertConstructorParameters = true)
        where T : IDeniableEvent
    {
        if (instructions is not List<CodeInstruction> list)
        {
            list = instructions.ToList();
        }

        types ??= typeof(Log).Assembly.DefinedTypes
            .Where(x => x.FullName?.StartsWith("LethalAPI.Core.Events.Handlers") ?? false).ToList();

        PropertyInfo? propertyInfo = null;
        foreach (TypeInfo type in types)
        {
            propertyInfo = type
                .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(pty => pty.PropertyType == typeof(Event<T>));

            if (propertyInfo is not null)
            {
                break;
            }
        }

        if (propertyInfo is null)
        {
            Log.Warn($"Could not find an acceptable event handler for event {typeof(T).FullName}! No injection will occur for this event.");
            return;
        }

        List<CodeInstruction> parameterStack = new ();

        List<ParameterInfo> baseParameters = baseMethod.GetParameters().ToList();
        ParameterInfo[] parameters = typeof(T).GetConstructors()[0].GetParameters();
        for (int i = 0; i < parameters.Length; i++)
        {
            ParameterInfo param = parameters[i];

            // this
            if (i == 0 && param.ParameterType == baseMethod.DeclaringType && !baseMethod.IsStatic)
            {
                parameterStack = (List<CodeInstruction>)parameterStack.Append(new(OpCodes.Ldarg_0));
                continue;
            }

            // IsAllowed or IsEnabled.
            if (i == parameters.Length - 1 && param.ParameterType == typeof(bool))
            {
                parameterStack = (List<CodeInstruction>)parameterStack.Append(new(OpCodes.Ldc_I4_1));
                continue;
            }

            CodeInstruction? opCode = null;
            for (int j = 0; j < baseParameters.Count; j++)
            {
                if (baseParameters[j].ParameterType != param.ParameterType || baseParameters[j].Name != param.Name)
                {
                    continue;
                }

                opCode = (j + (baseMethod.IsStatic ? 0 : 1)) switch
                {
                    0 => new CodeInstruction(OpCodes.Ldarg_0),
                    1 => new CodeInstruction(OpCodes.Ldarg_1),
                    2 => new CodeInstruction(OpCodes.Ldarg_2),
                    3 => new CodeInstruction(OpCodes.Ldarg_3),
                    _ => new CodeInstruction(OpCodes.Ldarg_S, j),
                };
                break;
            }

            if (opCode is null)
                continue;

            parameterStack.Insert(i, opCode);
        }

        LocalBuilder local = generator.DeclareLocal(typeof(T));
        Label rtn = generator.DefineLabel();

        List<CodeInstruction> opcodes = new()
        {
            // TEventArgs ev = new()
            // new(OpCodes.Callvirt),
            new(OpCodes.Newobj, GetDeclaredConstructors(typeof(T))[0]),
            new(OpCodes.Dup),
            new(OpCodes.Dup),
            new(OpCodes.Stloc_S, local),
            new(OpCodes.Call, PropertyGetter(propertyInfo.DeclaringType, propertyInfo.Name)),
            new(OpCodes.Call, PropertyGetter(typeof(Event<T>), nameof(Event<T>.InvokeSafely))),

            // Handlers.{Handler}.{Event}.InvokeSafely(ev)

            // if (!ev.IsAllowed)
            //   return
            new(OpCodes.Callvirt, PropertyGetter(typeof(T), nameof(IDeniableEvent.IsAllowed))),
            new(OpCodes.Brfalse, rtn),
            new(OpCodes.Ret),
        };
        if (prefixInstructions is { Count: > 0 })
        {
            list.InsertRange(index, prefixInstructions);
            index += prefixInstructions.Count;
        }

        opcodes.InsertRange(0, parameterStack);
        list.InsertRange(index, opcodes);
        index += opcodes.Count + parameterStack.Count + 1;
        list[index] = list[index].WithLabels();
    }
}