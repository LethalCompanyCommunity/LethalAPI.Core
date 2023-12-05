// -----------------------------------------------------------------------
// <copyright file="IConfig.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Interfaces;

using System.ComponentModel;

/// <summary>
/// The main interface for a plugin.
/// </summary>
public interface IConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether the plugin should be loaded or not.
    /// </summary>
    [Description("Indicates whether or not the plugin should be loaded.")]
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not debug logs will be shown.
    /// </summary>
    [Description("Indicates whether or not the plugin should show debug logs in the console.")]
    public bool Debug { get; set; }
}