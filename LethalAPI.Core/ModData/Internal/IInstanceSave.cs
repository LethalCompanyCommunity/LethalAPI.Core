// -----------------------------------------------------------------------
// <copyright file="IInstanceSave.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using Interfaces;

/// <summary>
/// Contains features for updating and tracking specific features.
/// </summary>
public interface IInstanceSave
{
    /// <summary>
    /// Gets or sets the save instance to use.
    /// </summary>
    public abstract ISave Save { get; set; }

    /// <summary>
    /// Called when a save is updated.
    /// </summary>
    public void OnSaveUpdated();
}