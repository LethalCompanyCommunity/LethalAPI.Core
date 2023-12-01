// -----------------------------------------------------------------------
// <copyright file="CommentsObjectDescriptor.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Thanks to Antoine Aubry for this awesome work.
// Checkout more here: https://dotnetfiddle.net/8M6iIE.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs.Tools;

using System;

using YamlDotNet.Core;
using YamlDotNet.Serialization;

/// <summary>
/// Gathers comments for nodes.
/// </summary>
public sealed class CommentsObjectDescriptor : IObjectDescriptor
{
    private readonly IObjectDescriptor innerDescriptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentsObjectDescriptor"/> class.
    /// </summary>
    /// <param name="innerDescriptor">The inner descriptor instance.</param>
    /// <param name="comment">The comment to be written.</param>
    public CommentsObjectDescriptor(IObjectDescriptor innerDescriptor, string comment)
    {
        this.innerDescriptor = innerDescriptor;
        this.Comment = comment;
    }

    /// <summary>
    /// Gets the comment to be written.
    /// </summary>
    public string Comment { get; private set; }

    /// <inheritdoc cref="IObjectDescriptor" />
    public object? Value => innerDescriptor.Value;

    /// <inheritdoc cref="IObjectDescriptor" />
    public Type Type => innerDescriptor.Type;

    /// <inheritdoc cref="IObjectDescriptor" />
    public Type StaticType => innerDescriptor.StaticType;

    /// <inheritdoc cref="IObjectDescriptor" />
    public ScalarStyle ScalarStyle => innerDescriptor.ScalarStyle;
}