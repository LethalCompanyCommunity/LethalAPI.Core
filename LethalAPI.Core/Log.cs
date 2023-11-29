// -----------------------------------------------------------------------
// <copyright file="Log.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable MemberCanBePrivate.Global
namespace LethalAPI.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Interfaces;
using Loader;
using MelonLoader;

// ReSharper Unity.PerformanceAnalysis
#pragma warning disable SA1201 // ordering by correct order.
#pragma warning disable SA1202 // ordering by access.

/// <summary>
/// The logging class for LethalAPI.
/// </summary>
public static class Log
{
    static Log()
    {
        AssemblyNameReplacements = new ConcurrentDictionary<string, string>();
        AssemblyNameReplacements.TryAdd("UnityEngine.CoreModule", "Unity");
    }

    /// <summary>
    /// Gets or sets a value indicating whether logs will show the type and method name of the method calling the logger.
    /// </summary>
    public static bool ShowCallingMethod { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether logs will show the arguments of the method if <see cref="ShowCallingMethod"/> is enabled.
    /// <seealso cref="ShowCallingMethod"/>
    /// </summary>
    public static bool ShowCallingMethodArgs { get; set; } = false;

    /// <summary>
    /// Gets a dictionary that can be used to specify assembly name replacements.
    /// <code>
    /// Example Entry:
    ///     { "UnityEngine.CoreModule", "Unity" }
    /// Original Log:
    ///     [Time] [LogLevel] [UnityEngine.CoreModule] Message
    /// Updated Log:
    ///     [Time] [LogLevel] [Unity] Message
    /// </code>
    /// </summary>
    /// <param name="originalAssemblyName">The name of the assembly to be replaced.</param>
    /// <param name="newAssemblyName">The new name of the assembly to log.</param>
    public static void AddAssemblyNameReplacement(string originalAssemblyName, string newAssemblyName)
        => AssemblyNameReplacements.TryAdd(originalAssemblyName, newAssemblyName);

    private static ConcurrentDictionary<string, string> AssemblyNameReplacements { get; }

    /// <summary>
    /// A dictionary containing different logging templates.
    /// </summary>
    /// <example>
    /// <code>
    /// {time}   - the time of the log.
    /// {type}   - the type of the log.
    /// {prefix} - the plugins prefix if applicable. Alternatively the calling method if debug info is enabled.
    /// {msg}    - the log message.
    /// {il}     - the il label of an error.
    /// {line}   - the line of an error.
    /// </code>
    /// <code>
    /// Info - the template for info logs.
    /// Debug - the template for debug logs.
    /// Warn - the template for warning logs.
    /// Error - the template for error logs.
    /// LineLocNotFound - Stack trace line name if the line is not found.
    /// LineLocFound - Stack trace line name if the line is found.
    /// </code>
    /// </example>
    public static readonly Dictionary<string, string> Templates = new()
    {
        { "Info", "{time} &r[&6{type}&r] &r[&2{prefix}&r]&r {msg}" },
        { "Debug", "{time} &r[&5{type}&r] &r[&2{prefix}&r]&r {msg}" },
        { "Warn", "{time} &r[&3{type}&r] &r[&2{prefix}&r]&r {msg}" },
        { "Error", "{time} &r[&1{type}&r] &r[&2{prefix}&r]&r {msg}" },
        { "LineLocNotFound", "&1Line Unknown &h[&6IL_{il}&h]&r" },
        { "LineLocFound", "&3Line {line} &h[&6IL_{il}&h]&r" },
    };

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
    /// &amp;r - Default Console Color.
    /// </code>
    /// </example>
    public static readonly ReadOnlyDictionary<string, ConsoleColor> ColorCodes = new(
        new Dictionary<string, ConsoleColor>()
        {
            { "0", ConsoleColor.Black }, // black
            { "1", ConsoleColor.Red }, // red
            { "2", ConsoleColor.Green }, // green
            { "3", ConsoleColor.Yellow }, // yellow
            { "4", ConsoleColor.Blue }, // blue
            { "5", ConsoleColor.Magenta }, // purple
            { "6", ConsoleColor.Cyan }, // cyan
            { "7", ConsoleColor.White }, // white
            { "a", ConsoleColor.DarkGray }, // dark gray
            { "b", ConsoleColor.DarkRed }, // dark red
            { "c", ConsoleColor.DarkGreen }, // dark green
            { "d", ConsoleColor.DarkYellow }, // dark yellow
            { "e", ConsoleColor.DarkBlue }, // dark blue
            { "f", ConsoleColor.DarkMagenta }, // dark magenta
            { "g", ConsoleColor.DarkCyan }, // dark cyan
            { "h", ConsoleColor.Gray }, // gray
            { "r", ConsoleColor.Gray }, // gray
        });

    /// <summary>
    /// Gets the longest length of text representing a color.
    /// </summary>
    public static int LongestColor => ColorCodes.Keys.OrderByDescending(x => x.Length).First().Length;

    /// <summary>
    /// Gets the formatted date string.
    /// </summary>
    /// <returns>The formatted date / time string.</returns>
    internal static string GetDateString()
    {
        if (PluginLoader.MelonLoaderFound)
            return string.Empty;
        DateTime now = DateTime.Now;
        return $"[{$"{now:g}",-19} ({$"{now:ss}",-2}.{$"{now.Millisecond:000}",-3}s)]";
    }

    /// <summary>
    /// Gets the name of the calling plugin.
    /// </summary>
    /// <param name="method">The method that is calling.</param>
    /// <param name="input">The prefix name if desired.</param>
    /// <param name="includeMethod">Include method info.</param>
    /// <returns>The name of the calling plugin.</returns>
    internal static string GetCallingPlugin(MethodBase method, string input, bool includeMethod)
    {
        try
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (method.DeclaringType?.Assembly is null)
            {
                return "Unknown";
            }

            Type type = method.DeclaringType!;

            Assembly assembly = method.DeclaringType.Assembly;
            IPlugin<IConfig>? plugin = null;
            foreach (IPlugin<IConfig> plgn in PluginLoader.Plugins.Values)
            {
                if (!PluginLoader.Locations.ContainsKey(assembly))
                    break;

                if (plgn.Assembly == assembly && plgn.Assembly.DefinedTypes.Contains(method.DeclaringType))
                {
                    plugin = plgn;
                    break;
                }
            }

            if (plugin is not null)
            {
                input = plugin.Name;
                goto skip;
            }

            // Check to see if it is a bepinex mod.
            if (PluginLoader.BepInExFound)
            {
                input = GetBepInExTypeLoadPreventor(method);
                if (input != string.Empty)
                    goto skip;
            }

            // Check to see if it is a melonmod.
            if (PluginLoader.MelonLoaderFound)
            {
                input = GetMelonLoaderTypeLoadPreventor(method);
                if (input != string.Empty)
                    goto skip;
            }

            // Use the name of the assembly.
            input = method.DeclaringType.Assembly.GetName().Name;

            // Check to see if the assembly should be called something else.
            if (AssemblyNameReplacements.ContainsKey(input) && AssemblyNameReplacements.TryGetValue(input, out string replacementName))
                input = replacementName!;

            skip:
            if (!includeMethod)
                return input;

            string args = string.Empty;

            if (ShowCallingMethodArgs)
            {
                foreach (ParameterInfo x in method.GetParameters())
                {
                    args += $"&g{x.ParameterType.Name} &r{x.Name}&r, ";
                }

                if (args != string.Empty)
                {
                    args = args.Substring(0, args.Length - 2) + "&7";
                }
            }

            input += $"&r::{type.FullName}.&6{method.Name}&r({args})";
            return input;
        }
        catch (Exception e)
        {
            Exception(e, "LethalAPI-Logger");
            return "Unknown";
        }
    }

    /// <summary>
    /// Logs information to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callingPlugin">Displays a custom message for the plugin name. This will be automatically inferred.</param>
    public static void Info(string message, string callingPlugin = "")
    {
        callingPlugin = GetCallingPlugin(GetCallingMethod(), callingPlugin, ShowCallingMethod);

        // &7[&b&6{type}&B&7] &7[&b&2{prefix}&B&7]&r
        Raw(Templates["Info"].Replace("{time}", GetDateString()).Replace("{prefix}", $"{callingPlugin,-5}")
            .Replace("{msg}", message).Replace("{type}", "Info "));
    }

    /// <summary>
    /// Logs debugging information to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="canLog">Can be used to prevent the log from being logged. Essentially an integrated if statement.
    /// <code>
    /// if(canLog)
    ///     Log.Debug();
    /// </code></param>
    /// <param name="callingPlugin">Displays a custom message for the plugin name. This will be automatically inferred.</param>
    public static void Debug(string message, bool canLog = true, string callingPlugin = "")
    {
        if (!canLog)
        {
            return;
        }

        MethodBase method = GetCallingMethod();
        if (!PluginLoader.Locations.ContainsKey(method.DeclaringType!.Assembly))
            return;

        IPlugin<IConfig>? plugin = PluginLoader.Plugins.Values.FirstOrDefault(x => x.Assembly == method.DeclaringType.Assembly && x.Assembly.DefinedTypes.Contains(method.DeclaringType));
        if (plugin is null)
            return;

        if (!plugin.Config.Debug)
            return;

        callingPlugin = GetCallingPlugin(method, callingPlugin, ShowCallingMethod);

        // &7[&b&5{type}&B&7] &7[&b&2{prefix}&B&7]&r
        Raw(Templates["Debug"].Replace("{time}", GetDateString()).Replace("{prefix}", callingPlugin)
            .Replace("{msg}", message).Replace("{type}", "Debug"));
    }

    /// <summary>
    /// Logs warning information to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callingPlugin">Displays a custom message for the plugin name. This will be automatically inferred.</param>
    public static void Warn(string message, string callingPlugin = "")
    {
        callingPlugin = GetCallingPlugin(GetCallingMethod(), callingPlugin, ShowCallingMethod);

        // &7[&b&3{type}&B&7] &7[&b&2{prefix}&B&7]&r
        Raw(Templates["Warn"].Replace("{time}", GetDateString()).Replace("{prefix}", callingPlugin)
            .Replace("{msg}", message).Replace("{type}", "Warn "));
    }

    /// <summary>
    /// Logs error information to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="callingPlugin">Displays a custom message for the plugin name. This will be automatically inferred.</param>
    public static void Error(string message, string callingPlugin = "")
    {
        callingPlugin = GetCallingPlugin(GetCallingMethod(), callingPlugin, ShowCallingMethod);

        // &7[&b&1{type}&B&7] &7[&b&2{prefix}&B&7]&r
        Raw(Templates["Error"].Replace("{time}", GetDateString()).Replace("{prefix}", callingPlugin)
            .Replace("{msg}", message).Replace("{type}", "Error"));
    }

    /// <summary>
    /// Logs an exception and any relevant information to the console.
    /// </summary>
    /// <param name="exception">The exception being logged.</param>
    /// <param name="callingPlugin">Displays a custom message for the plugin name. This will be automatically inferred.</param>
    public static void Exception(Exception exception, string callingPlugin = "")
    {
        string message = $"An error has occured. {exception.Message}. Information: \n";
        Raw(Templates["Error"].Replace("{time}", GetDateString()).Replace("{prefix}", callingPlugin)
            .Replace("{msg}", message).Replace("{type}", "Error"));
        for (Exception? e = exception; e != null; e = e.InnerException)
        {
            string msg1 = "Exception Information";
            string name = e.GetType().FullName!;
            if (name.Length > msg1.Length)
                msg1 = msg1.PadBoth(name.Length);
            else
                name = name.PadBoth(msg1.Length);
            string msg2 = $"&r[&b {msg1} &r]&a".PadBoth(100, '-');
            string name1 = $"&r[&1 {name} &r]&a".PadBoth(100);
            Raw($"&r[&a{msg2}&r]");
            Raw($" {name1} ");
            Raw("&7" + e.Message + "\n&r" + e.StackTrace);
            if (e is ReflectionTypeLoadException typeLoadException)
            {
                for (int index = 0; index < typeLoadException.Types.Length; ++index)
                    Raw("&7ReflectionTypeLoadException.Types[&3" + index + "&7]: &6" + typeLoadException.Types[index]);
                for (int index = 0; index < typeLoadException.LoaderExceptions.Length; ++index)
                {
                    Exception(typeLoadException
                        .LoaderExceptions[index]); // (tag + (tag == null ? "" : ", ") + "rtle:" + index.ToString());
                }
            }

            if (e is TypeLoadException)
                Raw("TypeLoadException.TypeName: " + ((TypeLoadException)e).TypeName);
            if (e is BadImageFormatException)
                Raw("BadImageFormatException.FileName: " + ((BadImageFormatException)e).FileName);
        }
    }

    /// <summary>
    /// Logs an exception and the respective information to the console.
    /// </summary>
    /// <param name="e">The exception to log.</param>
    /// <param name="callingPlugin">The name of the calling plugin.</param>
    public static void Error(Exception e, string callingPlugin = "")
    {
        string errorMsg = $"{e}";
        Raw(Templates["Error"].Replace("{time}", GetDateString()).Replace("{prefix}", callingPlugin)
            .Replace("{msg}", errorMsg).Replace("{type}", "Error"));
    }

    /// <summary>
    /// Logs information to the console, without adding any text to the message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
    public static void Raw(string message)
    {
    }

    /// <summary>
    /// Gets the calling plugin.
    /// </summary>
    /// <param name="skip">Skips that should be made to ignore api stuff.</param>
    /// <returns>The method that called.</returns>
    internal static MethodBase GetCallingMethod(int skip = 0)
    {
        StackTrace stack = new(2 + skip);

        return stack.GetFrame(0).GetMethod();
    }

    private static string PadBoth(this string source, int length, char padChar = ' ')
    {
        int spaces = length - source.Length;
        int padLeft = (spaces / 2) + source.Length;
        return source.PadLeft(padLeft, padChar).PadRight(length, padChar);
    }

    // Used to prevent typeload exceptions (system only checks one method deep, this checks two methods to avoid it.)
    private static string GetBepInExTypeLoadPreventor(MethodBase method) =>
        GetBepInExPluginName(method);

    private static string GetMelonLoaderTypeLoadPreventor(MethodBase method) =>
        GetMelonLoaderPluginName(method);

    private static string GetBepInExPluginName(MethodBase method)
    {
        Assembly assembly = method.DeclaringType!.Assembly;
        if (BepInEx.Bootstrap.Chainloader.PluginInfos is not null)
        {
            // FirstOrDefault keeps throwing a NullReferenceException. This doesnt throw an exception so we will use it.
            foreach (KeyValuePair<string, BepInEx.PluginInfo> modInfo in BepInEx.Bootstrap.Chainloader.PluginInfos)
            {
                if (modInfo.Value?.Instance is null)
                {
                    continue;
                }

                if (modInfo.Value.Instance.Info.Instance.GetType().Assembly != assembly)
                {
                    continue;
                }

                return modInfo.Value.Metadata.Name;
            }
        }

        return string.Empty;
    }

    private static string GetMelonLoaderPluginName(MethodBase method)
    {
        Assembly assembly = method.DeclaringType!.Assembly;
        if (MelonMod.RegisteredMelons is not null)
        {
            // FirstOrDefault keeps throwing a NullReferenceException. This doesnt throw an exception so we will use it.
            foreach (MelonMod mod in MelonMod.RegisteredMelons)
            {
                if (mod.MelonAssembly.Assembly is null)
                {
                    continue;
                }

                if (mod.MelonAssembly.Assembly != assembly)
                {
                    continue;
                }

                return mod.Info.Name;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Gets the color of a string.
    /// </summary>
    /// <param name="value">The string being checked.</param>
    /// <param name="color">The color found.</param>
    /// <param name="length">The length of text to remove for the color.</param>
    /// <param name="returnFirstMatch">Indicates whether to return after the first matched color or to keep searching for longer colors.</param>
    /// <returns>True if the color was found, false if the color is unknown.</returns>
    public static bool TryGetColor(string value, out ConsoleColor? color, out int length, bool returnFirstMatch = true)
    {
        color = null;
        length = 0;
        List<string> validExtensions = ColorCodes.Keys.ToList();
        List<string> optionsToKeep = ColorCodes.Keys.ToList();

        // Skip first check.
        if (value[0] == '&')
            value = value.Substring(1, value.Length - 1);

        for (int i = 0; i < LongestColor; i++)
        {
            // For when no matches are found but one of the options is larger than the file location.
            // ie. File 'img.png' vs '.metafile' - prevents errors.
            if (value.Length <= i)
            {
                return color == null;
            }

            // This will compare the file location to the custom file extensions, starting from the last char -> first char.
            // Using this method is more optimal as we can break from the first instance. (less total instructions in theory.)
            char currentChar = value[i];
            foreach (string option in validExtensions)
            {
                // This option does not match. This compares the chars of both indexes (which need to be the same).
                // We don't add this option to the keeper list.
                if (option[i] != currentChar)
                {
                    continue;
                }

                // This option is matched. if there are two options, we may want to keep searching to ensure there isn't a larger option that matches
                // This can be disabled if we toggle AcceptFirstMatch / goto the found match part.
                if (option.Length == i + 1)
                {
                    length = option.Length;
                    color = ColorCodes[option];
                    if (returnFirstMatch)
                        return true;

                    // Ex: .gz and .tar.gz -
                    // If we skip to the foundMatch, .tar.gz cannot be a returned value, given that .gz is already a value.
                    continue;
                }

                // This option can keep being searched. It matches so far, but hasn't been fully matched yet.
                optionsToKeep.Add(option);
            }

            // Clear results add the remaining valid options to iterate through.
            // note: if we do validOptions = optionsToKeep, this won't actually add the options, as validOptions will just point to the cleared optionsToKeep.
            validExtensions.Clear();
            validExtensions.InsertRange(0, optionsToKeep);
            optionsToKeep.Clear();

            // Because we use .First(), we need to break here in case there are no more options. Otherwise it's an enumerable exception, as First() is called before we can check.
            if (validExtensions.Count == 0)
            {
                return color == null;
            }
        }

        return color == null;
    }
}