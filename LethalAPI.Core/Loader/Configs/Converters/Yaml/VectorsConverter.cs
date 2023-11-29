// -----------------------------------------------------------------------
// <copyright file="VectorsConverter.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
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
/// Converts a Vector2, Vector3 or Vector4 to Yaml configs and vice versa.
/// </summary>
public sealed class VectorsConverter : IYamlTypeConverter
{
    /// <inheritdoc cref="IYamlTypeConverter" />
    public bool Accepts(Type type) => type == typeof(Vector2) || type == typeof(Vector2Int) || type == typeof(Vector3) || type == typeof(Vector3Int) || type == typeof(Vector4);

    /// <inheritdoc cref="IYamlTypeConverter" />
    public object ReadYaml(IParser parser, Type type)
    {
        if (!parser.TryConsume<MappingStart>(out _))
            throw new InvalidDataException($"Cannot deserialize object of type {type.FullName}.");

        List<object> coordinates = ListPool<object>.Get();
        int i = 0;
        bool isInt = type == typeof(Vector2Int) || type == typeof(Vector3Int);

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            if (i++ % 2 == 0)
            {
                parser.MoveNext();
                continue;
            }

            if (!parser.TryConsume(out Scalar scalar))
                goto invalidValue;

            // Nullable weirdness.
            if (!isInt && float.TryParse(scalar.Value, NumberStyles.Float, new CultureInfo("en-US"), out float coordinate))
            {
                coordinates.Add(coordinate);
                continue;
            }

            if (isInt && int.TryParse(scalar.Value, NumberStyles.Integer, new CultureInfo("en-US"), out int coordinateInt))
            {
                coordinates.Add(coordinateInt);
                continue;
            }

            invalidValue:
            ListPool<object>.Release(coordinates);
            throw new InvalidDataException($"Invalid {(isInt ? "Integer" : "Float")} value.");
        }

        object vector = Activator.CreateInstance(type, coordinates.ToArray());

        ListPool<object>.Release(coordinates);

        return vector;
    }

    /// <inheritdoc cref="IYamlTypeConverter" />
    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        Dictionary<string, float> coordinates = DictionaryPool<string, float>.Get();

        switch (value)
        {
            case Vector2 vector2:
                coordinates["x"] = vector2.x;
                coordinates["y"] = vector2.y;
                break;
            case Vector2Int vector2i:
                coordinates["x"] = vector2i.x;
                coordinates["y"] = vector2i.y;
                break;
            case Vector3 vector3:
                coordinates["x"] = vector3.x;
                coordinates["y"] = vector3.y;
                coordinates["z"] = vector3.z;
                break;
            case Vector3Int vector3i:
                coordinates["x"] = vector3i.x;
                coordinates["y"] = vector3i.y;
                coordinates["z"] = vector3i.z;
                break;
            case Vector4 vector4:
                coordinates["x"] = vector4.x;
                coordinates["y"] = vector4.y;
                coordinates["z"] = vector4.z;
                coordinates["w"] = vector4.w;
                break;
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
