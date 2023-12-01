// -----------------------------------------------------------------------
// <copyright file="ResetSaveEventArgs.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events.EventArgs.Server;

using Interfaces;

/// <summary>
///     Represents the event args that are called when saving.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ResetSaveEventArgs : IDeniableEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetSaveEventArgs"/> class.
    /// </summary>
    /// <param name="saveSlot">The slot being reset.</param>
    /// <param name="isAllowed">Is the event allowed to occur.</param>
    public ResetSaveEventArgs(string saveSlot, bool isAllowed = true)
    {
        this.SaveSlot = saveSlot;
        this.IsAllowed = isAllowed;
    }

    /// <summary>
    /// Gets the slot of the save being saved to.
    /// </summary>
    /// <code>
    /// Currently Supports Save Slots:
    ///     LCGeneralSaveData - Global save slot.
    ///     LCSaveFile1 - Save slot 1.
    ///     LCSaveFile2 - Save slot 2.
    ///     LCSaveFile3 - Save slot 3.
    /// </code>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string SaveSlot { get; }

    /// <inheritdoc />
    public bool IsAllowed { get; set; }
}