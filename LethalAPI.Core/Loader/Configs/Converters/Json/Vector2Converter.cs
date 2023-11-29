// -----------------------------------------------------------------------
// <copyright file="Vector2Converter.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// From https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters
// Licensed under MIT.
// View the license here:
// https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters/blob/master/LICENSE.md
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs.Converters.Json;

using Helpers;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Custom Newtonsoft.Json converter <see cref="JsonConverter{T}"/> for the Unity Vector2 type <see cref="Vector2"/>.
/// </summary>
public class Vector2Converter : PartialConverter<Vector2>
{
    /// <inheritdoc />
    protected override void ReadValue(ref Vector2 value, string name, JsonReader reader, JsonSerializer serializer)
    {
        switch (name)
        {
            case nameof(value.x):
                value.x = reader.ReadAsFloat() ?? 0f;
                break;
            case nameof(value.y):
                value.y = reader.ReadAsFloat() ?? 0f;
                break;
        }
    }

    /// <inheritdoc />
    protected override void WriteJsonProperties(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WritePropertyName(nameof(value.x));
        writer.WriteValue(value.x);
        writer.WritePropertyName(nameof(value.y));
        writer.WriteValue(value.y);
    }
}