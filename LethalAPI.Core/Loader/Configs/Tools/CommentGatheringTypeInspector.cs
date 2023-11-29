// -----------------------------------------------------------------------
// <copyright file="CommentGatheringTypeInspector.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Thanks to Antoine Aubry for this awesome work.
// Checkout more here: https://dotnetfiddle.net/8M6iIE.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs.Tools;

using System;
using System.Collections.Generic;
using System.Linq;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

/// <summary>
/// Gathers comments for nodes.
/// </summary>
public sealed class CommentGatheringTypeInspector : TypeInspectorSkeleton
{
    private readonly ITypeInspector innerTypeDescriptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentGatheringTypeInspector"/> class.
    /// </summary>
    /// <param name="innerTypeDescriptor">The inner type description instance.</param>
    public CommentGatheringTypeInspector(ITypeInspector innerTypeDescriptor)
    {
        this.innerTypeDescriptor = innerTypeDescriptor ?? throw new ArgumentNullException(nameof(innerTypeDescriptor));
    }

    /// <inheritdoc/>
    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
    {
        return innerTypeDescriptor
            .GetProperties(type, container)
            .Select(descriptor => new CommentsPropertyDescriptor(descriptor));
    }
}