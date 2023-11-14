// -----------------------------------------------------------------------
// <copyright file="HealingEventArgs.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Events.EventArgs.Player;

using GameNetcodeStuff;
using Interfaces;

/// <summary>
/// Contains the arguments for the <see cref="Handlers.Player.Healing"/> event.
/// </summary>
public class HealingEventArgs : IDeniableEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HealingEventArgs"/> class.
    /// </summary>
    /// <param name="player">The player who is healing.</param>
    /// <param name="isAllowed">Indicates whether the event is allowed to execute.</param>
    public HealingEventArgs(PlayerControllerB player, bool isAllowed = true)
    {
        this.Player = player;
        this.IsAllowed = isAllowed;
    }

    /// <summary>
    /// The player who is being healed.
    /// </summary>
    public PlayerControllerB Player { get; }

    /// <inheritdoc />
    public bool IsAllowed { get; set; }
}