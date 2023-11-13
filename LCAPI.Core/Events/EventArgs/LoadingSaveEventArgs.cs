// -----------------------------------------------------------------------
// <copyright file="LoadingSaveEventArgs.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Events.EventArgs;

/// <summary>
///     Represents the event args that are called when loading a save.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class LoadingSaveEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadingSaveEventArgs"/> class.
    /// </summary>
    /// <param name="saveSlot">The slot being loaded.</param>
    public LoadingSaveEventArgs(byte saveSlot)
    {
        this.SaveSlot = saveSlot;
    }

    /// <summary>
    /// Gets the slot of the save being loaded from.
    /// </summary>
    public byte SaveSlot { get; }
}