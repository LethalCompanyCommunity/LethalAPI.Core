// -----------------------------------------------------------------------
// <copyright file="ValidatingNodeDeserializer.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments.
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Loader.Configs.Tools;

using System;

using YamlDotNet.Core;
using YamlDotNet.Serialization;

/// <summary>
/// Basic configs validation.
/// </summary>
public sealed class ValidatingNodeDeserializer : INodeDeserializer
{
    private readonly INodeDeserializer nodeDeserializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatingNodeDeserializer"/> class.
    /// </summary>
    /// <param name="nodeDeserializer">The node deserializer instance.</param>
    public ValidatingNodeDeserializer(INodeDeserializer nodeDeserializer)
    {
        this.nodeDeserializer = nodeDeserializer;
    }

    /// <inheritdoc cref="INodeDeserializer"/>
    public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
    {
        if (nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
        {
            if (value is null)
                Log.Error("Yaml Deserializer Null value (ValidatingNodeDeserializer)");

            // tragic this doesnt work but it severely breaks things (like really really really badly)
            // it would allow for [Required] [StringLength] etc... but it doesnt work!
            // Validator.ValidateObject(value!, new ValidationContext(value!, null, null), true);
            return true;
        }

        return false;
    }
}