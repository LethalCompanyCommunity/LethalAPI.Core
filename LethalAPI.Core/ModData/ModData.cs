// -----------------------------------------------------------------------
// <copyright file="ModData.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using BepInEx.Bootstrap;

/// <summary>
/// The class that handle your custom mod save data.
/// </summary>
//todo: get a better name
public sealed class ModData
{
     /// <summary>
     /// Gets or sets the name of this ModData, by default it should be your mod name or better GUID to prevent duplication.
     /// </summary>
    public string SaveName { get; set; }

    /// <summary>
    /// Gets full path where the save data of this instance is located at.
    /// </summary>
    public string SavePath { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance of ModData use compression or not.
    /// </summary>
    public bool UseCompression => CompressionType != ModDataCompression.None;

    /// <summary>
    /// Gets or sets the compression type that this ModData instance will use when it compress the data.
    /// </summary>
    public ModDataCompression CompressionType { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance of ModData use encryption or not.
    /// </summary>
    public bool UseEncryption => !string.IsNullOrEmpty(EncryptionPassphrase);

    /// <summary>
    /// Gets or sets the encryption passphrase that this ModData instance will use when it encrypt the data.
    /// </summary>
    public string EncryptionPassphrase { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModData"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    public ModData(string name)
    {
        SaveName = name;
        SavePath = Path.Combine(UnityEngine.Application.persistentDataPath, name);
        CompressionType = ModDataCompression.GZip;
        EncryptionPassphrase = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModData"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    /// <param name="path">temp2</param>
    public ModData(string name, string path)
    {
        SaveName = name;
        SavePath = path;
        CompressionType = ModDataCompression.GZip;
        EncryptionPassphrase = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModData"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    /// <param name="path">temp2</param>
    /// <param name="compressionType">temp3</param>
    public ModData(string name, string path, ModDataCompression compressionType)
    {
        SaveName = name;
        SavePath = path;
        CompressionType = compressionType;
        EncryptionPassphrase = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModData"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    /// <param name="path">temp2</param>
    /// <param name="compressionType">temp3</param>
    /// <param name="encryptionPassphrase">temp4</param>
    public ModData(string name, string path, ModDataCompression compressionType, string encryptionPassphrase)
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
}
