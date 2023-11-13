// -----------------------------------------------------------------------
// <copyright file="Player.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Events.Handlers;

using EventArgs;
using EventArgs.Player;
using Features;

public static class Player
{
    /// <summary>
    ///     Gets or sets the event that is invoked when a save is initiated.
    /// </summary>
    public static Event<UsingKeyEventArgs> UsingKey { get; set; } = new ();

    /// <summary>
    ///     Gets or sets the event that is invoked when a save is initiated.
    /// </summary>
    public static Event<UsingItemEventArgs> UsingItem { get; set; } = new ();

    /// <summary>
    /// Called when a key is used.
    /// </summary>
    /// <param name="ev">
    ///     The <see cref="UsingKey"/> instance.
    /// </param>
    public static void OnUsingKey(UsingKeyEventArgs ev) => UsingKey.InvokeSafely(ev);
    public static void OnUsingItem(UsingItemEventArgs ev) => UsingItem.InvokeSafely(ev);
}