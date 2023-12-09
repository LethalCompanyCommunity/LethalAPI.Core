// -----------------------------------------------------------------------
// <copyright file="FixBepInExLoggerPrefix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Fixes;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using BepInEx.Logging;

using static AccessTools;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedParameter.Local
// This now is called manually in the plugin loader.
#pragma warning disable SA1402
#pragma warning disable SA1313

/// <summary>
/// Contains the new fix for bepinex logging.
/// </summary>
internal static class BepInExLogFix
{
    private static MethodInfo setConsoleColor = null!;

    private static TextWriter textWriter = null!;

    private static Dictionary<string, string> Templates { get; set; } = new();

    /// <summary>
    /// Patches the console logger.
    /// </summary>
    /// <param name="harmony">The harmony instance to patch with.</param>
    internal static void Patch(HarmonyLib.Harmony harmony)
    {
        Templates.Add("Info", Log.Templates["Info"]);
        Templates.Add("Debug", Log.Templates["Debug"]);
        Templates.Add("Warn", Log.Templates["Warn"]);
        Templates.Add("Error", Log.Templates["Error"]);

        // Get variables and default output.
        Type consoleManager = TypeByName("BepInEx.ConsoleManager");
        setConsoleColor = Method(consoleManager, "SetConsoleColor", new[] { typeof(ConsoleColor) });
        textWriter = (TextWriter)Property(consoleManager, "ConsoleStream")?.GetMethod.Invoke(null, null)!;

        // LogEventArgs.ToString();
        harmony.Patch(
            Method(typeof(LogEventArgs), nameof(LogEventArgs.ToString)),
            prefix: new HarmonyMethod(Method(typeof(BepInExLogFix), nameof(LogEventArgsPrefix))));

        // ConsoleLogListener.LogEvent();
        harmony.Patch(
            Method(typeof(ConsoleLogListener), nameof(ConsoleLogListener.LogEvent)),
            transpiler: new HarmonyMethod(Method(typeof(BepInExLogFix), nameof(ConsoleOverrideTranspiler))));

        // DiskLogListener.LogEvent();
        harmony.Patch(
            Method(typeof(DiskLogListener), nameof(DiskLogListener.LogEvent)),
            prefix: new HarmonyMethod(typeof(BepInExLogFix), nameof(Prefix)));
    }

    private static string GetTemplate(LogLevel level) => level switch
    {
        LogLevel.Info => Templates["Info"],
        LogLevel.Message => Templates["Info"],
        LogLevel.Debug => Templates["Debug"],
        LogLevel.Warning => Templates["Warn"],
        LogLevel.Error => Templates["Error"],
        LogLevel.Fatal => Templates["Error"],
        _ => "{2}",
    };

    // Overrides the console's 'if(Level == 0) return;' check.
    [HarmonyTranspiler]
    [HarmonyWrapSafe]
    private static IEnumerable<CodeInstruction> ConsoleOverrideTranspiler(ILGenerator generator)
    {
        Label label = generator.DefineLabel();
        List<CodeInstruction> newInstructions = new()
        {
            new (OpCodes.Ldarg_0),
            new (OpCodes.Callvirt, PropertyGetter(typeof(ConsoleLogListener), "WriteUnityLogs")),
            new(OpCodes.Brtrue_S, label),

            new(OpCodes.Ldarg_1),
            new(OpCodes.Isinst, typeof(UnityLogSource)),
            new(OpCodes.Brfalse_S, label),

            new(OpCodes.Ret),

            // new CodeInstruction(OpCodes.Ret).WithLabels(label),
            new CodeInstruction(OpCodes.Ldarg_0).WithLabels(label),
            new (OpCodes.Ldarg_1),
            new (OpCodes.Ldarg_2),
            new (OpCodes.Callvirt, Method(typeof(BepInExLogFix), nameof(ConsoleOutputLogic))),
            new (OpCodes.Ret),
        };

        for (int i = 0; i < newInstructions.Count; i++)
            yield return newInstructions[i];
    }

    // ReSharper disable once RedundantAssignment
    [HarmonyPrefix]
    private static bool LogEventArgsPrefix(LogEventArgs __instance, ref string __result)
    {
        __result = __instance.Level == 0 ? __instance.Data.ToString() : GetTemplate(__instance.Level)
            .Insert(26, __instance.Data.ToString())
            .Insert(20, __instance.Source.SourceName)
            .Insert(0, Log.GetDateString());
        return false;
    }

    private static void ConsoleOutputLogic(ConsoleLogListener __instance, object sender, LogEventArgs eventArgs)
    {
        try
        {
            // BepInEx.ConsoleManager.SetConsoleColor(Log.ColorCodes["r"]);
            setConsoleColor.Invoke(null, new object[] { Log.ColorCodes["r"] });

            string text = eventArgs.ToString();

            StringBuilder stringBuilder = new();
            ConsoleColor oldColor = ConsoleColor.Gray;
            bool previouslyEscaped = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\\')
                {
                    stringBuilder.Append(c);
                    previouslyEscaped = true;
                    continue;
                }

                if (c != '&' || previouslyEscaped)
                {
                    previouslyEscaped = false;
                    stringBuilder.Append(c);
                    continue;
                }

                StringBuilder searchColor = new();
                for (int j = 0; j < Log.LongestColor + 1; j++)
                {
                    searchColor.Append(text[i + j]);
                }

                if (!Log.TryGetColor(searchColor.ToString(), out ConsoleColor? color, out int removeLength) || color is null)
                {
                    stringBuilder.Append(c);
                    continue;
                }

                // Output prior text.
                setConsoleColor.Invoke(null, new object[] { oldColor });
                textWriter.Write(stringBuilder.ToString());
                oldColor = color.Value;

                // Change Color and reset for next string.
                stringBuilder = new StringBuilder();

                // Skip this many characters as they are all coloring.
                i += removeLength;
            }

            if (stringBuilder.Length > 0)
            {
                setConsoleColor.Invoke(null, new object[] { oldColor });
                textWriter.Write(stringBuilder.ToString());
            }

            textWriter.Write('\n');

            // revert default color.
            // instance!.Write((string)eventArgs.Data);
            setConsoleColor.Invoke(null, new object[] { Log.ColorCodes["r"] });
        }
        catch (Exception)
        {
            // unused.
        }
    }

    [HarmonyPrefix]
    private static bool Prefix(DiskLogListener __instance, object sender, LogEventArgs eventArgs)
    {
        string text = eventArgs.ToString();
        bool flag = false;

        // ReSharper disable once ForCanBeConvertedToForeach
        // In case we want to check for a \& in the future.
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '&')
            {
                flag = true;
                continue;
            }

            if (flag)
            {
                flag = false;
                continue;
            }

            __instance.LogWriter.Write(text[i]);
        }

        __instance.LogWriter.Write('\n');
        return false;
    }
}