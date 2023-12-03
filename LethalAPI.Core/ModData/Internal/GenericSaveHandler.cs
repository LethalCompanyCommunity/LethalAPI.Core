// -----------------------------------------------------------------------
// <copyright file="GenericSaveHandler.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

/// <summary>
/// Contains the implementation for holding a collection of general save data.
/// </summary>
public class GenericSaveHandler : SaveHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GenericSaveHandler"/> class.
    /// </summary>
    /// <param name="searchGlobal">Whether the save handler is representing a global save or a local save.</param>
    internal GenericSaveHandler(bool searchGlobal = false)
    {
        this.IsGlobalSave = searchGlobal;
        this.DataCollection = new();
    }
}