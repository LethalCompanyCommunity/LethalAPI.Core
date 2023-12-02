// -----------------------------------------------------------------------
// <copyright file="ModDataCompression.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData;

/// <summary>
/// Enum present for supported method of compression for storing mod data on disk.<br/>
/// </summary>
public enum ModDataCompression
{
    /// <summary>
    /// Don't use any compression.
    /// </summary>
    None = 0,

    /// <summary>
    /// Compress your data using GZip specification.
    /// </summary>
    GZip = 1,

    /// <summary>
    /// Compress your data using ZLib specification.
    /// </summary>
    ZLib = 2,

    /// <summary>
    /// Compress your data using Brotli specification.
    /// </summary>
    Brotli = 3,
}
