// -----------------------------------------------------------------------
// <copyright file="SaveSlots.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Enums;

/// <summary>
/// Gets the currently selected save slot.
/// </summary>
public enum SaveSlots : byte
{
    /// <summary>
    /// The global save slot is the independent from the normal 3 save slots.
    /// </summary>
    GlobalSlot = 7,

    /// <summary>
    /// Slot 1 is the first local save slot that a player can select.
    /// </summary>
    Slot1 = 1,

    /// <summary>
    /// Slot 2 is the second local save slot that a player can select.
    /// </summary>
    Slot2 = 2,

    /// <summary>
    /// Slot 3 is the third local save slot that a player can select.
    /// </summary>
    Slot3 = 3,
}