// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events.Handlers;

using EventArgs.Player;
using Features;

/// <summary>
/// Contains event handlers for player events.
/// </summary>
public static class Player
{
    /// <summary>
    ///     Gets or sets the event that is invoked when a player is critically injured.
    /// </summary>
    public static Event<CriticallyInjureEventArgs> CriticallyInjure { get; set; } = new ();

    /// <summary>
    ///     Gets or sets the event that is invoked when a player is healed.
    /// </summary>
    public static Event<HealingEventArgs> Healing { get; set; } = new ();

    /// <summary>
    ///     Gets or sets the event that is invoked when a player is using a key.
    /// </summary>
    public static Event<UsingKeyEventArgs> UsingKey { get; set; } = new ();

    /// <summary>
    ///     Gets or sets the event that is invoked when a player is using an item.
    /// </summary>
    public static Event<UsingItemEventArgs> UsingItem { get; set; } = new ();
}