// -----------------------------------------------------------------------
// <copyright file="SaveDataAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Attributes;

using System;

using ModData;

/// <summary>
/// An attribute that specify whether a given field is used to store/retrieve save data.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class SaveDataAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveDataAttribute"/> class.
    /// </summary>
    /// <param name="saveName">The custom save name, if this parameter is empty, default will be taken.</param>
    /// <param name="compressionType">The compression type that will be used when storing and loading your data.</param>
    /// <param name="encryptionPassphrase">The encryption passphrase that will be used when storing and loading data.</param>
    /// <param name="autoLoad">Indicate whether this will be automatically loaded when the game begin to load it data.</param>
    /// <param name="autoSave">Indicate whether this will be automatically saved when the game begin to save it data.</param>
    public SaveDataAttribute(bool autoLoad = true, bool autoSave = true, string saveName = "", ModDataCompression compressionType = ModDataCompression.None, string encryptionPassphrase = "")
    {
        this.SaveName = saveName;
        this.CompressionType = compressionType;
        this.AutoLoad = autoLoad;
        this.AutoSave = autoSave;
        this.EncryptionPassphrase = encryptionPassphrase;
    }

    /// <summary>
    /// Gets the custom save name, if this parameter is empty, default will be taken.
    /// </summary>
    public string SaveName { get; init; }

    /// <summary>
    /// Gets the compression type that will be used when storing and loading your data.
    /// </summary>
    public ModDataCompression CompressionType { get; }

    /// <summary>
    /// Gets the encryption passphrase that will be used when storing and loading data.
    /// </summary>
    public string EncryptionPassphrase { get; init; }

    /// <summary>
    /// Gets a value indicating whether indicate whether this will be automatically loaded when the game begin to load it data.
    /// </summary>
    public bool AutoLoad { get; init; }

    /// <summary>
    /// Gets a value indicating whether indicate whether this will be automatically saved when the game begin to save it data.
    /// </summary>
    public bool AutoSave { get; init; }
}