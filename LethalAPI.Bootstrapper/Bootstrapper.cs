// -----------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using LethalAPI.Bootstrapper.MelonLoader;
using MelonLoader;

#pragma warning disable SA1201 // enum should not follow type
#pragma warning disable SA1403 // file can only contain one namespace.
#pragma warning disable SA1402 // file may only contain one type.

[assembly: MelonInfo(typeof(Bootstrapper), "LethalAPI-Bootstrap", "1.0.0", "LethalAPI Modding Community")]
[assembly: MelonGame("LethalAPI Modding Community", "Lethal Company")]

namespace LethalAPI.Bootstrapper.MelonLoader
{
    /// <summary>
    /// The bootstrapping class for MelonLoader.
    /// </summary>
    internal class Bootstrapper : MelonMod
    {
        /// <inheritdoc />
        public override void OnEarlyInitializeMelon()
        {
            Loader.LoadMethod = LoadMethod.MelonLoader;
            LogMessage("Loading Lethal API Bootstrapper [MelonLoader]");
            Loader.Load();
        }

        /// <summary>
        /// Logs a message via MelonLoader.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void LogMessage(string message)
        {
            MelonLogger.Msg(message);
        }
    }
}

namespace LethalAPI.Bootstrapper.BepInEx
{
    using global::BepInEx;
    using global::BepInEx.Logging;

    /// <summary>
    /// The bootstrapping class for BepInEx.
    /// </summary>
    [BepInPlugin("LethalAPI-Bootstrap", "LethalAPI", "1.0.0")]
    internal class Bootstrapper : BaseUnityPlugin
    {
        private static ManualLogSource logger = null!;

        /// <summary>
        /// Logs a message via BepInEx.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void LogMessage(string message)
        {
            logger.Log((LogLevel)62, message);
        }

        private void Awake()
        {
            logger = this.Logger;
            Loader.LoadMethod = LoadMethod.BepInEx;
            LogMessage("Loading Lethal API Bootstrapper [BepInEx]");
            Loader.Load();
        }
    }
}

namespace Doorstop
{
    using LethalAPI.Bootstrapper;

    /// <summary>
    /// The bootstrapping class for Doorstop.
    /// </summary>
    internal static class Entrypoint
    {
        /// <summary>
        /// The doorstop start method.
        /// </summary>
        internal static void Start()
        {
            Loader.LoadMethod = LoadMethod.Doorstop;
            UnityEngine.Debug.Log("Loading Lethal API Bootstrapper [Doorstop]");
            Loader.Load();
        }
    }
}

namespace LethalAPI.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;

    /// <summary>
    /// The bootstrap loader class.
    /// </summary>
    public static class Loader
    {
        // ReSharper disable once CollectionNeverQueried.Local
        private static readonly List<Assembly> Dependencies = new ();
        private static Assembly baseAssembly = null!;

        /// <summary>
        /// Gets the <see cref="LoadMethod"/> being used.
        /// </summary>
        public static LoadMethod LoadMethod { get; internal set; } = LoadMethod.Manual;

        /// <summary>
        /// Logs a message appropriately.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            switch (LoadMethod)
            {
                case LoadMethod.MelonLoader:
                    LogMelon(message);
                    break;

                case LoadMethod.BepInEx:
                    LogBepInEx(message);
                    break;

                case LoadMethod.Doorstop or LoadMethod.Manual:
                    UnityEngine.Debug.Log(message);
                    break;
            }
        }

        /// <summary>
        /// Loads the framework.
        /// </summary>
        internal static void Load()
        {
            CosturaUtility.Initialize();
            Log($"[LethalAPI-Bootstrapper] Loading Bootstrapper [{LoadMethod}].");
            baseAssembly = typeof(Loader).Assembly;
            LoadDependencies();
        }

        // TypeLoadExceptions only check so far ahead, so we can avoid them with the method.
        private static void LogMelon(string message) => MelonLoader.Bootstrapper.LogMessage(message);

        private static void LogBepInEx(string message) => BepInEx.Bootstrapper.LogMessage(message);

        private static HarmonyLib.Harmony Harmony => new ("com.leathalapi.bootstrapper");

        private static void LoadDependencies()
        {
            bool embeddedCoreFound = false;
            Log("[LethalAPI-Bootstrapper] Loading dependencies.");
            foreach (string resourcePath in baseAssembly.GetManifestResourceNames())
            {
                if (!LoadAssembly(resourcePath, out Assembly? resultAssembly))
                    continue;

                Log($"[LethalAPI-Bootstrapper] Loaded embedded dependency '{resultAssembly!.GetName().Name}'@v{resultAssembly.GetName().Version}.");
                try
                {
                    Dependencies.Add(resultAssembly);
                    if (resultAssembly.GetName().Name == "LethalAPI.Core")
                    {
                        embeddedCoreFound = true;
                        if (LoadMethod == LoadMethod.BepInEx)
                            Core.Loader.PluginLoader.FixLoggingBepInEx(Harmony);
                        Core.Log.LogMessage += Log;
                        Log("[LethalAPI-Bootstrapper] Log fix patched.");
                        _ = new Core.Loader.PluginLoader();
                    }
                }
                catch (Exception e)
                {
                    Log($"[LethalAPI-Bootstrapper] An error has occured while loading dependency '{resourcePath}'. Exception: \n{e}");
                }
            }

            if (embeddedCoreFound)
                return;

            try
            {
                if (LoadMethod == LoadMethod.BepInEx)
                    Core.Loader.PluginLoader.FixLoggingBepInEx(Harmony);
                Core.Log.LogMessage += Log;
                Log("[LethalAPI-Bootstrapper] Log fix patched.");
                _ = new Core.Loader.PluginLoader();
                foreach (Assembly dependency in Dependencies)
                {
                    Core.Loader.PluginLoader.Dependencies.Add(dependency);
                }
            }
            catch (Exception e)
            {
                Log($"[LethalAPI-Bootstrapper] An error has occured while loading the LethalAPI Core Plugin. Exception: \n{e}");
            }
        }

        private static bool LoadAssembly(string resourcePath, out Assembly? assembly)
        {
            try
            {
                assembly = null;
                if (!resourcePath.EndsWith(".dll.compressed") && !resourcePath.EndsWith(".dll"))
                    return false;

                MemoryStream memStream = new();
                Stream? dataStream = baseAssembly.GetManifestResourceStream(resourcePath);
                if (dataStream is null)
                    return false;

                if (resourcePath.EndsWith(".dll.compressed"))
                {
                    DeflateStream decompressionStream = new(dataStream, CompressionMode.Decompress);
                    decompressionStream.CopyTo(memStream);
                }
                else
                {
                    dataStream.CopyTo(memStream);
                }

                assembly = AppDomain.CurrentDomain.Load(memStream.ToArray());
                return true;
            }
            catch (TypeLoadException e)
            {
                Log($"Missing dependency for plugin '{resourcePath}'. Exception: {e.Message}");
            }
            catch (Exception e)
            {
                Log($"Could not load a dependency or plugin. Exception: \n{e}");
            }

            assembly = null;
            return false;
        }
    }

    /// <summary>
    /// Represents the different load methods that can be used.
    /// </summary>
    public enum LoadMethod
    {
        /// <summary>
        /// Loaded via BepInEx.
        /// </summary>
        BepInEx = 1,

        /// <summary>
        /// Loaded via MelonLoader.
        /// </summary>
        MelonLoader = 2,

        /// <summary>
        /// Loaded via Doorstop.
        /// </summary>
        Doorstop = 4,

        /// <summary>
        /// Loaded manually.
        /// </summary>
        Manual = 8,
    }
}
