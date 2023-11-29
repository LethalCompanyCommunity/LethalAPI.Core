// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Features;

// ReSharper disable InconsistentNaming
#pragma warning disable SA1402 // file may only contain a single type

using System;
using System.Reflection;

using Interfaces;
using Loader.Configs;

/// <summary>
/// Creates a new instance of a plugin.
/// </summary>
/// <typeparam name="TConfig">The config type for the plugin.</typeparam>
public abstract class Plugin<TConfig> : IPlugin<TConfig>
    where TConfig : IConfig, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin{TConfig}"/> class.
    /// </summary>
    public Plugin()
    {
        this.Assembly = Assembly.GetCallingAssembly();
    }

    /// <inheritdoc />
    /// <remarks>
    /// DO NOT OVERRIDE THIS UNLESS YOU KNOW WHAT YOU ARE DOING.
    /// It should only be utilized for extremely specific implementations.
    /// </remarks>
    public virtual TConfig Config { get; set; } = new();

    /// <inheritdoc />
    public Assembly Assembly { get; init; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public abstract string Author { get; }

    /// <inheritdoc />
    public abstract Version Version { get; }

    /// <inheritdoc />
    public virtual Version RequiredAPIVersion { get; } = new(1, 0, 0);

    /// <inheritdoc/>
    public void UpdateConfig(object newConfig)
    {
        Config.CopyProperties(newConfig);
    }

    /// <inheritdoc />
    public abstract void OnEnabled();

    /// <inheritdoc />
    public virtual void OnDisabled()
    {
    }

    /// <inheritdoc />
    public virtual void OnReloaded()
    {
    }
}

/// <summary>
/// An implementation for creating a plugin via attributes.
/// </summary>
/// <typeparam name="TPlugin">The type of the class..</typeparam>
/// <typeparam name="TConfig">The type of the config.</typeparam>
internal sealed class AttributePlugin<TPlugin, TConfig> : Plugin<TConfig>
#pragma warning restore SA1402
    where TConfig : IConfig, new()
{
    private readonly string nameValue;
    private readonly string authorValue;
    private readonly string descriptionValue;
    private readonly Version versionValue;
    private readonly Action onEnable;
    private readonly FieldInfo configField;

    private readonly Version requiredAPIVersionValue;
    private readonly Action? onDisable;
    private readonly Action? onReload;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributePlugin{TPlugin,TConfig}"/> class.
    /// </summary>
    /// <param name="instance">The instance of the class..</param>
    /// <param name="name">The name of the plugin.</param>
    /// <param name="description">A description of what the plugin is and what it does.</param>
    /// <param name="author">The name(s) of the author(s).</param>
    /// <param name="version">The version of the plugin being run.</param>
    /// <param name="configField">The field that represents the config.</param>
    /// <param name="requiredAPIVersion">The optional required framework version for the plugin to run.</param>
    /// <param name="onEnabled">The action that will be called when the plugin is enabled.</param>
    /// <param name="onDisabled">The action that will be called when the plugin is disabled.</param>
    /// <param name="onReloaded">The action that will be called when the plugin is reloaded.</param>
    public AttributePlugin(object instance, string name, string description, string author, Version version, Action onEnabled, FieldInfo configField, Version? requiredAPIVersion = null, Action? onDisabled = null, Action? onReloaded = null)
    {
        this.Instance = (TPlugin)instance;
        this.nameValue = name;
        this.descriptionValue = description;
        this.authorValue = author;
        this.versionValue = version;
        this.onEnable = onEnabled;
        this.configField = configField;
        this.requiredAPIVersionValue = requiredAPIVersion ?? new Version(1, 0, 0);
        this.onReload = onReloaded;
        this.onDisable = onDisabled;
        this.Config = new TConfig();
        this.Assembly = typeof(TPlugin).Assembly;
    }

    /// <summary>
    /// Gets the main instance of the class.
    /// </summary>
    public TPlugin Instance { get; init; }

    /// <inheritdoc/>
    public override TConfig Config
    {
        get => (TConfig)this.configField.GetValue(this.Instance);
        set => this.configField.SetValue(this.Instance, value);
    }

    /// <inheritdoc/>
    public override string Name => this.nameValue;

    /// <inheritdoc/>
    public override string Author => this.authorValue;

    /// <inheritdoc/>
    public override string Description => this.descriptionValue;

    /// <inheritdoc/>
    public override Version Version => this.versionValue;

    /// <inheritdoc/>
    public override Version RequiredAPIVersion => this.requiredAPIVersionValue;

    /// <inheritdoc/>
    public override void OnEnabled()
    {
        onEnable.Invoke();
    }

    /// <inheritdoc/>
    public override void OnDisabled()
    {
        onDisable?.Invoke();
        base.OnDisabled();
    }

    /// <inheritdoc/>
    public override void OnReloaded()
    {
        onReload?.Invoke();
        base.OnReloaded();
    }
}