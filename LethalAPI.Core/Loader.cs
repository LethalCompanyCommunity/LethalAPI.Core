// -----------------------------------------------------------------------
// <copyright file="Loader.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments. Loader has been adjusted to use our custom plugin types.
// It also now includes the attribute based plugin loading.
// -----------------------------------------------------------------------

namespace LethalAPI.Core;

#pragma warning disable SA1401 // field should be private
#pragma warning disable SA1202 // public before private fields - methods

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

using Attributes;
using Features;
using Interfaces;

/// <summary>
/// Loads plugins.
/// </summary>
public sealed class Loader
{
    /// <summary>
    /// Gets the main instance of the <see cref="Loader"/>.
    /// </summary>
    public static Loader Singleton = null!;

    private static readonly Dictionary<string, IPlugin<IConfig>> PluginsValue = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Loader"/> class.
    /// </summary>
    public Loader()
    {
        Singleton = this;
        try
        {
            LoadDependencies();
            LoadPlugins();
        }
        catch (Exception e)
        {
            Log.Error($"An error has occured while loading plugins.");
            Log.Debug($"{e}");
        }
    }

    /// <summary>
    /// Gets the base directory to load plugins from.
    /// </summary>
    public static string BaseDirectory { get; } = string.Empty;

    /// <summary>
    /// Gets the base directory of the dependency folder.
    /// </summary>
    public static string DependencyDirectory { get; } = string.Empty;

    /// <summary>
    /// Gets a dictionary containing the file paths of assemblies.
    /// </summary>
    public static Dictionary<Assembly, string> Locations { get; } = new();

    /// <summary>
    /// Gets the initialized global random class.
    /// </summary>
    public static Random Random { get; } = new();

    /// <summary>
    /// Gets the version of the assembly.
    /// </summary>
    public static Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

    /// <summary>
    /// Gets plugin dependencies.
    /// </summary>
    // ReSharper disable once CollectionNeverQueried.Global
    public static List<Assembly> Dependencies { get; } = new();

    /// <summary>
    /// Gets a dictionary of all registered plugins, with the key being the plugin name.
    /// </summary>
    public static ReadOnlyDictionary<string, IPlugin<IConfig>> Plugins => new(PluginsValue);

    /// <summary>
    /// Loads all plugins.
    /// </summary>
    public static void LoadPlugins()
    {
        foreach (string assemblyPath in Directory.GetFiles(BaseDirectory, "*.dll"))
        {
            Assembly? assembly = LoadAssembly(assemblyPath);

            if (assembly is null)
                continue;

            Locations[assembly] = assemblyPath;
        }

        foreach (Assembly assembly in Locations.Keys)
        {
            if (Locations[assembly].Contains("dependencies"))
                continue;

            IPlugin<IConfig>? plugin = CreatePlugin(assembly);

            plugin ??= CreatePluginFromAttributes(assembly);

            if (plugin is null)
                continue;

            AssemblyInformationalVersionAttribute attribute =
                plugin.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            Log.Info($"Loaded plugin {plugin.Name}@{(plugin.Version is not null ? $"{plugin.Version.Major}.{plugin.Version.Minor}.{plugin.Version.Build}" : attribute is not null ? attribute.InformationalVersion : string.Empty)}");
            PluginsValue.Add(plugin.Name, plugin);
        }
    }

    /// <summary>
    /// Loads an assembly.
    /// </summary>
    /// <param name="path">The path to load the assembly from.</param>
    /// <returns>Returns the loaded assembly or <see langword="null"/>.</returns>
    public static Assembly? LoadAssembly(string path)
    {
        try
        {
            Assembly assembly = Assembly.Load(File.ReadAllBytes(path));

            ResolveAssemblyEmbeddedResources(assembly);

            return assembly;
        }
        catch (Exception exception)
        {
            Log.Error($"Error while loading an assembly at {path}! {exception}");
        }

        return null;
    }

    /// <summary>
    /// Create a plugin instance.
    /// </summary>
    /// <param name="assembly">The plugin assembly.</param>
    /// <returns>Returns the created plugin instance or <see langword="null"/>.</returns>
    public static IPlugin<IConfig>? CreatePlugin(Assembly assembly)
    {
        try
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    Log.Debug($"\"{type.FullName}\" is an interface or abstract class, skipping.");
                    continue;
                }

                if (!IsDerivedFromPlugin(type))
                {
                    Log.Debug($"\"{type.FullName}\" does not inherit from Plugin<TConfig>, skipping.");
                    continue;
                }

                Log.Debug($"Loading type {type.FullName}");

                IPlugin<IConfig>? plugin = null;

                ConstructorInfo? constructor = type.GetConstructor(new[] { typeof(Assembly) }) ?? type.GetConstructor(Type.EmptyTypes);
                if (constructor is not null)
                {
                    Log.Debug("Public default constructor found, creating instance...");

                    plugin = constructor.Invoke(null) as IPlugin<IConfig>;
                }
                else
                {
                    Log.Debug($"Constructor wasn't found, searching for a property with the {type.FullName} type...");

                    object? value = Array
                        .Find(
                            type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public),
                            property => property.PropertyType == type)?.GetValue(null);

                    if (value is not null)
                        plugin = value as IPlugin<IConfig>;
                }

                if (plugin is null)
                {
                    Log.Error($"{type.FullName} is a valid plugin, but it cannot be instantiated! It either doesn't have a public default constructor without any arguments or a static property of the {type.FullName} type!");

                    continue;
                }

                Log.Debug($"Instantiated type {type.FullName}");

                if (CheckPluginRequiredAPIVersion(plugin))
                    continue;

                return plugin;
            }
        }
        catch (ReflectionTypeLoadException reflectionTypeLoadException)
        {
            Log.Error(
                $"Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {reflectionTypeLoadException}");

            foreach (Exception loaderException in reflectionTypeLoadException.LoaderExceptions)
            {
                Log.Error(loaderException);
            }
        }
        catch (Exception exception)
        {
            Log.Error(
                $"Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {exception}");
        }

        return null;
    }

    private static IPlugin<IConfig>? CreatePluginFromAttributes(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            LethalPluginAttribute? pluginInfo = type.GetCustomAttribute<LethalPluginAttribute>();
            if (pluginInfo is null)
            {
                continue;
            }

            Version? version = pluginInfo.Version;
            if (version is null && type.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion is { } versionString)
            {
                if (!Version.TryParse(versionString, out version))
                {
                    Log.Error($"Plugin '{type.FullName}' does not have a valid version. Please define a version in the plugin attribute.\n" +
                              "The assembly version string must be a parsable version to be used for plugin versioning. https://semver.org/");
                    continue;
                }
            }

            if(version is null)
            {
                Log.Error($"Plugin '{type.FullName}' must have a valid version. This can be manually defined in the attribute, or defined in the assembly before being built.");
                continue;
            }

            FieldInfo? configField = null;
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.GetCustomAttribute<LethalConfigAttribute>() is null && field.Name != "Config")
                {
                    continue;
                }

                if (!field.FieldType.IsSubclassOf(typeof(IConfig)))
                {
                    Log.Warn($"Found a config for plugin '{type.FullName}', but it wasn't a IConfig, therefore it was skipped.");
                    continue;
                }

                configField = field;
                break;
            }

            if (configField is null)
            {
                Log.Warn($"Couldn't find a valid config for plugin '{type.FullName}'. Please include a field that has a type inheriting the IConfig interface. Either use the [LethalConfig] attribute on the field, or name the field 'Config'.");
                continue;
            }

            Action? enableHandler = null;
            Action? disableHandler = null;
            Action? reloadHandler = null;
            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (methodInfo.GetParameters().Length > 0)
                    continue;

                if (enableHandler is null && (methodInfo.GetCustomAttribute<LethalEntrypointAttribute>() is not null || methodInfo.Name == "OnEnabled"))
                {
                    enableHandler = (Action)Delegate.CreateDelegate(typeof(Action), methodInfo);
                    continue;
                }

                if (disableHandler is null && (methodInfo.GetCustomAttribute<LethalDisableHandlerAttribute>() is not null || methodInfo.Name == "OnDisabled"))
                {
                    disableHandler = (Action)Delegate.CreateDelegate(typeof(Action), methodInfo);
                    continue;
                }

                if (reloadHandler is null && (methodInfo.GetCustomAttribute<LethalReloadHandlerAttribute>() is not null || methodInfo.Name == "OnReloaded"))
                    reloadHandler = (Action)Delegate.CreateDelegate(typeof(Action), methodInfo);
            }

            if (enableHandler is null)
            {
                Log.Debug($"Plugin '{type.FullName}' could not be registered because there wasn't an entrypoint method. Try adding the [LethalEntrypoint] attribute, or naming a method OnEnabled." +
                          $"Also ensure that the method has no arguments.");
                continue;
            }

            Version requiredApiVersion = type.GetCustomAttribute<LethalRequiredFrameworkVersionAttribute>()?.Version ?? new Version(1, 0, 0);

            try
            {
                object? x = typeof(AttributePlugin<>).MakeGenericType(configField.FieldType).GetConstructors()[0]
                    .Invoke(new object[]
                    {
                        pluginInfo.Name,
                        pluginInfo.Description,
                        pluginInfo.Author,
                        version,
                        enableHandler,
                        requiredApiVersion,
                        disableHandler!,
                        reloadHandler!,
                    });
                return (IPlugin<IConfig>)x;
            }
            catch (Exception e)
            {
                Log.Error($"Could not initialize plugin \'{type.FullName}\' due to an error.");
                Log.Debug($"{e}");
            }
        }

        return null;
    }

    /// <summary>
    /// Enables all plugins.
    /// </summary>
    public static void EnablePlugins()
    {
        List<IPlugin<IConfig>> toLoad = Plugins.Values.ToList();

        foreach (IPlugin<IConfig> plugin in toLoad.ToList())
        {
            try
            {
                if (plugin.Name.StartsWith("LethalAPI") && plugin.Config.IsEnabled)
                {
                    plugin.OnEnabled();
                    toLoad.Remove(plugin);
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Plugin \"{plugin.Name}\" threw an exception while enabling: {exception}");
            }
        }

        foreach (IPlugin<IConfig> plugin in toLoad)
        {
            try
            {
                if (plugin.Config.IsEnabled)
                {
                    plugin.OnEnabled();
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Plugin \"{plugin.Name}\" threw an exception while enabling: {exception}");
            }
        }
    }

    /// <summary>
    /// Reloads all plugins.
    /// </summary>
    public static void ReloadPlugins()
    {
        foreach (IPlugin<IConfig> plugin in Plugins.Values)
        {
            try
            {
                plugin.OnReloaded();

                plugin.Config.IsEnabled = false;

                plugin.OnDisabled();
            }
            catch (Exception exception)
            {
                Log.Error($"Plugin \"{plugin.Name}\" threw an exception while reloading: {exception}");
            }
        }

        PluginsValue.Clear();
        Locations.Clear();

        LoadPlugins();

        EnablePlugins();
    }

    /// <summary>
    /// Disables all plugins.
    /// </summary>
    public static void DisablePlugins()
    {
        foreach (IPlugin<IConfig> plugin in Plugins.Values)
        {
            try
            {
                plugin.OnDisabled();
            }
            catch (Exception exception)
            {
                Log.Error($"Plugin \"{plugin.Name}\" threw an exception while disabling: {exception}");
            }
        }
    }

    /// <summary>
    /// Attempts to load Embedded (compressed) assemblies from specified Assembly.
    /// </summary>
    /// <param name="target">Assembly to check for embedded assemblies.</param>
    private static void ResolveAssemblyEmbeddedResources(Assembly target)
    {
        try
        {
            Log.Debug($"Attempting to load embedded resources for {target.FullName}");

            string[] resourceNames = target.GetManifestResourceNames();

            foreach (string name in resourceNames)
            {
                Log.Debug($"Found resource {name}");

                if (name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    using MemoryStream stream = new();

                    Log.Debug($"Loading resource {name}");

                    Stream? dataStream = target.GetManifestResourceStream(name);

                    if (dataStream == null)
                    {
                        Log.Error($"Unable to resolve resource {name} Stream was null");
                        continue;
                    }

                    dataStream.CopyTo(stream);

                    Dependencies.Add(Assembly.Load(stream.ToArray()));

                    Log.Debug($"Loaded resource {name}");
                }
                else if (name.EndsWith(".dll.compressed", StringComparison.OrdinalIgnoreCase))
                {
                    Stream? dataStream = target.GetManifestResourceStream(name);

                    if (dataStream == null)
                    {
                        Log.Error($"Unable to resolve resource {name} Stream was null");
                        continue;
                    }

                    using DeflateStream stream = new(dataStream, CompressionMode.Decompress);
                    using MemoryStream memStream = new();

                    Log.Debug($"Loading resource {name}");

                    stream.CopyTo(memStream);

                    Dependencies.Add(Assembly.Load(memStream.ToArray()));

                    Log.Debug($"Loaded resource {name}");
                }
            }
        }
        catch (Exception exception)
        {
            Log.Error($"Failed to load embedded resources from {target.FullName}: {exception}");
        }
    }

    /// <summary>
    /// Gets a plugin with its prefix or name.
    /// </summary>
    /// <param name="args">The name or prefix of the plugin (Using the prefix is recommended).</param>
    /// <returns>The desired plugin, null if not found.</returns>
    public static IPlugin<IConfig>? GetPlugin(string args) =>
        Plugins.ContainsKey(args) ? Plugins[args] : null;

    /// <summary>
    /// Gets a plugin with its type.
    /// </summary>
    /// <typeparam name="TPlugin">The plugin's type.</typeparam>
    /// <returns>The desired plugin, null if not found.</returns>
    public static IPlugin<IConfig>? GetPlugin<TPlugin>()
        where TPlugin : IPlugin<IConfig> =>
        Plugins.FirstOrDefault(x => x.Value.GetType() == typeof(TPlugin)).Value;

    /// <summary>
    /// Loads all dependencies.
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    private static void LoadDependencies()
    {
        try
        {
            Log.Info($"Loading dependencies at {DependencyDirectory}");

            foreach (string dependency in Directory.GetFiles(DependencyDirectory, "*.dll"))
            {
                Assembly? assembly = LoadAssembly(dependency);

                if (assembly is null)
                    continue;

                Locations[assembly] = dependency;

                Dependencies.Add(assembly);

                Log.Info($"Loaded dependency {assembly.GetName().Name}@{assembly.GetName().Version.ToString(3)}");
            }

            Log.Info("Dependencies loaded successfully!");
        }
        catch (Exception exception)
        {
            Log.Error($"An error has occurred while loading dependencies! {exception}");
        }
    }

    /// <summary>
    /// Indicates that the passed type is derived from the plugin type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <returns><see langword="true"/> if passed type is derived from <see cref="Plugin{TConfig}"/> or <see cref="Plugin"/>, otherwise <see langword="false"/>.</returns>
    private static bool IsDerivedFromPlugin(Type type)
    {
        while (type?.BaseType is not null)
        {
            type = type.BaseType;

            if (type is { IsGenericType: true })
            {
                Type genericTypeDef = type.GetGenericTypeDefinition();

                if (genericTypeDef == typeof(Plugin<>))
                    return true;
            }
        }

        return false;
    }

    private static bool CheckPluginRequiredAPIVersion(IPlugin<IConfig> plugin)
    {
        Version requiredVersion = plugin.RequiredAPIVersion;
        Version actualVersion = Version;

        // Check Major version
        // It's increased when an incompatible API change was made
        if (requiredVersion.Major != actualVersion.Major)
        {
            // Assume that if the Required Major version is greater than the Actual Major version,
            // LethalAPI is outdated
            if (requiredVersion.Major > actualVersion.Major)
            {
                Log.Error(
                    $"You're running an older version of LethalAPI ({Version.ToString(3)})! {plugin.Name} won't be loaded! " +
                    $"Required version to load it: {plugin.RequiredAPIVersion.ToString(3)}");

                return true;
            }

            if (requiredVersion.Major < actualVersion.Major)
            {
                Log.Error(
                    $"You're running an older version of {plugin.Name} ({plugin.Version.ToString(3)})! " +
                    $"Its Required Major version is {requiredVersion.Major}, but the actual version is: {actualVersion.Major}. This plugin will not be loaded!");

                return true;
            }
        }

        return false;
    }
}