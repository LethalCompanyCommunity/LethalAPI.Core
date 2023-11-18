// -----------------------------------------------------------------------
// <copyright file="UnknownResourceParser.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Resources;

using System.IO;

/// <summary>
/// An implementation dedicated to unresolved parsers.
/// </summary>
public sealed class UnknownResourceParser : ResourceParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownResourceParser"/> class.
    /// </summary>
    public UnknownResourceParser()
    {
        Instance = this;
    }

    /// <summary>
    /// Gets the main instance of <see cref="UnknownResourceParser"/>.
    /// </summary>
    public static UnknownResourceParser Instance { get; private set; } = null!;

    /// <inheritdoc />
    public override string ExtensionName => "unknown";

    /// <inheritdoc />
    /// <exception cref="System.NotImplementedException">This method is not meant to be called for this implementation.</exception>
    public override object Parse(MemoryStream stream) =>
        throw new System.NotImplementedException();
}