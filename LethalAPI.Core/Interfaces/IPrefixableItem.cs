// -----------------------------------------------------------------------
// <copyright file="IPrefixableItem.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Interfaces;

/// <summary>
/// Adds a prefix to an item. Used for collections.
/// </summary>
public interface IPrefixableItem
{
    /// <summary>
    /// Gets the prefix of the item.
    /// </summary>
    string Prefix { get; }
}