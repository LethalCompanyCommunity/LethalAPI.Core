// -----------------------------------------------------------------------
// <copyright file="UsingItemEventArgs.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Events.EventArgs.Player;

using Interfaces;

public class UsingItemEventArgs : IDeniableEvent, IItemEvent
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="grabbableItem"></param>
    /// <param name="isAllowed"></param>
    public UsingItemEventArgs(GrabbableObject grabbableItem, bool isAllowed = true)
    {
        this.IsAllowed = isAllowed;
        this.GrabbableItem = grabbableItem;
    }

    /// <inheritdoc />
    public bool IsAllowed { get; set; }

    /// <inheritdoc />
    public GrabbableObject GrabbableItem { get; }
}