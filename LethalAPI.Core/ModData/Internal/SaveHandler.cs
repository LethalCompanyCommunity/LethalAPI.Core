// -----------------------------------------------------------------------
// <copyright file="SaveType.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using LethalAPI.Core.Interfaces;

/// <summary>
/// Contains abstractions for handling save properties.
/// </summary>
public abstract class SaveHandler
{
    /// <summary>
    /// The save instance to use.
    /// </summary>
    internal abstract ISave Save { get; set; }
}