// -----------------------------------------------------------------------
// <copyright file="FixBepInExLoggerPrefix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Fixes;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;

using BepInEx.Logging;
using Core.Events.Attributes;

#pragma warning disable SA1402
#pragma warning disable SA1313

/// <summary>
/// Patches the <see cref="BepInEx.Logging.ConsoleLogListener.LogEvent">BepInEx.Logging.ConsoleLogListener.LogEvent</see> method to utilize a custom logger.
/// </summary>
// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedParameter.Local
// This now is called manually in the plugin loader.
// [HarmonyPatch(typeof(ConsoleLogListener), nameof(ConsoleLogListener.LogEvent))]
[IgnorePatch]
internal static class FixBepInExLoggerPrefix
{
    /// <inheritdoc cref ="Core.Log.ColorCodes" />
    private static ReadOnlyDictionary<string, ConsoleColor> ConsoleText => Log.ColorCodes;

    /// <summary>
    /// Enables this patch.
    /// </summary>
    /// <param name="harmony">The harmony instance to enable it on.</param>
    internal static void Patch(HarmonyLib.Harmony harmony)
        => harmony.Patch(
            AccessTools.Method(typeof(ConsoleLogListener), nameof(ConsoleLogListener.LogEvent)),
            new HarmonyMethod(AccessTools.Method(typeof(FixBepInExLoggerPrefix), nameof(Prefix))));

    [HarmonyPrefix]
    private static bool Prefix(ConsoleLogListener __instance, object sender, LogEventArgs eventArgs)
    {
        // Redirect BepInEx logs
        if ((int)eventArgs.Level != 62)
        {
            Skip(eventArgs.Data.ToString(), eventArgs.Level);
            return false;
        }

        // Get variables and default output.
        Type consoleManager = AccessTools.TypeByName("BepInEx.ConsoleManager");
        MethodInfo setConsoleColor = AccessTools.Method(consoleManager, "SetConsoleColor", new[] { typeof(ConsoleColor) });
        TextWriter instance = (TextWriter)AccessTools.Property(consoleManager, "ConsoleStream")?.GetMethod.Invoke(null, null)!;

        /* BepInEx.ConsoleManager.SetConsoleColor(eventArgs.Level.GetConsoleColor());
           BepInEx.ConsoleStream?.Write((string)eventArgs.Data);
           BepInEx.SetConsoleColor(ConsoleColor.Gray);*/

        // Set default color
        setConsoleColor.Invoke(null, new object[] { ConsoleColor.Gray });

        string text = (string)eventArgs.Data;

        StringBuilder stringBuilder = new();
        ConsoleColor oldColor = ConsoleColor.Gray;
        bool previouslyEscaped = false;
        for (int i = 1; i < text.Length; i++)
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
            instance.Write(stringBuilder.ToString());
            oldColor = color.Value;

            // Change Color and reset for next string.
            stringBuilder = new StringBuilder();

            // Skip this many characters as they are all coloring.
            i += removeLength;
        }

        if (stringBuilder.Length > 0)
        {
            setConsoleColor.Invoke(null, new object[] { oldColor });
            instance.Write(stringBuilder.ToString());
        }

        instance.Write('\n');

        // revert default color.
        // instance!.Write((string)eventArgs.Data);
        setConsoleColor.Invoke(null, new object[] { ConsoleColor.Gray });
        return false;
    }

    /// <summary>
    /// Used to rewrite a BepInEx log into the new method of logging, but skip several stack trace iterations.
    /// </summary>
    /// <param name="message">The previous message.</param>
    /// <param name="level">The log level.</param>
    private static void Skip(string message, LogLevel level)
    {
        string callingPlugin = Log.GetCallingPlugin(Log.GetCallingMethod(6), string.Empty, Log.ShowCallingMethod);
        string template = level switch
        {
            LogLevel.Info => "Info",
            LogLevel.Message => "Info",
            LogLevel.Debug => "Debug",
            LogLevel.Warning => "Warn",
            LogLevel.Error => "Error",
            LogLevel.Fatal => "Error",
            _ => "Info",
        };

        Log.Raw(Log.Templates[template].Replace("{time}", Log.GetDateString()).Replace("{prefix}", callingPlugin).Replace("{msg}", message).Replace("{type}", template));
    }
}

/// <summary>
/// Patches the <see cref="BepInEx.Logging.DiskLogListener.LogEvent">BepInEx.Logging.DiskLogListener.LogEvent</see> with a custom implementation.
/// </summary>
[HarmonyPatch(typeof(DiskLogListener), nameof(DiskLogListener.LogEvent))]
internal static class PatchLogFilePrefix
{
    [HarmonyPrefix]
    private static bool Prefix(DiskLogListener __instance, object sender, LogEventArgs eventArgs)
    {
        // Skip default implementation. The other prefix will re-route it through the custom implementation anyways.
        if ((int)eventArgs.Level != 62)
        {
            return false;
        }

        string text = (string)eventArgs.Data;
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