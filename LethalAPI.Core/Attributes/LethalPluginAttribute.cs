// -----------------------------------------------------------------------
// <copyright file="LethalPluginAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Attributes;

using System;

/// <summary>
/// Represents a plugin defined via attributes.
/// </summary>
/// <remarks>
/// A class can also just inherit the IPlugin&lt;IConfig&gt; interface or Plugin&lt;IConfig&gt; class instead of using this.
/// <code>
/// class MyPlugin : Plugin&lt;MyConfig&gt;
/// {
/// }
/// class MyConfig : IConfig
/// {
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class LethalPluginAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LethalPluginAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the plugin.</param>
    /// <param name="description">A description of the plugin.</param>
    /// <param name="author">The author or authors of the plugin.</param>
    /// <param name="version">The version of the plugin. You can also use the assembly versioning system and leave this null.</param>
    public LethalPluginAttribute(string name, string description, string author, string version)
    {
        this.Name = name;
        this.Description = description;
        this.Author = author;
        this.Version = Version.Parse(version);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LethalPluginAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the plugin.</param>
    /// <param name="description">A description of the plugin.</param>
    /// <param name="author">The author or authors of the plugin.</param>
    public LethalPluginAttribute(string name, string description, string author)
    {
        this.Name = name;
        this.Description = description;
        this.Author = author;
        this.Version = new Version(1, 0, 0);
    }

    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets a description of the plugin.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the author or authors of the plugin.
    /// </summary>
    public string Author { get; }

    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    public Version Version { get; }
}
