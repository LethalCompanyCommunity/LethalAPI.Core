// -----------------------------------------------------------------------
// <copyright file="DefaultConfig.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs;

using Interfaces;

/// <summary>
/// Gets the default config.
/// </summary>
internal sealed class DefaultConfig : IConfig
{
    /// <inheritdoc/>
    public bool IsEnabled { get; set; } = true;

    /// <inheritdoc/>
    public bool Debug { get; set; } = false;
}