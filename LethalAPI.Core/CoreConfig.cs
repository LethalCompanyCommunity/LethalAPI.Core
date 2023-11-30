// -----------------------------------------------------------------------
// <copyright file="CoreConfig.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core;

using Interfaces;

/// <summary>
/// The main config instance for LethalAPI.Core.
/// </summary>
// Named "CoreConfig" as to not be confused with the Config class.
public sealed class CoreConfig : IConfig
{
    /// <inheritdoc />
    public bool IsEnabled { get; set; } = true;

    /// <inheritdoc />
    public bool Debug { get; set; } = false;
}