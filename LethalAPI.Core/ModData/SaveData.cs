// -----------------------------------------------------------------------
// <copyright file="SaveData.cs" company="LethalAPI Modding Community">
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
public sealed class SaveData
{
     /// <summary>
     /// Gets or sets the name of this SaveData, by default it should be your mod name or better GUID to prevent duplication.
     /// </summary>
    public string SaveName { get; set; }

    /// <summary>
    /// Gets full path where the save data of this instance is located at.
    /// </summary>
    public string SavePath { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveData"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    public SaveData(string name)
    {
        SaveName = name;
        SavePath = Path.Combine(UnityEngine.Application.persistentDataPath, name);
        CompressionType = ModDataCompression.GZip;
        EncryptionPassphrase = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveData"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    /// <param name="path">temp2</param>
    public SaveData(string name, string path)
    {
        SaveName = name;
        SavePath = path;
        CompressionType = ModDataCompression.GZip;
        EncryptionPassphrase = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveData"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    /// <param name="path">temp2</param>
    /// <param name="compressionType">temp3</param>
    public SaveData(string name, string path, ModDataCompression compressionType)
    {
        SaveName = name;
        SavePath = path;
        CompressionType = compressionType;
        EncryptionPassphrase = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveData"/> class.<br/>
    /// </summary>
    /// <param name="name">temp</param>
    /// <param name="path">temp2</param>
    /// <param name="compressionType">temp3</param>
    /// <param name="encryptionPassphrase">temp4</param>
    public SaveData(string name, string path, ModDataCompression compressionType, string encryptionPassphrase)
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
