// -----------------------------------------------------------------------
// <copyright file="StartScreenEventArgs.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace LethalAPI.Core.Events.EventArgs.Server;

using Interfaces;

#pragma warning disable SA1201

/// <summary>
///     Represents the event args that are called when loading a save.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class StartScreenEventArgs : ILethalApiEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartScreenEventArgs"/> class.
    /// </summary>
    public StartScreenEventArgs()
    {
    }
}