// -----------------------------------------------------------------------
// <copyright file="EmbeddedResourceLoader.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Resources;

using System;
using System.Reflection;

/// <summary>
/// Contains the implementation for loading embedded files from the manifest..
/// </summary>
public class EmbeddedResourceLoader
{
    /// <summary>
    /// Indicates whether debug logs should be shown for the embedded resource loader.
    /// </summary>
    internal static readonly bool Debug = true;

    /// <summary>
    /// Gets the static instance of the <see cref="EmbeddedResourceLoader"/>.
    /// </summary>
    public static EmbeddedResourceLoader Instance => new();

    /// <summary>
    /// Gets a list of embedded resources in an assembly.
    /// </summary>
    /// <param name="assembly">The assembly to retrieve the resources from.</param>
    public void GetEmbeddedObjects(Assembly assembly)
    {
        string[] resourceNames = assembly.GetManifestResourceNames();

        foreach (string name in resourceNames)
        {
            Log.Debug($"Loading resource {name}", Debug, "Lethal-Loader");
            try
            {
                // Cached in ResourceParser.CachedResources.
                _ = new EmbeddedResourceData(name, assembly);
            }
            catch (ArgumentNullException)
            {
            }
            catch (Exception e)
            {
                Log.Debug($"Resource '{name}' could not be loaded. It was probably embedded incorrectly. Exception: \n{e}", Debug, "Lethal-Loader-Resources");
            }
        }
    }
}