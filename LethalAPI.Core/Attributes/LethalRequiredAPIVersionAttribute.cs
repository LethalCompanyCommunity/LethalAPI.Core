// -----------------------------------------------------------------------
// <copyright file="LethalRequiredAPIVersionAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Attributes;

using System;

/// <summary>
/// Specifies the minimum version of the lethal framework required to work. This is optional.
/// </summary>
/// <example>
/// <code>
/// [LethalRequiredFrameworkVersion(1, 0, 0)]
/// class MyPlugin : Plugin&amp;lt;MyConfig&amp;gt;
/// {
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class LethalRequiredAPIVersionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LethalRequiredAPIVersionAttribute"/> class.
    /// </summary>
    /// <param name="major">The major version.</param>
    /// <param name="minor">The minor version.</param>
    /// <param name="revision">The revision version.</param>
    public LethalRequiredAPIVersionAttribute(int major = 1, int minor = 0, int revision = 0)
    {
        this.Version = new Version(major, minor, revision);
    }

    /// <summary>
    /// Gets the version required.
    /// </summary>
    public Version Version { get; }
}