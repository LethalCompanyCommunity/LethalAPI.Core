// -----------------------------------------------------------------------
// <copyright file="Server.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events.Handlers;

using EventArgs;
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
    /// Called when a save is initiated.
    /// </summary>
    /// <param name="ev">
    ///     The <see cref="SavingEventArgs"/> instance.
    /// </param>
    public static void OnSaving(SavingEventArgs ev) => Saving.InvokeSafely(ev);

    /// <summary>
    /// Called when a save is initiated.
    /// </summary>
    /// <param name="ev">
    ///     The <see cref="LoadingSaveEventArgs"/> instance.
    /// </param>
    public static void OnLoadingSave(LoadingSaveEventArgs ev) => LoadingSave.InvokeSafely(ev);
}