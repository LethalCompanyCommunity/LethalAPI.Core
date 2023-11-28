// -----------------------------------------------------------------------
// <copyright file="Serialization.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs;

using System.ComponentModel;

using Converters;
using Tools;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

/// <summary>
/// Provides the base serializer for configs.
/// </summary>
public static class Serialization
{
    /// <summary>
    /// Gets or sets the serializer for configs.
    /// </summary>
    public static ISerializer Serializer { get; set; } = new SerializerBuilder()
        .WithTypeConverter(new VectorsConverter())
        .WithTypeConverter(new ColorConverter())
        .WithEventEmitter(eventEmitter => new TypeAssigningEventEmitter(eventEmitter))
        .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
        .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .IgnoreFields()
        .DisableAliases()
        .Build();

    /// <summary>
    /// Gets or sets provides the base deserializer for configs.
    /// </summary>
    public static IDeserializer Deserializer { get; set; } = new DeserializerBuilder()
        .EnablePrivateConstructors()
        .WithTypeConverter(new VectorsConverter())
        .WithTypeConverter(new ColorConverter())
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
        .IgnoreFields()
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Gets or sets the quotes wrapper type.
    /// </summary>
    [Description(
        "Indicates in which quoted strings in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
    public static ScalarStyle ScalarStyle { get; set; } = ScalarStyle.SingleQuoted;

    /// <summary>
    /// Gets or sets the quotes wrapper type.
    /// </summary>
    [Description(
        "Indicates in which quoted strings with multiline in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
    public static ScalarStyle MultiLineScalarStyle { get; set; } = ScalarStyle.Literal;
}