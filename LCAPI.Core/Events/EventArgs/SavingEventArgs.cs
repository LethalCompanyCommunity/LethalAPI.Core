// -----------------------------------------------------------------------
// <copyright file="SavingEventArgs.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Events.EventArgs;

using Interfaces;

/// <summary>
///     Represents the event args that are called when saving.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SavingEventArgs : ILcApiEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SavingEventArgs"/> class.
    /// </summary>
    /// <param name="saveSlot">The slot being saved.</param>
    public SavingEventArgs(byte saveSlot)
    {
        this.SaveSlot = saveSlot;
    }

    /// <summary>
    /// Gets the slot of the save being saved to.
    /// </summary>
    public byte SaveSlot { get; }
}