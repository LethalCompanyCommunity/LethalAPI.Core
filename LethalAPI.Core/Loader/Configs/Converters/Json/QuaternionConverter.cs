// -----------------------------------------------------------------------
// <copyright file="QuaternionConverter.cs" company="LethalAPI Modding Community">
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
/// Custom Newtonsoft.Json converter <see cref="JsonConverter"/> for the Unity Quaternion type <see cref="Quaternion"/>.
/// </summary>
public class QuaternionConverter : PartialConverter<Quaternion>
{
    /// <inheritdoc />
    protected override void ReadValue(ref Quaternion value, string name, JsonReader reader, JsonSerializer serializer)
    {
        switch (name)
        {
            case nameof(value.x):
                value.x = reader.ReadAsFloat() ?? 0f;
                break;
            case nameof(value.y):
                value.y = reader.ReadAsFloat() ?? 0f;
                break;
            case nameof(value.z):
                value.z = reader.ReadAsFloat() ?? 0f;
                break;
            case nameof(value.w):
                value.w = reader.ReadAsFloat() ?? 0f;
                break;
        }
    }

    /// <inheritdoc />
    protected override void WriteJsonProperties(JsonWriter writer, Quaternion value, JsonSerializer serializer)
    {
        writer.WritePropertyName(nameof(value.x));
        writer.WriteValue(value.x);
        writer.WritePropertyName(nameof(value.y));
        writer.WriteValue(value.y);
        writer.WritePropertyName(nameof(value.z));
        writer.WriteValue(value.z);
        writer.WritePropertyName(nameof(value.w));
        writer.WriteValue(value.w);
    }
}