// -----------------------------------------------------------------------
// <copyright file="ILockableDoor.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LCAPI.Core.Events.Interfaces;

using DunGen;

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