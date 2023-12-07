// -----------------------------------------------------------------------
// <copyright file="ColorConverter.cs" company="LethalAPI Modding Community">
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
/// Custom Newtonsoft.Json converter <see cref="JsonConverter"/> for the Unity Color type <see cref="Color"/>.
/// </summary>
public class ColorConverter : PartialConverter<Color>
{
    /// <inheritdoc />
    protected override void ReadValue(ref Color value, string name, JsonReader reader, JsonSerializer serializer)
    {
        switch (name)
        {
            case nameof(value.r):
                value.r = reader.ReadAsFloat() ?? 0f;
                break;
            case nameof(value.g):
                value.g = reader.ReadAsFloat() ?? 0f;
                break;
            case nameof(value.b):
                value.b = reader.ReadAsFloat() ?? 0f;
                break;
            case nameof(value.a):
                value.a = reader.ReadAsFloat() ?? 0f;
                break;
        }
    }

    /// <inheritdoc />
    protected override void WriteJsonProperties(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        writer.WritePropertyName(nameof(value.r));
        writer.WriteValue(value.r);
        writer.WritePropertyName(nameof(value.g));
        writer.WriteValue(value.g);
        writer.WritePropertyName(nameof(value.b));
        writer.WriteValue(value.b);
        writer.WritePropertyName(nameof(value.a));
        writer.WriteValue(value.a);
    }
}