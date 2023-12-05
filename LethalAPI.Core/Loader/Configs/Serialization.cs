// -----------------------------------------------------------------------
// <copyright file="Serialization.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED and Netwonsoft.Json-for-Unity.Converters
// (https://github.com/Exiled-Team/EXILED and https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters)
// Licensed under the CC BY SA 3 license and MIT.
// View CC BY SA 3 License here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// View the MIT License here:
// https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters/blob/master/LICENSE.md
// Changes: Namespace adjustments, minor tweaks to converters.
// -----------------------------------------------------------------------

#pragma warning disable SA1201 // property shouldn't follow a method.
namespace LethalAPI.Core.Loader.Configs;

using System.ComponentModel;
using System.Globalization;

using Converters.Json;
using Converters.Yaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tools;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

using ColorConverter = Converters.Yaml.ColorConverter;

/// <summary>
/// Provides the base serializer for configs.
/// </summary>
public static class Serialization
{
    /// <summary>
    /// Gets or sets the yaml serializer for configs.
    /// </summary>
    public static ISerializer YamlSerializer { get; set; } = new SerializerBuilder()
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
    /// Gets or sets provides the base yaml deserializer for configs.
    /// </summary>
    public static IDeserializer YamlDeserializer { get; set; } = new DeserializerBuilder()
        .EnablePrivateConstructors()
        .WithTypeConverter(new VectorsConverter())
        .WithTypeConverter(new ColorConverter())
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
        .IgnoreFields()
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Gets or sets the json serializer for general use.
    /// </summary>
    public static JsonSerializer JsonSerializer { get; set; } = JsonSerializer.CreateDefault(DefaultJsonSerializerSettings);

    /// <summary>
    /// Gets the default settings used by LethalAPI for json serialization and deserialization.
    /// </summary>
    public static JsonSerializerSettings DefaultJsonSerializerSettings => new()
    {
        ContractResolver = new UnityTypeContractResolver(),
        Converters = new JsonConverter[]
        {
            // Unity Specific Converters
            new Color32Converter(),
            new Converters.Json.ColorConverter(),
            new QuaternionConverter(),
            new Vector2Converter(),
            new Vector2IntConverter(),
            new Vector3Converter(),
            new Vector3IntConverter(),
            new Vector4Converter(),

            // Default Newtonsoft.Json Converters
            new BinaryConverter(),
            new DataSetConverter(),
            new DataTableConverter(),
            new IsoDateTimeConverter(),
            new JavaScriptDateTimeConverter(),
            new UnixDateTimeConverter(),
            new XmlNodeConverter(),
            new KeyValuePairConverter(),
            new RegexConverter(),
            new StringEnumConverter(),
            new VersionConverter(),

            // dynamic tools
            new ExpandoObjectConverter(),

            // Unused Newtonsoft.Json Converters.
            /* new BsonObjectIdConverter(), - Obsolete. */
            /* new DiscriminatedUnionConverter(), - F# tool. Unnecessary. */
            /* new EntityKeyMemberConverter(), - Entity Framework. Unnecessary. */
        },
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = Formatting.Indented,

        // Default Parameters - Left here in-case we deem it necessary to change them in the future.
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Culture = CultureInfo.InvariantCulture,
        DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
        DateFormatString = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK",
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        DateParseHandling = DateParseHandling.DateTime,
        FloatParseHandling = FloatParseHandling.Double,
        FloatFormatHandling = FloatFormatHandling.String,
        StringEscapeHandling = StringEscapeHandling.Default,
        PreserveReferencesHandling = PreserveReferencesHandling.None,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        MetadataPropertyHandling = MetadataPropertyHandling.Default,
        DefaultValueHandling = DefaultValueHandling.Include,
        TypeNameHandling = TypeNameHandling.None,
        NullValueHandling = NullValueHandling.Include,
        ObjectCreationHandling = ObjectCreationHandling.Auto,
        CheckAdditionalContent = false,
        MaxDepth = 64,
    };

    /// <summary>
    /// Gets or sets the quotes wrapper type.
    /// </summary>
    [Description("Indicates in which quoted strings in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
    public static ScalarStyle ScalarStyle { get; set; } = ScalarStyle.SingleQuoted;

    /// <summary>
    /// Gets or sets the quotes wrapper type.
    /// </summary>
    [Description("Indicates in which quoted strings with multiline in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
    public static ScalarStyle MultiLineScalarStyle { get; set; } = ScalarStyle.Literal;
}