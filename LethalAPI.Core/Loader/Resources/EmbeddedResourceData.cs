// -----------------------------------------------------------------------
// <copyright file="EmbeddedResourceData.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable SA1629 // documentation should end with a period.

namespace LethalAPI.Core.Loader.Resources;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

/// <summary>
/// Contains information about resources embedded in the assembly manifest.
/// </summary>
public class EmbeddedResourceData
{
    private static readonly bool AcceptFirstMatch = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedResourceData"/> class.
    /// </summary>
    /// <param name="fileLoc">The file location.</param>
    /// <param name="assembly">The assembly of the resource.</param>
    internal EmbeddedResourceData(string fileLoc, Assembly assembly)
    {
        this.AssemblyName = null;
        this.Assembly = assembly;
        this.FileLocation = fileLoc;
        Stream? dataStream = assembly.GetManifestResourceStream(this.FileLocation);
        if (dataStream is null)
        {
            Log.Error($"Invalid embedded resource '{fileLoc}' found in Assembly '{assembly.FullName}'.");
            throw new ArgumentNullException(
                nameof(fileLoc),
                "The associated data-stream was null. This resource cannot be loaded.");
        }

        dataStream.Close();
        this.IsCompressed = CheckIfCompressed();
        this.FileName = Path.GetFileName(this.IsCompressed
            ? this.FileLocation.Replace(".compressed", string.Empty)
            : this.FileLocation);
        this.FileExtension = GetFileExtension();
        this.IsDll = this.FileExtension is "dll";

        if (this.FileExtension == string.Empty || !ResourceParser.Parsers.ContainsKey(this.FileExtension))
        {
            this.Parser = null;
            Log.Debug($"No file parser found for file \"{this.FileName}\". Resorting to the 'unknown' file parser.");
            UnknownResourceParser.Instance.ResourceFound(this);
        }
        else
        {
            this.Parser = ResourceParser.Parsers[this.FileExtension];
            this.Parser.ResourceFound(this);
        }
    }

    /// <summary>
    /// Gets the resource parser if it has one.
    /// </summary>
    public ResourceParser? Parser { get; }

    /// <summary>
    /// Gets a value indicating whether or not the file is a dll.
    /// </summary>
    public bool IsDll { get; }

    /// <summary>
    /// Gets the original file location from the manifest list.
    /// </summary>
    /// <remarks>If a file is compressed, this will always keep the .compressed extension.</remarks>
    public string FileLocation { get; }

    /// <summary> Gets the file name with the extension. </summary>
    /// <remarks> Compressed extensions will be stripped by this point. <code>
    /// Original:
    ///   - picture.png.compressed
    /// FileName:
    ///   - picture.png
    /// </code></remarks>
    /// <example> picture.png </example>
    public string FileName { get; }

    /// <summary> Gets or sets the file extension. </summary>
    /// <remarks> Compressed extensions will be stripped by this point. <code>
    /// Original:
    ///   - picture.png.compressed
    /// FileName:
    ///   - png
    /// </code></remarks>
    /// <example> png </example>
    /// <seealso cref="IsCompressed"/>
    public string FileExtension { get; set; }

    /// <summary>
    /// Gets a value indicating whether or not the file is compressed.
    /// </summary>
    public bool IsCompressed { get; }

    /// <summary>
    /// Gets the assembly of the resource.
    /// </summary>
    public Assembly Assembly { get; }

    /// <summary>
    /// Gets or sets the <see cref="AssemblyName"/> information if this embedded resource is a DLL. This will be null for non DLL's, and assemblies without AssemblyName info.
    /// </summary>
    public AssemblyName? AssemblyName { get; set; }

    /// <summary>
    /// Gets the memory stream of the file. Compressed files will be uncompressed by this point. The memory stream will also be uncompressed.
    /// </summary>
    /// <returns>The uncompressed memory stream that is retrieved.</returns>
    public MemoryStream GetStream()
    {
        Stream? dataStream = this.Assembly.GetManifestResourceStream(this.FileLocation);
        if (dataStream == null)
        {
            Log.Error($"Could not load memory stream from embedded resource '{this.FileLocation}' found in Assembly '{this.Assembly.FullName}'.");

            // ReSharper disable once NotResolvedInText
            throw new NullReferenceException("The associated data-stream for the resource was null. This resource cannot be loaded.");
        }

        MemoryStream memStream = new();
        if (this.IsCompressed)
        {
            DeflateStream decompressionStream = new(dataStream, CompressionMode.Decompress);
            decompressionStream.CopyTo(memStream);
        }
        else
        {
            dataStream.CopyTo(memStream);
        }

        return memStream;
    }

    /// <summary>
    /// Gets a file extensions of the file.
    /// </summary>
    /// <param name="validExtensions">A list of valid file extensions it can find.</param>
    /// <returns>The name of the file extension found, or an empty string if not found.</returns>
    public string GetFileExtension(List<string>? validExtensions = null)
    {
        string queryName = IsCompressed ? FileName.Replace(".compressed", string.Empty).ToLower() : FileName.ToLower();
        string result = string.Empty;
        validExtensions ??= new();
        foreach (KeyValuePair<string, ResourceParser> x in ResourceParser.Parsers)
        {
            validExtensions.Add(x.Key.ToLower());
        }

        if (validExtensions.Count == 0)
        {
            Log.Debug("No valid extensions have been registered yet.", EmbeddedResourceLoader.Debug);
            return string.Empty;
        }

        List<string> optionsToKeep = new();
        for (int i = 0; i < validExtensions.OrderByDescending(x => x.Length).First().Length; i++)
        {
            // For when no matches are found but one of the options is larger than the file location.
            // ie. File 'img.png' vs '.metafile' - prevents errors.
            if (queryName.Length <= i)
                return result;

            // This will compare the file location to the custom file extensions, starting from the last char -> first char.
            // Using this method is more optimal as we can break from the first instance. (less total instructions in theory.)
            char currentChar = queryName[queryName.Length - i - 1];
            foreach (string option in validExtensions)
            {
                // This option does not match. This compares the chars of both indexes (which need to be the same).
                // We don't add this option to the keeper list.
                if (option[option.Length - i - 1] != currentChar)
                    continue;

                // This option is matched. if there are two options, we may want to keep searching to ensure there isn't a larger option that matches
                // This can be disabled if we toggle AcceptFirstMatch / goto the found match part.
                if (option.Length == i + 1)
                {
                    if (AcceptFirstMatch)
                        return result;

                    // Ex: .gz and .tar.gz -
                    // If we skip to the foundMatch, .tar.gz cannot be a returned value, given that .gz is already a value.
                    result = option;
                    continue;
                }

                // This option can keep being searched. It matches so far, but hasn't been fully matched yet.
                optionsToKeep.Add(option);
            }

            // Clear results add the remaining valid options to iterate through.
            // note: if we do validOptions = optionsToKeep, this won't actually add the options, as validOptions will just point to the cleared optionsToKeep.
            validExtensions.Clear();
            validExtensions.InsertRange(0, optionsToKeep);
            optionsToKeep.Clear();

            // Because we use .First(), we need to break here in case there are no more options. Otherwise it's an enumerable exception, as First() is called before we can check.
            if (validExtensions.Count == 0)
                return result;
        }

        return result;
    }

    private bool CheckIfCompressed() => this.FileLocation.ToLower().EndsWith("compressed");
}