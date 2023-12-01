// -----------------------------------------------------------------------
// <copyright file="UnderscoredNamingConvention.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs.Tools;

using System.Collections.Generic;

using UnityEngine.UIElements;
using YamlDotNet.Serialization;

/// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention"/>
public class UnderscoredNamingConvention : INamingConvention
{
    /// <inheritdoc cref="YamlDotNet.Serialization.NamingConventions.UnderscoredNamingConvention.Instance"/>
    public static UnderscoredNamingConvention Instance { get; } = new();

    /// <summary>
    /// Gets the list.
    /// </summary>
    public List<object> Properties { get; } = new();

    /// <inheritdoc/>
    public string Apply(string value)
    {
        string newValue = value.ToSnakeCase();
        Properties.Add(newValue);
        return newValue;
    }
}