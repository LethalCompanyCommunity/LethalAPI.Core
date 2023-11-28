// -----------------------------------------------------------------------
// <copyright file="CommentsPropertyDescriptor.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Thanks to Antoine Aubry for this awesome work.
// Checkout more here: https://dotnetfiddle.net/8M6iIE.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs.Tools;

using System;
using System.ComponentModel;

using YamlDotNet.Core;
using YamlDotNet.Serialization;

/// <summary>
/// Establishes a comment property descriptor.
/// </summary>
public sealed class CommentsPropertyDescriptor : IPropertyDescriptor
{
    private readonly IPropertyDescriptor baseDescriptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentsPropertyDescriptor"/> class.
    /// </summary>
    /// <param name="baseDescriptor">The base descriptor instance.</param>
    public CommentsPropertyDescriptor(IPropertyDescriptor baseDescriptor)
    {
        this.baseDescriptor = baseDescriptor;
        Name = baseDescriptor.Name;
    }

    /// <inheritdoc cref="IPropertyDescriptor"/>
    public string Name { get; set; }

    /// <inheritdoc cref="IPropertyDescriptor"/>
    public Type Type => baseDescriptor.Type;

    /// <inheritdoc cref="IPropertyDescriptor"/>
    public Type? TypeOverride
    {
        get => baseDescriptor.TypeOverride;
        set => baseDescriptor.TypeOverride = value;
    }

    /// <inheritdoc cref="IPropertyDescriptor"/>
    public int Order { get; set; }

    /// <inheritdoc cref="IPropertyDescriptor"/>
    public ScalarStyle ScalarStyle
    {
        get => baseDescriptor.ScalarStyle;
        set => baseDescriptor.ScalarStyle = value;
    }

    /// <inheritdoc cref="IPropertyDescriptor"/>
    public bool CanWrite => baseDescriptor.CanWrite;

    /// <inheritdoc cref="IPropertyDescriptor"/>
    public void Write(object target, object? value)
    {
        baseDescriptor.Write(target, value);
    }

    /// <inheritdoc cref="IPropertyDescriptor"/>
    public T? GetCustomAttribute<T>()
        where T : Attribute => baseDescriptor.GetCustomAttribute<T>();

    /// <inheritdoc cref="IPropertyDescriptor"/>
    public IObjectDescriptor Read(object target)
    {
        DescriptionAttribute? description = baseDescriptor.GetCustomAttribute<DescriptionAttribute>();
        return
            description is not null
                ? new CommentsObjectDescriptor(baseDescriptor.Read(target), description.Description)
                : baseDescriptor.Read(target);
    }
}