// -----------------------------------------------------------------------
// <copyright file="APIPluginInfo.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Models;

using System;

/// <summary>
///     Contains information about a plugin for use throughout the API.
/// </summary>
/// <param name="Guid"> The GUID of the plugin. </param>
/// <param name="PluginVersion"> The version of the plugin. </param>
/// <param name="Required"> Whether the plugin is required for all players. </param>
[Serializable]
public record APIPluginInfo(string Guid, Version PluginVersion, bool Required)
{
    /// <summary>
    ///     Gets the GUID of the plugin.
    /// </summary>
    public string Guid { get; } = Guid;

    /// <summary>
    ///     Gets the version of the plugin.
    /// </summary>
    public Version PluginVersion { get; } = PluginVersion;

    /// <summary>
    ///     Gets a value indicating whether the plugin is required for all players.
    /// </summary>
    public bool Required { get; } = Required;
}