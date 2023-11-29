// -----------------------------------------------------------------------
// <copyright file="ColorConverter.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs.Converters.Yaml;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using UnityEngine;
using UnityEngine.Pool;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

/// <summary>
/// Converts <see cref="Color"/> to Yaml configs and vice versa.
/// </summary>
public sealed class ColorConverter : IYamlTypeConverter
{
    /// <inheritdoc cref="IYamlTypeConverter" />
    public bool Accepts(Type type) => type == typeof(Color);

    /// <inheritdoc cref="IYamlTypeConverter" />
    public object ReadYaml(IParser parser, Type type)
    {
        if (!parser.TryConsume<MappingStart>(out _))
            throw new InvalidDataException($"Cannot deserialize object of type {type.FullName}");

        List<object> coordinates = ListPool<object>.Get();
        int i = 0;

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            if (i++ % 2 == 0)
            {
                parser.MoveNext();
                continue;
            }

            // Nullable weirdness.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            if (!parser.TryConsume(out Scalar scalar) || !float.TryParse(scalar.Value, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out float coordinate))
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            {
                ListPool<object>.Release(coordinates);
                throw new InvalidDataException("Invalid float value.");
            }

            coordinates.Add(coordinate);
        }

        object color = Activator.CreateInstance(type, coordinates.ToArray());

        ListPool<object>.Release(coordinates);

        return color;
    }

    /// <inheritdoc cref="IYamlTypeConverter" />
    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        Dictionary<string, float> coordinates = DictionaryPool<string, float>.Get();

        if (value is Color color)
        {
            coordinates["r"] = color.r;
            coordinates["g"] = color.g;
            coordinates["b"] = color.b;
            coordinates["a"] = color.a;
        }

        emitter.Emit(new MappingStart());

        foreach (KeyValuePair<string, float> coordinate in coordinates)
        {
            emitter.Emit(new Scalar(coordinate.Key));
            emitter.Emit(new Scalar(coordinate.Value.ToString(CultureInfo.GetCultureInfo("en-US"))));
        }

        DictionaryPool<string, float>.Release(coordinates);
        emitter.Emit(new MappingEnd());
    }
}