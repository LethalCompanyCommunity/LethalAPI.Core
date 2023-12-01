// -----------------------------------------------------------------------
// <copyright file="DllParser.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Resources;

using System;
using System.IO;
using System.Reflection;

/// <summary>
/// Contains the implementation for parsing an assembly.
/// </summary>
public sealed class DllParser : ResourceParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DllParser"/> class.
    /// </summary>
    public DllParser()
    {
        Instance = this;
    }

    /// <summary>
    /// Gets the main instance of the <see cref="DllParser"/>.
    /// </summary>
    public static DllParser Instance { get; private set; } = null!;

    /// <inheritdoc />
    public override string ExtensionName => "dll";

    /// <inheritdoc />
    public override object Parse(MemoryStream stream)
    {
        return Assembly.Load(stream.ToArray());
    }

    /// <inheritdoc />
    public override void ResourceFound(EmbeddedResourceData resourceData)
    {
        try
        {
            // resourceData.AssemblyName = AssemblyName.GetAssemblyName(resourceData.FileLocation);
        }
        catch (Exception e)
        {
            Log.Debug($"Couldn't find an AssemblyName instance for the dll resource '{resourceData.FileLocation}'. \n{e}", EmbeddedResourceLoader.Debug, "LethalAPI-Loader-DepsLoader");
        }

        base.ResourceFound(resourceData);
    }
}