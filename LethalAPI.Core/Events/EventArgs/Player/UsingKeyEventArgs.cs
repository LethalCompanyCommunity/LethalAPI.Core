// -----------------------------------------------------------------------
// <copyright file="UsingKeyEventArgs.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events.EventArgs.Player;

using DunGen;
using Interfaces;
using UnityEngine;

/// <summary>
/// Contains the arguments for the <see cref="Handlers.Player.UsingKey"/> event.
/// </summary>
public class UsingKeyEventArgs : IDeniableEvent, IItemEvent, ILockableDoor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingKeyEventArgs"/> class.
    /// </summary>
    /// <param name="target">The target door.</param>
    /// <param name="item">The key item being used.</param>
    /// <param name="isAllowed">Is the event allowed to occur.</param>
    public UsingKeyEventArgs(GameObject target, GrabbableObject item,  bool isAllowed = true)
    {
        this.GrabbableItem = item;
        this.Key = (item as KeyItem)!;
        this.Lock = target.transform.GetComponent<DoorLock>();
        this.Door = target.transform.GetComponent<Door>();
        this.IsAllowed = isAllowed;
    }

    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    /// <inheritdoc />
    public Door Door { get; }

    /// <inheritdoc />
    public DoorLock Lock { get; }

    /// <inheritdoc />
    public GrabbableObject GrabbableItem { get; }

    /// <summary>
    /// Gets the key being used.
    /// </summary>
    public GrabbableObject Key { get; }
}