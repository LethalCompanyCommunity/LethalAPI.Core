// -----------------------------------------------------------------------
// <copyright file="ResourceParser.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Resources;

#pragma warning disable SA1201 // Constructor shouldn't follow a method.

using System.Collections.Generic;
using System.IO;

/// <summary>
/// Contains the implementation for resource parsing.
/// </summary>
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global
public abstract class ResourceParser
{
    /// <summary>
    /// Gets a dictionary containing all registered ResourceParser.
    /// </summary>
    /// <example>
    /// <code>
    /// "FileExtension": Parser
    /// "png": PngParser
    /// </code>
    /// </example>
    public static Dictionary<string, ResourceParser> Parsers { get; private set; } = new();

    /// <summary>
    /// Gets a dictionary containing any cached embedded resources.
    /// </summary>
    public static Dictionary<string, List<EmbeddedResourceData>> CachedResources { get; private set; } = new()
    {
        { "unknown", new List<EmbeddedResourceData>() },
        { "dll", new List<EmbeddedResourceData>() },
    };

    /// <summary>
    /// Registers a parser.
    /// </summary>
    /// <param name="parser">The parser to be registered.</param>
    public static void RegisterParser(ResourceParser parser)
    {
        Log.Debug($"Registering Parser '{parser.GetType().FullName}'.", EmbeddedResourceLoader.Debug);
        if(!Parsers.ContainsKey(parser.ExtensionName.ToLower()))
            Parsers.Add(parser.ExtensionName.ToLower(), parser);

        if(!CachedResources.ContainsKey(parser.ExtensionName.ToLower()))
            CachedResources.Add(parser.ExtensionName.ToLower(), new List<EmbeddedResourceData>());

        if (CachedResources["unknown"].Count > 0)
        {
            int i = 0;
            foreach (EmbeddedResourceData resource in CachedResources["unknown"])
            {
                string extension = resource.GetFileExtension(new List<string>() { parser.ExtensionName });
                if (extension == string.Empty)
                    continue;

                parser.ResourceFound(resource);
                i++;
            }

            Log.Debug($"Found {i} related cached resources. [{Parsers.Count}]");
        }

        Log.Debug($"Did not find any related cached resources. [{Parsers.Count}]");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceParser"/> class.
    /// </summary>
    protected ResourceParser()
    {
        RegisterParser(this);
    }

    /// <summary>
    /// Gets the name of the extension that this will parse.
    /// </summary>
    /// <remarks>
    /// Case-Insensitive.
    /// </remarks>
    public abstract string ExtensionName { get; }

    /// <summary>
    /// The implementation for parsing a resource.
    /// </summary>
    /// <param name="stream">The uncompressed memory stream to parse.</param>
    /// <returns>The parsed resource as an object.</returns>
    public abstract object Parse(MemoryStream stream);

    /// <summary>
    /// Called whenever a resource is found, which can be parsed by the parser. This is useful for caching purposes.
    /// </summary>
    /// <param name="resourceData">The embedded resource information.</param>
    public virtual void ResourceFound(EmbeddedResourceData resourceData)
    {
        if (!CachedResources.ContainsKey(this.ExtensionName))
            CachedResources.Add(this.ExtensionName, new List<EmbeddedResourceData>());

        CachedResources[this.ExtensionName].Add(resourceData);
        Log.Debug($"Added '{resourceData.FileName}' to cache {this.ExtensionName} [{CachedResources[this.ExtensionName].Count}]", EmbeddedResourceLoader.Debug);
    }
}