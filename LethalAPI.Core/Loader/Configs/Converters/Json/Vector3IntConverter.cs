// -----------------------------------------------------------------------
// <copyright file="Vector3IntConverter.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// From https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters
// Licensed under MIT.
// View the license here:
// https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters/blob/master/LICENSE.md
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs.Converters.Json;

using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Custom Newtonsoft.Json converter <see cref="JsonConverter"/> for the Unity integer Vector3 type <see cref="Vector3Int"/>.
/// </summary>
public class Vector3IntConverter : PartialConverter<Vector3Int>
{
    /// <inheritdoc />
    protected override void ReadValue(ref Vector3Int value, string name, JsonReader reader, JsonSerializer serializer)
    {
        switch (name)
        {
            case nameof(value.x):
                value.x = reader.ReadAsInt32() ?? 0;
                break;
            case nameof(value.y):
                value.y = reader.ReadAsInt32() ?? 0;
                break;
            case nameof(value.z):
                value.z = reader.ReadAsInt32() ?? 0;
                break;
        }
    }

    /// <inheritdoc />
    protected override void WriteJsonProperties(JsonWriter writer, Vector3Int value, JsonSerializer serializer)
    {
        writer.WritePropertyName(nameof(value.x));
        writer.WriteValue(value.x);
        writer.WritePropertyName(nameof(value.y));
        writer.WriteValue(value.y);
        writer.WritePropertyName(nameof(value.z));
        writer.WriteValue(value.z);
    }
}