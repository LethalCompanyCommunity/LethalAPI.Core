// -----------------------------------------------------------------------
// <copyright file="IConfig.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Interfaces;

/// <summary>
/// The main interface for a plugin.
/// </summary>
public interface IConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether the plugin should be loaded or not.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not debug logs will be shown.
    /// </summary>
    public bool Debug { get; set; }
}