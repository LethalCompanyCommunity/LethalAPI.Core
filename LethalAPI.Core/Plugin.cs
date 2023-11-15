// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable SA1401 // field should be made private
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using HarmonyTools;

/// <inheritdoc />
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// Gets the singleton for a plugin.
    /// </summary>
    public static Plugin Singleton;

    /// <summary>
    /// Gets the <see cref="Logger"/>.
    /// </summary>
    /// <summary>
    /// The base logger.
    /// </summary>
#pragma warning disable SA1309
    internal static ManualLogSource _Logger;
#pragma warning restore SA1309

    /// <summary>
    /// The harmony instance.
    /// </summary>
    internal static Harmony Harmony;

    private void Awake()
    {
        Harmony = new(PluginInfo.PLUGIN_GUID);
        Harmony.PatchAll(typeof(Plugin).Assembly);
        _Logger = this.Logger;
        Singleton = this;

        Log.Info($"{PluginInfo.PLUGIN_GUID} is being loaded...");

    }

    private static void Test()
    {
        List<Type> typesToGet = new()
        {
            typeof(PlayerControllerB),
        };
        foreach (Type type in typesToGet)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                                                          BindingFlags.Public | BindingFlags.NonPublic |
                                                          BindingFlags.CreateInstance | BindingFlags.GetProperty))
            {
                Harmony.Patch(method, null, null, new HarmonyMethod(typeof(Plugin), nameof(Transpiler)));
            }
        }
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> codeInstructions = instructions.ToList();
        int i0 = InstructionSearchTemplates.ForLoop.Static.FindIndex(codeInstructions);
        int i1 = InstructionSearchTemplates.ForLoop.NonStatic.FindIndex(codeInstructions);
        Log.Debug($"Found {i0} {i1} ({i0 + i1})");
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            yield return codeInstructions[i];
        }
    }
}