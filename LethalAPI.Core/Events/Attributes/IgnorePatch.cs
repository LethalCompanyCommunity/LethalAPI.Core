// -----------------------------------------------------------------------
// <copyright file="IgnorePatch.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events.Attributes;

using System;

/// <summary>
/// Indicates to the <see cref="Features.Patcher"/> to not patch a class.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class IgnorePatch : Attribute
{
}