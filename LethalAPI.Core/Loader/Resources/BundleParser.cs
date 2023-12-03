// -----------------------------------------------------------------------
// <copyright file="BundleParser.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Resources;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using JetBrains.Annotations;
using UnityEngine;

/// <inheritdoc />
public class BundleParser : ResourceParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BundleParser"/> class.
    /// </summary>
    internal BundleParser()
    {
        Instance = this;
    }

    /// <summary>
    /// Gets the instance of this resource parser.
    /// </summary>
    public static ResourceParser Instance { get; private set; } = null!;

    /// <summary>
    /// Gets a list of all cached asset bundles by name.
    /// </summary>
    /// <example>
    /// MyBundle.bundle -> CachedBundles[MyBundle].
    /// </example>
    public static Dictionary<string, AssetBundle> CachedBundles => new ();

    /// <inheritdoc />
    public override string ExtensionName => "bundle";

    /// <summary>
    /// Tries to get an asset bundle from the embedded bundles.
    /// </summary>
    /// <param name="name">The name of the bundle.</param>
    /// <param name="bundle">The returned bundle.</param>
    /// <returns>True if the bundle was found, false if the bundle could not be found.</returns>
    public static bool TryGetBundle(string name, [NotNullWhen(true)] out AssetBundle? bundle)
    {
        bundle = null;
        if (CachedBundles.ContainsKey(name))
        {
            bundle = CachedBundles[name];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets a cached asset resource.
    /// </summary>
    /// <param name="assetName">The name of the asset.</param>
    /// <param name="bundleName">The name of the bundle (optional).</param>
    /// <typeparam name="T">The type of resource to get.</typeparam>
    /// <returns>The asset resource if found. Null if not found.</returns>
    public static T? GetResource<T>(string assetName, string bundleName = "")
        where T : Object
    {
        if (bundleName != string.Empty && !CachedBundles.ContainsKey(bundleName))
            return null;

        List<AssetBundle> bundles = bundleName == string.Empty ? CachedBundles.Values.ToList() : new() { CachedBundles[bundleName] };
        foreach (AssetBundle bundle in bundles)
        {
            return bundle.LoadAsset<T>(assetName);
        }

        return null;
    }

    /// <inheritdoc />
    public override object Parse(MemoryStream stream)
    {
        return AssetBundle.LoadFromStream(stream);
    }

    /// <inheritdoc />
    public override void ResourceFound(EmbeddedResourceData resourceData)
    {
        CachedBundles.Add(resourceData.FileName.Replace(".bundle", string.Empty), (AssetBundle)Parse(resourceData.GetStream()));
    }
}