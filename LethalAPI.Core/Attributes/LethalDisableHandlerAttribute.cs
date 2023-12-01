// -----------------------------------------------------------------------
// <copyright file="LethalDisableHandlerAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Attributes;

using System;

/// <summary>
/// Defines a method to handle disabling. You are not required to implement a disabled handler. It is optional.
/// </summary>
/// <remarks>
/// Naming a void method 'OnDisabled()' can be used instead of this.
/// </remarks>
/// <example>
/// <code>
/// [LethalPlugin]
/// class MyPlugin
/// {
///    void OnDisabled()
///    { }
///    /*-----[OR]-----*/
///    [LethalDisableHandler]
///    void Disable()
///    { }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method)]
public sealed class LethalDisableHandlerAttribute : Attribute
{
}