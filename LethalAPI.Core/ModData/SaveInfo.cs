// -----------------------------------------------------------------------
// <copyright file="SaveData.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

using BepInEx.Bootstrap;
using Interfaces;
using Internal;

/// <summary>
/// A class containing information about save data for a plugin.
/// </summary>
public sealed class SaveInfo
{
    /// <summary>
    /// Gets the path to the save data where all modded saves will be saved to.
    /// </summary>
    public static string GlobalSaveDirectory => UnityEngine.Application.persistentDataPath;

    /// <summary>
    /// Gets the plugin being represented.
    /// </summary>
    public IPlugin<IConfig> Plugin { get; init; } = null!;

    /// <summary>
    /// Gets the save handler.
    /// </summary>
    public SaveHandler Save { get; init; } = null!;

    /// <summary>
    /// Gets the Com Name of the plugin.
    /// </summary>
    public string SaveName => $"{this.Plugin.Author}.{this.Plugin.Name}";
/*
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveInfo"/> class.<br/>
    /// </summary>
    /// <param name="name">temp.</param>
    public SaveInfo(string name)
    {
        SaveName = name;
        SavePath = Path.Combine(UnityEngine.Application.persistentDataPath, name);
        CompressionType = ModDataCompression.GZip;
        EncryptionPassphrase = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveInfo"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    /// <param name="path">temp2</param>
    public SaveInfo(string name, string path)
    {
        SaveName = name;
        SavePath = path;
        CompressionType = ModDataCompression.GZip;
        EncryptionPassphrase = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveInfo"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    /// <param name="path">temp2</param>
    /// <param name="compressionType">temp3</param>
    public SaveInfo(string name, string path, ModDataCompression compressionType)
    {
        SaveName = name;
        SavePath = path;
        CompressionType = compressionType;
        EncryptionPassphrase = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveInfo"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    /// <param name="path">temp2</param>
    /// <param name="compressionType">temp3</param>
    /// <param name="encryptionPassphrase">temp4</param>
    public SaveInfo(string name, string path, ModDataCompression compressionType, string encryptionPassphrase)
    {
        SaveName = name;
        SavePath = path;
        CompressionType = compressionType;
        EncryptionPassphrase = encryptionPassphrase;
    }

    internal static void CatchAttribute(Type[] types)
    {
        // todo: kill me
    }

    internal static void PopulateModData()
    {
        // we don't need uninstantiated plugin.
        var pluginList = Chainloader.PluginInfos.Values
                                                .Where(plugin => plugin.Instance != null)
                                                .Select(plugin => plugin.Instance);
        foreach (var plugin in pluginList)
        {
            Assembly asm = Assembly.GetAssembly(plugin.GetType());
            var types = asm.GetTypes();
            CatchAttribute(types);
        }
    }
    */
}
