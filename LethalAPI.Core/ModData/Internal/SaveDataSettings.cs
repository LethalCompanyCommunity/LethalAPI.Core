// -----------------------------------------------------------------------
// <copyright file="SaveDataSettings.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

/// <summary>
/// Contains settings which indicate how to handle saving and loading data.
/// </summary>
public sealed class SaveDataSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveDataSettings"/> class.
    /// </summary>
    /// <param name="autoLoad">Indicates whether or not to auto-load data.</param>
    /// <param name="autoSave">Indicates whether or not to auto-save data.</param>
    public SaveDataSettings(bool autoLoad, bool autoSave)
    {
        this.AutoLoad = autoLoad;
        this.AutoSave = autoSave;
    }

    /// <summary>
    /// Gets a value indicating whether indicate whether this will be automatically loaded when the game begin to load it data.
    /// </summary>
    public bool AutoLoad { get; init; }

    /// <summary>
    /// Gets a value indicating whether indicate whether this will be automatically saved when the game begin to save it data.
    /// </summary>
    public bool AutoSave { get; init; }
}