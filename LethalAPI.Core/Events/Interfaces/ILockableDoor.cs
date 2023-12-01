// -----------------------------------------------------------------------
// <copyright file="ILockableDoor.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Events.Interfaces;

/// <summary>
///     Event args used for all door lock related events.
/// </summary>
public interface ILockableDoor : IDoorEvent
{
    /// <summary>
    /// Gets the <see cref="DoorLock"/>.
    /// </summary>
    public DoorLock Lock { get; }
}