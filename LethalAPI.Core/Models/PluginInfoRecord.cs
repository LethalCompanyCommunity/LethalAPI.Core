// -----------------------------------------------------------------------
// <copyright file="PluginInfoRecord.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Models;

using System;

/// <summary>
///     A record containing information about a plugin.
/// </summary>
/// <param name="GUID"> The GUID of the plugin. </param>
/// <param name="Version"> The version of the plugin. </param>
/// <param name="IsRequired"> True if the plugin is required, false otherwise. </param>
[Serializable]
public record PluginInfoRecord(string GUID, Version Version, bool IsRequired);