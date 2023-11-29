// -----------------------------------------------------------------------
// <copyright file="LoadingSaveEventArgs.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace LethalAPI.Core.Events.EventArgs.Server;

using System;

using Interfaces;

#pragma warning disable SA1201

/// <summary>
///     Represents the event args that are called when loading a save.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class LoadingSaveEventArgs : ILethalApiEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadingSaveEventArgs"/> class.
    /// </summary>
    /// <param name="saveSlot">The slot being loaded.</param>
    /// <param name="loadedItem">The item or items being loaded..</param>
    public LoadingSaveEventArgs(string saveSlot, LoadedItem loadedItem)
    {
        this.SaveSlot = saveSlot;
        this.LoadedItem = loadedItem;
    }

    /// <summary>
    /// Gets the slot of the save being loaded from.
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

    /// <summary>
    /// Gets the item or items that are being loaded.
    /// </summary>
    public LoadedItem LoadedItem { get; }
}

/// <summary>
/// Items that are being loaded.
/// </summary>
[Flags]
public enum LoadedItem
{
    /// <summary>
    /// Custom Mod info is being loaded.
    /// </summary>
    Mods,

    /// <summary>
    /// Round stats are loaded. Called after StartOfRound.SetTimeAndPlanetToSavedSettings.
    /// </summary>
    SetTimeAndPlanetToSavedSettings,

    /// <summary>
    /// Grabbable Items are loaded. Called after StartOfRound.LoadShipGrabbableItems.
    /// </summary>
    LoadShipGrabbableItems,

    /// <summary>
    /// Unlockables are spawned. Called after StartOfRound.SpawnUnlockable.
    /// </summary>
    SpawnUnlockable,

    /// <summary>
    /// Unlockables are loaded. Called after StartOfRound.LoadUnlockables.
    /// </summary>
    LoadUnlockables,

    /// <summary>
    /// Last selected save is loaded. Called after GameNetworkManager.Start.
    /// </summary>
    LastSelectedSave,
}