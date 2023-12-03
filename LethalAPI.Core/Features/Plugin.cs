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
using System.Linq;
using System.Reflection;

using Attributes;
using Interfaces;
using Loader.Configs;
using MelonLoader;
using Models;

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
        this.RootInstance = this;
        this.Assembly = Assembly.GetCallingAssembly();

        // ReSharper disable VirtualMemberCallInConstructor
        bool isRequired = this.IsRequired || this.RootInstance.GetType().GetCustomAttributes<LethalRequiredPluginAttribute>().Any();
        this.Info = new PluginInfoRecord(this.Name, this.Version, isRequired);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin{TConfig}"/> class.
    /// </summary>
    /// <param name="rootInstance">The root instance of the plugin.</param>
    protected Plugin(object rootInstance)
    {
        this.RootInstance = rootInstance;
        this.Assembly = rootInstance.GetType().Assembly;
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
    public object RootInstance { get; }

    /// <inheritdoc />
    public virtual PluginInfoRecord Info { get; } = null!;

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

    /// <summary>
    /// Gets a value indicating whether or not the plugin is required for everyone in the lobby.
    /// </summary>
    public virtual bool IsRequired => false;

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
    public AttributePlugin(object instance, string name, string description, string author, Version version, Action onEnabled, FieldInfo configField, Version? requiredAPIVersion = null, Action? onDisabled = null, Action? onReloaded = null)
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
        bool isRequired = this.IsRequired || this.RootInstance.GetType().GetCustomAttributes<LethalRequiredPluginAttribute>().Any();
        this.Info = new PluginInfoRecord(this.Name, this.Version, isRequired);
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
    public override PluginInfoRecord Info { get; }

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
/// An implementation of plugins for BepInEx or MelonLoader.
/// </summary>
internal sealed class ExternalPlugin : IPlugin<IConfig>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalPlugin"/> class.
    /// </summary>
    /// <param name="pluginInfo">The plugin instance.</param>
    internal ExternalPlugin(BepInEx.PluginInfo pluginInfo)
    {
        this.RootInstance = pluginInfo.Instance;
        this.Assembly = pluginInfo.Instance.GetType().Assembly;
        this.Name = pluginInfo.Metadata.Name;
        this.Version = pluginInfo.Metadata.Version;
        this.Config = new DefaultConfig();
        this.Author = "Unknown";
        bool isRequired = this.RootInstance.GetType().GetCustomAttributes<LethalRequiredPluginAttribute>().Any();
        this.Info = new PluginInfoRecord(this.Name, this.Version, isRequired);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalPlugin"/> class.
    /// </summary>
    /// <param name="mod">The mod instance.</param>
    internal ExternalPlugin(MelonMod mod)
    {
        this.RootInstance = mod;
        this.Assembly = mod.MelonAssembly.Assembly;
        this.Author = mod.Info.Author;
        this.Name = mod.Info.Name;
        this.Config = new DefaultConfig();
        this.Version = new Version(mod.Info.SemanticVersion.Major, mod.Info.SemanticVersion.Minor, mod.Info.SemanticVersion.Patch);
        bool isRequired = this.RootInstance.GetType().GetCustomAttributes<LethalRequiredPluginAttribute>().Any();
        this.Info = new PluginInfoRecord(this.Name, this.Version, isRequired);
    }

    /// <inheritdoc />
    public IConfig Config { get; }

    /// <inheritdoc />
    public Assembly Assembly { get; init; }

    /// <inheritdoc />
    public object RootInstance { get; }

    /// <inheritdoc />
    public PluginInfoRecord Info { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description => "Unknown";

    /// <inheritdoc />
    public string Author { get; }

    /// <inheritdoc />
    public Version Version { get; }

    /// <inheritdoc />
    public Version RequiredAPIVersion => new (0, 0, 0);

    /// <inheritdoc />
    public void UpdateConfig(object newConfig)
    {
    }

    /// <inheritdoc />
    public void OnEnabled()
    {
    }

    /// <inheritdoc />
    public void OnDisabled()
    {
    }

    /// <inheritdoc />
    public void OnReloaded()
    {
    }
}

/// <summary>
/// Gets a default implementation for a config.
/// </summary>
internal class DefaultConfig : IConfig
{
    /// <inheritdoc />
    public bool IsEnabled { get; set; } = true;

    /// <inheritdoc />
    public bool Debug { get; set; } = false;
}