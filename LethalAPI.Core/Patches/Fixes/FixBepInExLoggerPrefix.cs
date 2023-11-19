// -----------------------------------------------------------------------
// <copyright file="FixBepInExLoggerPrefix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Fixes;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

using BepInEx.Logging;

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
internal static class FixBepInExLoggerPrefix
{
    /// <summary>
    /// Contains a list of colorcodes that can be used.
    /// </summary>
    /// <example>
    /// <code>
    /// Use &amp; in front of a code for the color. IE: &amp;1 red text &amp;2 green text.
    /// &amp;0 - Black
    /// &amp;1 - Red
    /// &amp;2 - Green
    /// &amp;3 - Yellow
    /// &amp;4 - Blue
    /// &amp;5 - Magenta
    /// &amp;6 - Cyan
    /// &amp;7 - White
    /// &amp;a - Dark Gray
    /// &amp;b - Dark Red
    /// &amp;c - Dark Green
    /// &amp;d - Dark Yellow
    /// &amp;e - Dark Blue
    /// &amp;f - Dark Magenta
    /// &amp;g - Dark Cyan
    /// &amp;h - Gray
    /// </code>
    /// </example>
    internal static readonly ReadOnlyDictionary<char, ConsoleColor> ConsoleText = new (new Dictionary<char, ConsoleColor>()
    {
        { '0', ConsoleColor.Black }, // black
        { '1', ConsoleColor.Red }, // red
        { '2', ConsoleColor.Green }, // green
        { '3', ConsoleColor.Yellow }, // yellow
        { '4', ConsoleColor.Blue }, // blue
        { '5', ConsoleColor.Magenta }, // purple
        { '6', ConsoleColor.Cyan }, // cyan
        { '7', ConsoleColor.White }, // white
        { 'a', ConsoleColor.DarkGray }, // dark gray
        { 'b', ConsoleColor.DarkRed }, // dark red
        { 'c', ConsoleColor.DarkGreen }, // dark green
        { 'd', ConsoleColor.DarkYellow }, // dark yellow
        { 'e', ConsoleColor.DarkBlue }, // dark blue
        { 'f', ConsoleColor.DarkMagenta }, // dark magenta
        { 'g', ConsoleColor.DarkCyan }, // dark cyan
        { 'h', ConsoleColor.Gray }, // gray
    });

    /// <summary>
    /// Enables this patch.
    /// </summary>
    /// <param name="harmony">The harmony instance to enable it on.</param>
    internal static void Patch(Harmony harmony)
        => harmony.Patch(
            AccessTools.Method(typeof(ConsoleLogListener), nameof(ConsoleLogListener.LogEvent)),
            new HarmonyMethod(AccessTools.Method(typeof(FixBepInExLoggerPrefix), nameof(Prefix))));

    [HarmonyPrefix]
    private static bool Prefix(ConsoleLogListener __instance, object sender, LogEventArgs eventArgs)
    {
        // Redirect BepInEx logs
        if ((int)eventArgs.Level != 62)
        {
            Log.Skip(eventArgs.Data.ToString(), eventArgs.Level);
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
        bool flag = false;
        string text = (string)eventArgs.Data;

        // Process the characters
        // ReSharper disable once ForCanBeConvertedToForeach
        // In case we want to ensure it isn't \& in the future, we will need a for loop.
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '&' && !flag)
            {
                flag = true;
            }
            else if (flag)
            {
                if (!ConsoleText.ContainsKey(text[i]))
                {
                    flag = false;
                    continue;
                }

                setConsoleColor.Invoke(null, new object[] { ConsoleText[text[i]] });
                flag = false;
            }
            else
            {
                instance.Write(text[i]);
            }
        }

        instance.Write('\n');

        // revert default color.
        // instance!.Write((string)eventArgs.Data);
        setConsoleColor.Invoke(null, new object[] { ConsoleColor.Gray });
        return false;
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