// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Features;

// ReSharper disable InconsistentNaming.
#pragma warning disable SA1402 // file may only contain a single type

using System;
using System.Reflection;

using BepInEx;
using Interfaces;
using Loader.Configs;
using ModData.Internal;

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
        this.RootInstance ??= this;
        this.Assembly = Assembly.GetCallingAssembly();
        this.LocalSaveHandler = SaveHandler.GetSaveHandler((IPlugin<IConfig>)this);
        this.GlobalSaveHandler = SaveHandler.GetSaveHandler((IPlugin<IConfig>)this, true);

        // ReSharper disable once VirtualMemberCallInConstructor
        Log.Debug($"&3{this.Name}&r Local Handler: {this.LocalSaveHandler.GetType().Name}, Global Handler: {this.GlobalSaveHandler.GetType().Name}");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin{TConfig}"/> class.
    /// </summary>
    /// <param name="instance">The instance of the plugin to use as the <see cref="RootInstance"/>.</param>
    internal Plugin(object instance)
    {
        this.RootInstance = instance;
        this.Assembly = instance.GetType().Assembly;
        this.LocalSaveHandler = SaveHandler.GetSaveHandler((IPlugin<IConfig>)this);
        this.GlobalSaveHandler = SaveHandler.GetSaveHandler((IPlugin<IConfig>)this, true);

        // ReSharper disable once VirtualMemberCallInConstructor
        Log.Debug($"&3{this.Name}&r Local Handler: {this.LocalSaveHandler.GetType().Name}, Global Handler: {this.GlobalSaveHandler.GetType().Name}");
    }

    /// <inheritdoc />
    /// <remarks>
    /// DO NOT OVERRIDE THIS UNLESS YOU KNOW WHAT YOU ARE DOING.
    /// It should only be utilized for extremely specific implementations.
    /// </remarks>
    public virtual TConfig Config { get; set; } = new();

    /// <inheritdoc cref="IPlugin{TConfig}.Assembly" />
    public Assembly Assembly { get; init; }

    /// <inheritdoc cref="IPlugin{TConfig}.Name" />
    public abstract string Name { get; }

    /// <inheritdoc cref="IPlugin{TConfig}.Description" />
    public abstract string Description { get; }

    /// <inheritdoc cref="IPlugin{TConfig}.Author" />
    public abstract string Author { get; }

    /// <inheritdoc cref="IPlugin{TConfig}.Version" />
    public abstract Version Version { get; }

    /// <inheritdoc cref="IPlugin{TConfig}.RequiredAPIVersion" />
    public virtual Version RequiredAPIVersion { get; } = new(1, 0, 0);

    /// <inheritdoc cref="IPlugin{TConfig}.LocalSaveHandler" />
    public SaveHandler LocalSaveHandler { get; set; }

    /// <inheritdoc cref="IPlugin{TConfig}.GlobalSaveHandler" />
    public SaveHandler GlobalSaveHandler { get; set; }

    /// <inheritdoc cref="IPlugin{TConfig}.RootInstance" />
    public object RootInstance { get; init; }

    /// <inheritdoc cref="IPlugin{TConfig}.UpdateConfig" />
    public void UpdateConfig(object newConfig)
    {
        Config.CopyProperties(newConfig);
    }

    /// <inheritdoc cref="IPlugin{TConfig}.OnEnabled" />
    public abstract void OnEnabled();

    /// <inheritdoc cref="IPlugin{TConfig}.OnDisabled" />
    public virtual void OnDisabled()
    {
    }

    /// <inheritdoc cref="IPlugin{TConfig}.OnReloaded" />
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
    where TConfig : IConfig, new()
{
    private readonly Action onEnable;
    private readonly FieldInfo configField;

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
    internal AttributePlugin(object instance, string name, string description, string author, Version version, Action onEnabled, FieldInfo configField, Version? requiredAPIVersion = null, Action? onDisabled = null, Action? onReloaded = null)
        : base(instance)
    {
        this.Instance = (TPlugin)instance;
        this.Name = name;
        this.Description = description;
        this.Author = author;
        this.Version = version;
        this.onEnable = onEnabled;
        this.configField = configField;
        this.RequiredAPIVersion = requiredAPIVersion ?? new Version(1, 0, 0);
        this.onReload = onReloaded;
        this.onDisable = onDisabled;
        this.Config = new TConfig();
        this.Assembly = typeof(TPlugin).Assembly;
        this.RootInstance = this.Instance;
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
    public override string Name { get; }

    /// <inheritdoc/>
    public override string Author { get; }

    /// <inheritdoc/>
    public override string Description { get; }

    /// <inheritdoc/>
    public override Version Version { get; }

    /// <inheritdoc/>
    public override Version RequiredAPIVersion { get; }

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

/// <summary>
/// Contains the abstractions for BepInEx plugins.
/// </summary>
internal sealed class BepInExPlugin : IPlugin<IConfig>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BepInExPlugin"/> class.
    /// </summary>
    /// <param name="info">The BepInEx plugin info.</param>
    internal BepInExPlugin(PluginInfo info)
    {
        this.RootInstance = info.Instance;
        this.Assembly = info.Instance.GetType().Assembly;
        this.Name = info.Metadata.Name;
        this.Version = info.Metadata.Version;
        this.Description = "Unknown";
        this.Author = "Unknown";
        this.Config = new DefaultConfig();
        this.LocalSaveHandler = SaveHandler.GetSaveHandler(this);
        this.GlobalSaveHandler = SaveHandler.GetSaveHandler(this, true);
    }

    /// <inheritdoc/>
    public IConfig Config { get; }

    /// <inheritdoc/>
    public Assembly Assembly { get; init; }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string Description { get; }

    /// <inheritdoc/>
    public string Author { get; }

    /// <inheritdoc/>
    public Version Version { get; }

    /// <inheritdoc/>
    public Version RequiredAPIVersion => new(1, 0, 0);

    /// <inheritdoc/>
    public SaveHandler LocalSaveHandler { get; set; }

    /// <inheritdoc/>
    public SaveHandler GlobalSaveHandler { get; set; }

    /// <inheritdoc/>
    public object RootInstance { get; init; }

    /// <inheritdoc/>
    public void UpdateConfig(object newConfig)
    {
    }

    /// <inheritdoc/>
    public void OnEnabled()
    {
    }

    /// <inheritdoc/>
    public void OnDisabled()
    {
    }

    /// <inheritdoc/>
    public void OnReloaded()
    {
    }
}