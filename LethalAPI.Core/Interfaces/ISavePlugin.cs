// -----------------------------------------------------------------------
// <copyright file="ISavePlugin.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Interfaces;

using ModData.Attributes;

/// <summary>
/// A wrapper for <see cref="IPlugin{TConfig}">plugins</see> to contain a <see cref="ISave"/> property to load and save game data to.
/// </summary>
/// <typeparam name="TSave">The class that inherits <see cref="ISave"/> and holds the save data.</typeparam>
public interface ISavePlugin<TSave>
    where TSave : ISave, new()
{
    /// <summary>
    /// Gets or sets the save data instance.
    /// </summary>
    public TSave SaveData { get; set; }

    /// <summary>
    /// Gets the settings for the save.
    /// </summary>
    /// <remarks>This property is get-only because the implementation does not allow for reading the value later.</remarks>
    public SaveDataAttribute SaveSettings { get; init; }
}
