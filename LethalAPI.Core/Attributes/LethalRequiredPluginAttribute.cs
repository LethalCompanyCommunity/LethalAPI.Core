// -----------------------------------------------------------------------
// <copyright file="LethalRequiredPluginAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Attributes;

using System;

/// <summary>
///     Specifies that a plugin is required for all players to have installed.
/// </summary>
/// <example>
///     <code>
/// [LethalRequiredPlugin]
/// class MyPlugin : Plugin&lt;MyConfig&gt;
/// {
/// }
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class LethalRequiredPluginAttribute : Attribute
{
}