// -----------------------------------------------------------------------
// <copyright file="IItemEvent.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments, and potential removed properties.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events.Interfaces;

/// <summary>
///     Event args used for all Item related events.
/// </summary>
public interface IItemEvent : ILethalApiEvent
{
    /// <summary>
    /// Gets the item.
    /// </summary>
    public GrabbableObject GrabbableItem { get; }
}