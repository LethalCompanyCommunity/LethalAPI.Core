// -----------------------------------------------------------------------
// <copyright file="IPlugin.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Interfaces;

using System;
using System.Reflection;

/// <summary>
/// The main interface for implementing a plugin.
/// </summary>
/// <typeparam name="TConfig">The config type of the plugin.</typeparam>
public interface IPlugin<out TConfig>
    where TConfig : IConfig
{
    /// <summary>
    /// Gets the config.
    /// </summary>
    /// <remarks>If utilizing the <see cref="IConfig"/> interface, this must be = new() / not null.</remarks>
    public TConfig Config { get; }

    /// <summary>
    /// Gets the assembly that the plugin is located in.
    /// </summary>
    public Assembly Assembly { get; init; }

    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the description of the plugin.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the author of the plugin.
    /// </summary>
    public string Author { get; }

    /// <summary>
    /// Gets the Version of the plugin.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// Gets the minimum required version of the api for the plugin to load.
    /// </summary>
    public Version RequiredAPIVersion { get; }

    /// <summary>
    /// Updates a config with a new config.
    /// </summary>
    /// <param name="newConfig">The new config to use.</param>
    public void UpdateConfig(object newConfig);

    /// <summary>
    /// Occurs when the plugin is enabled.
    /// </summary>
    public void OnEnabled();

    /// <summary>
    /// Occurs when the plugin is disabled.
    /// </summary>
    public void OnDisabled();

    /// <summary>
    /// Occurs when the plugin is reloaded.
    /// </summary>
    /// <example>
    /// Reload Execution order:
    /// <code>
    /// OnDisabled(); ->
    /// OnReloaded(); ->
    /// OnEnabled();
    /// </code>
    /// </example>
    public void OnReloaded();
}