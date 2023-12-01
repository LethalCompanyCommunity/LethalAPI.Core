// -----------------------------------------------------------------------
// <copyright file="SaveDataAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Attributes;

using System;

/// <summary>
/// An attribute that specify whether a given field is used to store/retrieve save data.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class SaveDataAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether this are globally used/modified for all player or just for a single player.
    /// </summary>
    public bool IsGlobal { get; init; }

    /// <summary>
    /// Gets a value indicating whether this will be loaded right after the game start up finished.
    /// </summary>
    public bool LoadOnStartUp { get; init; }

    /// <summary>
    /// Gets a value indicating whether this will be automatically loaded when the game begin to load it data.
    /// </summary>
    public bool AutomaticallyLoad { get; init; }

    /// <summary>
    /// Gets a value indicating whether this will be automatically saved when the game begin to save it data.
    /// </summary>
    public bool AutomaticallySave { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveDataAttribute"/> class.
    /// </summary>
    /// <param name="loadOnStartup">Whether to load right after the game start up finished, if set to true <paramref name="autoLoad"/> and <paramref name="autoSave"/> will be set to true</param>
    /// <param name="global">Indicate whether this are globally used/modified for all player or just for a single player.</param>
    /// <param name="autoLoad">Indicate whether this will be automatically loaded when the game begin to load it data.</param>
    /// <param name="autoSave">Indicate whether this will be automatically saved when the game begin to save it data.</param>
    public SaveDataAttribute(bool loadOnStartup = false, bool global = false, bool autoLoad = false, bool autoSave = false)
    {
        LoadOnStartUp = loadOnStartup;
        IsGlobal = global;
        AutomaticallyLoad = loadOnStartup || autoLoad;
        AutomaticallySave = loadOnStartup || autoSave;
    }
}