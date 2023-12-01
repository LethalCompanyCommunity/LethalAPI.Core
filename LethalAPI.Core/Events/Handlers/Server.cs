// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events.Handlers;

using EventArgs.Server;
using Features;

/// <summary>
/// Contains "Server" event handlers.
/// </summary>
// ReSharper disable MemberCanBePrivate.Global
public static class Server
{
    /// <summary>
    ///     Gets or sets the event that is invoked when a save is initiated.
    /// </summary>
    public static Event<SavingEventArgs> Saving { get; set; } = new ();

    /// <summary>
    ///     Gets or sets the event that is invoked when a save is initiated.
    /// </summary>
    public static Event<LoadingSaveEventArgs> LoadingSave { get; set; } = new ();

    /// <summary>
    ///     Gets or sets the event that is invoked when the game gets to the very first menu.
    /// </summary>
    public static Event GameOpened { get; set; } = new();
}