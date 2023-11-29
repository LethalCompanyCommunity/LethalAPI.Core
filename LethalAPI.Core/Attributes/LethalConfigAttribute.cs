// -----------------------------------------------------------------------
// <copyright file="LethalConfigAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Attributes;

using System;

/// <summary>
/// Represents a config defined via attributes.
/// </summary>
/// <remarks>
/// Naming a field 'Config' can be used instead of this.
/// Ensure that the type of the field that this is placed on inherits IConfig.
/// </remarks>
/// <example>
/// <code>
/// [LethalPlugin]
/// class MyPlugin
/// {
///     [LethalConfig]
///     MyConfig Conf;
///     /*-----[OR]-----*/
///     MyConfig Config;
/// }
///
/// class MyConfig : IConfig
/// {
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class LethalConfigAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LethalConfigAttribute"/> class.
    /// </summary>
    public LethalConfigAttribute()
    {
    }
}
