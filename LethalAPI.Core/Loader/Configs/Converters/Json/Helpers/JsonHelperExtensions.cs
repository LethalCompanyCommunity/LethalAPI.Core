// -----------------------------------------------------------------------
// <copyright file="JsonHelperExtensions.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// From https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters
// Licensed under MIT.
// View the license here:
// https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters/blob/master/LICENSE.md
// -----------------------------------------------------------------------

#pragma warning disable SA1600 // elements should be documented.
namespace LethalAPI.Core.Loader.Configs.Converters.Json.Helpers;

using System;
using System.Globalization;
using System.Reflection;
using System.Text;

using Newtonsoft.Json;

/// <summary>
/// Helper extensions for json tools.
/// </summary>
internal static class JsonHelperExtensions
{
    /// <summary>
    /// This refers to the ctor that lets you specify the line number and
    /// position that was introduced in Json.NET v12.0.1.
    /// <see cref="JsonSerializationException"/>
    /// <see href="https://github.com/JamesNK/Newtonsoft.Json/blob/12.0.1/Src/Newtonsoft.Json/JsonSerializationException.cs#L110"/>.
    /// </summary>
    private static readonly ConstructorInfo? JsonSerializationExceptionPositionalCtor = typeof(JsonSerializationException).GetConstructor(new[]
    {
        typeof(string), typeof(string), typeof(int), typeof(int), typeof(Exception),
    });

    public static JsonSerializationException CreateSerializationException(this JsonReader reader, string message, Exception? innerException = null)
    {
        StringBuilder builder = CreateStringBuilderWithSpaceAfter(message);

        builder.AppendFormat(CultureInfo.InvariantCulture, "Path '{0}'", reader.Path);

        IJsonLineInfo? lineInfo = reader as IJsonLineInfo;
        int lineNumber = default;
        int linePosition = default;

        if (lineInfo?.HasLineInfo() == true)
        {
            lineNumber = lineInfo.LineNumber;
            linePosition = lineInfo.LinePosition;
            builder.AppendFormat(CultureInfo.InvariantCulture, ", line {0}, position {1}", lineNumber, linePosition);
        }

        builder.Append('.');

        return NewJsonSerializationException(
            message: builder.ToString(), reader.Path, lineNumber, linePosition, innerException);
    }

    public static JsonWriterException CreateWriterException(this JsonWriter writer, string message, Exception? innerException = null)
    {
        StringBuilder builder = CreateStringBuilderWithSpaceAfter(message);

        builder.AppendFormat(CultureInfo.InvariantCulture, "Path '{0}'.", writer.Path);

        return new JsonWriterException(
            message: builder.ToString(), writer.Path, innerException);
    }

    public static T? ReadViaSerializer<T>(this JsonReader reader, JsonSerializer serializer)
    {
        reader.Read();
        return serializer.Deserialize<T>(reader);
    }

    public static float? ReadAsFloat(this JsonReader reader)
    {
        // https://github.com/jilleJr/Newtonsoft.Json-for-Unity.Converters/issues/46
        string? str = reader.ReadAsString();

        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        if (float.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out float valueParsed))
        {
            return valueParsed;
        }

        return 0f;
    }

    public static byte? ReadAsInt8(this JsonReader reader)
    {
        return checked((byte)(reader.ReadAsInt32() ?? 0));
    }

    private static JsonSerializationException NewJsonSerializationException(string message, string path, int lineNumber, int linePosition, Exception? innerException)
    {
        if (JsonSerializationExceptionPositionalCtor != null)
        {
            return (JsonSerializationException)JsonSerializationExceptionPositionalCtor.Invoke(new object[]
            {
                message, path, lineNumber, linePosition, innerException!,
            });
        }
        else
        {
            return new JsonSerializationException(message, innerException!);
        }
    }

    private static StringBuilder CreateStringBuilderWithSpaceAfter(string message)
    {
        StringBuilder builder = new StringBuilder(message);

        if (message.EndsWith("."))
        {
            builder.Append(' ');
        }
        else if (!message.EndsWith(". "))
        {
            builder.Append(". ");
        }

        return builder;
    }
}