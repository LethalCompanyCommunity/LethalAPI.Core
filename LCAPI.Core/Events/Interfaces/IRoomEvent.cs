// -----------------------------------------------------------------------
// <copyright file="IRoomEvent.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments, and potential removed properties.
// -----------------------------------------------------------------------

namespace LCAPI.Core.Events.Interfaces;

using UnityEngine;

/// <summary>
///     Event args used for all Room related events.
/// </summary>
public interface IRoomEvent : ILcApiEvent
{
    /// <summary>
    /// Gets the Player.
    /// </summary>
    public GameObject Room { get; }
}