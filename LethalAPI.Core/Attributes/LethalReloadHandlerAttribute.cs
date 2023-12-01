// -----------------------------------------------------------------------
// <copyright file="LethalReloadHandlerAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Attributes;

using System;

/// <summary>
/// Defines a method to handle reloading. You are not required to implement a reload handler. It is optional.
/// </summary>
/// <remarks>
/// Naming a void method 'OnReloaded()' can be used instead of this.
/// </remarks>
/// <example>
/// <code>
/// [LethalPlugin]
/// class MyPlugin
/// {
///    void OnReloaded()
///    { }
///    /*-----[OR]-----*/
///    [LethalReloadHandler]
///    void Reload()
///    { }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method)]
public sealed class LethalReloadHandlerAttribute : Attribute
{
}