// -----------------------------------------------------------------------
// <copyright file="LethalEntrypointAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Attributes;

using System;

/// <summary>
/// Defines a method as an entrypoint. You must have a method to handle enabling the plugin.
/// </summary>
/// <remarks>
/// Naming a void method 'OnEnabled()' can be used instead of this.
/// </remarks>
/// <example>
/// <code>
/// [LethalPlugin]
/// class MyPlugin
/// {
///    void OnEnabled()
///    { }
///    /*-----[OR]-----*/
///    [LethalEntrypoint]
///    void OnStart()
///    { }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method)]
public sealed class LethalEntrypointAttribute : Attribute
{
}