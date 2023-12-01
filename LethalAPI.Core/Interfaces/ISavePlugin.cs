// -----------------------------------------------------------------------
// <copyright file="ISaveData.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Interfaces;

/// <summary>
/// A wrapper for inherited plugins to contain a <see cref="ISave"/> property to load and save game data to.
/// </summary>
/// <typeparam name="TSave">The class that inherits <see cref="ISave"/> and holds the save data.</typeparam>
public interface ISavePlugin<TSave> : ISavePlugin
    where TSave : ISave, new()
{
    /// <summary>
    /// Gets or sets the save data instance.
    /// </summary>
    public new TSave SaveData { get; set; }
}

/// <summary>
/// Used for internal use for handling saves.
/// </summary>
public interface ISavePlugin
{
    /// <summary>
    /// Gets or sets the save data.
    /// </summary>
    internal ISave SaveData { get; set; }
}