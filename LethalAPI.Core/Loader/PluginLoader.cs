// -----------------------------------------------------------------------
// <copyright file="PluginLoader.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments. Loader has been adjusted to use our custom plugin types.
// It also now includes the attribute based plugin loading.
// Some new features have also been added for better plugin loading.
// -----------------------------------------------------------------------

// ReSharper disable MemberCanBePrivate.Global
namespace LethalAPI.Core.Loader;

#pragma warning disable SA1401 // field should be private
#pragma warning disable SA1202 // public before private fields - methods
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Attributes;
using Configs;
using Features;
using Interfaces;
using MonoMod.RuntimeDetour;
using Patches.Fixes;
using Resources;

/// <summary>
/// Loads plugins.
/// </summary>
public sealed class PluginLoader
{
    /// <summary>
    /// Gets the main instance of the <see cref="PluginLoader"/>.
    /// </summary>
    public static PluginLoader Singleton = null!;

    private static readonly Dictionary<string, IPlugin<IConfig>> PluginsValue = new();

    private static readonly bool ShowDebug = true;

    private static readonly bool ShowPluginFinderDebug = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLoader"/> class.
    /// </summary>
    /// <param name="loadType">The type of loading method that will be used.</param>
    public PluginLoader(LoadMethod loadType)
    {
        LoaderType = loadType;
        Log.Raw("[LethalAPI-Loader] Initializing Loader.");
        Singleton = this;
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetName().Name.ToLower().Contains("melonloader"))
                MelonLoaderFound = true;

            if(assembly.GetName().Name.ToLower().Contains("bepinex"))
                BepInExFound = true;

            if (BepInExFound && MelonLoaderFound)
                break;
        }

        // Hooks and fixes the exception stacktrace il.
        _ = new ILHook(typeof(StackTrace).GetMethod("AddFrames", BindingFlags.Instance | BindingFlags.NonPublic), FixExceptionIL.IlHook);

        // Ensure that these are registered by loading the reference.
        _ = new UnknownResourceParser();
        _ = new DllParser();

        // Instance is stored in the type.
        _ = new EmbeddedResourceLoader();

        if(!Directory.Exists(PluginDirectory))
            Directory.CreateDirectory(PluginDirectory);

        if(!Directory.Exists(DependencyDirectory))
            Directory.CreateDirectory(DependencyDirectory);

        if(!Directory.Exists(ConfigDirectory))
            Directory.CreateDirectory(ConfigDirectory);

        ConfigLoader.ConfigDirectory = ConfigDirectory;

        try
        {
            LoadDependencies();
            LoadPlugins();
            try
            {
                ConfigLoader.LoadAllConfigs();
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }

            EnablePlugins();
        }
        catch (Exception e)
        {
            Log.Error($"An error has occured while loading plugins.");
            Log.Debug($"{e}");
        }
    }

    /// <summary>
    /// Gets the loader type for the bootstrap.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static LoadMethod LoaderType { get; private set; }

    /// <summary>
    /// Gets a value indicating whether or not BepInEx is found.
    /// </summary>
    public static bool BepInExFound { get; private set; }

    /// <summary>
    /// Gets a value indicating whether or not MelonLoader is found.
    /// </summary>
    public static bool MelonLoaderFound { get; private set; }

    /// <summary>
    /// Gets or sets the base directory to load plugins from.
    /// </summary>
    public static string PluginDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base directory of the dependency folder.
    /// </summary>
    public static string DependencyDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base directory for configs.
    /// </summary>
    public static string ConfigDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets a dictionary containing the file paths of assemblies.
    /// </summary>
    public static Dictionary<Assembly, string> Locations { get; private set; } = new();

    /// <summary>
    /// Gets the version of the assembly.
    /// </summary>
    public static Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

    /// <summary>
    /// Gets plugin dependencies.
    /// </summary>
    // ReSharper disable once CollectionNeverQueried.Global
    public static List<Assembly> Dependencies { get; private set; } = new();

    /// <summary>
    /// Gets a dictionary of all registered plugins, with the key being the plugin name.
    /// </summary>
    public static ReadOnlyDictionary<string, IPlugin<IConfig>> Plugins => new(PluginsValue);

    /// <summary>
    /// Triggers the fix for the BepInEx Logger.
    /// </summary>
    /// <param name="harmony">The harmony instance to use to log.</param>
    public static void FixLoggingBepInEx(HarmonyLib.Harmony harmony) => FixBepInExLoggerPrefix.Patch(harmony);

    /// <summary>
    /// Enables all plugins.
    /// </summary>
    public static void EnablePlugins()
    {
        Log.Debug("Enabling plugins", ShowDebug);
        List<IPlugin<IConfig>> toLoad = Plugins.Values.ToList();

        foreach (IPlugin<IConfig> plugin in toLoad.ToList())
        {
            try
            {
                if (plugin.Name.ToLower().StartsWith("lethalapi") && plugin.Config.IsEnabled)
                {
                    plugin.OnEnabled();
                    toLoad.Remove(plugin);
                    Log.Info($"Successfully Enabled LethalAPI Core Feature &3{plugin.Name} &gv{plugin.Version}&7, by &6{plugin.Author}&7");
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Plugin \"{plugin.Name}\" threw an exception while enabling: \n{exception}");
            }
        }

        foreach (IPlugin<IConfig> plugin in toLoad)
        {
            try
            {
                Log.Debug($"LethalPlugin Loaded. {plugin.Name}", ShowDebug);
                if (plugin.Config.IsEnabled)
                {
                    plugin.OnEnabled();
                    Log.Info($"Successfully Enabled Plugin &3{plugin.Name} &gv{plugin.Version}&7, by &6{plugin.Author}&7");
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Plugin \"{plugin.Name}\" threw an exception while enabling: \n{exception}");
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
    /// Loads all plugins.
    /// </summary>
    public static void LoadPlugins()
    {
        Log.Debug("Now loading plugins.", ShowDebug);
        foreach (string assemblyPath in Directory.GetFiles(PluginDirectory, "*.dll"))
        {
            Assembly? assembly = LoadAssembly(assemblyPath);

            if (assembly is null)
                continue;

            Locations[assembly] = assemblyPath;
        }

        // This is where we ensure we load the highest version dependencies first.
        Log.Debug($"Loading {ResourceParser.CachedResources[DllParser.Instance.ExtensionName].Count()} cached assemblies.");

        // Now load the higher priority assemblies first.
        foreach (EmbeddedResourceData highPriorityAssembly in ResourceParser.CachedResources[DllParser.Instance.ExtensionName])
        {
            try
            {
                Assembly assembly = (Assembly)highPriorityAssembly.Parser?.Parse(highPriorityAssembly.GetStream())!;
                Dependencies.Add(assembly);
                Locations[assembly] = highPriorityAssembly.FileLocation;
                Log.Info($"Loaded &fEmbedded Dependency &h'&3{assembly.GetName().Name}&h'&7@&gv{assembly.GetName().Version.ToString(3)}&7", "LethalAPI-Loader");
            }
            catch (Exception e)
            {
                Log.Warn($"An error has occured while loading embedded dependency '{highPriorityAssembly}'.", "LethalAPI-Loader");
                Log.Debug($"Exception: \n{e}", ShowDebug, "LethalAPI-Loader");
            }
        }

        StringBuilder builder = new StringBuilder().AppendLine("Dependencies: ");
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (Dependencies.Contains(assembly))
                builder.AppendLine($" - &h'&3{assembly.GetName().Name}&h'&7@&gv{assembly.GetName().Version.ToString(3)}&h [{assembly.FullName}]&7");
        }

        Log.Debug(builder.ToString(), EmbeddedResourceLoader.Debug);

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (Locations.ContainsKey(assembly))
                continue;

            Locations.Add(assembly, assembly.Location);
        }

        foreach (Assembly assembly in Locations.Keys)
        {
            // skip dependencies.
            if (Locations[assembly].ToLower().Contains("dependencies"))
                continue;

            IPlugin<IConfig>? plugin = CreatePlugin(assembly);

            plugin ??= CreatePluginFromAttributes(assembly);

            if (plugin is null)
                continue;

            if (PluginsValue.ContainsKey(plugin.Name))
            {
                Log.Warn($"Tried to load two plugins with the same name! This is not allowed! Plugin: '{plugin.Name}'");
                continue;
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            string debug = ShowDebug ? $"[{plugin.Assembly.GetName().Name}]" : string.Empty;
            Log.Info($"Loaded plugin '&3{plugin.Name}' &7@ &6v{plugin.Version.Major}.{plugin.Version.Minor}.{plugin.Version.Build} &r{debug}");
            PluginsValue.Add(plugin.Name, plugin);
        }
    }

    /// <summary>
    /// Loads an assembly.
    /// </summary>
    /// <param name="path">The path to load the assembly from.</param>
    /// <returns>Returns the loaded assembly or <see langword="null"/>.</returns>
    private static Assembly? LoadAssembly(string path)
    {
        try
        {
            Assembly assembly = Assembly.Load(File.ReadAllBytes(path));

            try
            {
                EmbeddedResourceLoader.Instance.GetEmbeddedObjects(assembly);
            }
            catch (Exception e)
            {
                Log.Warn($"Could not load embedded assemblies. Error: \n{e}");
            }

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
    private static IPlugin<IConfig>? CreatePlugin(Assembly assembly)
    {
        try
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type == typeof(AttributePlugin<,>) || type.IsSubclassOf(typeof(AttributePlugin<,>)))
                    continue;

                if (type.IsAbstract || type.IsInterface)
                {
                    Log.Debug($"\"{type.FullName}\" is an interface or abstract class, skipping.", ShowPluginFinderDebug);
                    continue;
                }

                if (!IsDerivedFromPlugin(type))
                {
                    Log.Debug($"\"{type.FullName}\" does not inherit from Plugin<TConfig>, skipping.", ShowPluginFinderDebug);
                    continue;
                }

                Log.Debug($"Loading type {type.FullName}", ShowPluginFinderDebug);

                IPlugin<IConfig>? plugin = null;

                ConstructorInfo? constructor = type.GetConstructor(new[] { typeof(Assembly) }) ?? type.GetConstructor(Type.EmptyTypes);
                if (constructor is not null)
                {
                    Log.Debug("Public default constructor found, creating instance...", ShowDebug);

                    object obj = constructor.Invoke(null);

                    // object obj = Activator.CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (obj is not IPlugin<IConfig> pluginI)
                    {
                        Log.Warn("Result is not a plugin.");
                    }
                    else
                    {
                        plugin = pluginI;
                    }
                }
                else
                {
                    Log.Debug($"Constructor wasn't found, searching for a property with the {type.FullName} type...", ShowDebug);

                    object? value = Array
                        .Find(
                            type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public),
                            property => property.PropertyType == type)?.GetValue(null);

                    if (value is not null)
                        plugin = value as IPlugin<IConfig>;
                }

                if (plugin is null)
                {
                    Log.Warn($"{type.FullName} is a valid plugin, but it cannot be instantiated! It either doesn't have a public default constructor without any arguments or a static property of the {type.FullName} type!");

                    continue;
                }

                Log.Debug($"Instantiated type {type.FullName}", ShowDebug);

                if (CheckPluginRequiredAPIVersion(plugin))
                    continue;

                return plugin;
            }
        }
        catch (ReflectionTypeLoadException reflectionTypeLoadException)
        {
            Log.Error($"Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {reflectionTypeLoadException}");

            foreach (Exception loaderException in reflectionTypeLoadException.LoaderExceptions)
            {
                Log.Error(loaderException);
            }
        }
        catch (Exception exception)
        {
            Log.Error($"Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {exception}");
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

            Version? version = null;
            try
            {
                if (type.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion is { } versionString)
                    version = Version.Parse(versionString);
            }
            catch (Exception)
            {
                // unused.
            }

            // if version is not defined via assembly.
            version ??= pluginInfo.Version;

            FieldInfo? configField = null;
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.GetCustomAttribute<LethalConfigAttribute>() is null && field.Name != "Config")
                {
                    continue;
                }

                if (field.FieldType.GetInterface(nameof(IConfig)) is null)
                {
                    Log.Warn($"Found a config for plugin '{type.FullName}', but it didn't inherit an IConfig, therefore it was skipped.");
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

            MethodInfo? onEnabled = null;
            MethodInfo? onDisabled = null;
            MethodInfo? onReloaded = null;
            foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (methodInfo.GetParameters().Length > 0)
                    continue;

                if (onEnabled is null && (methodInfo.GetCustomAttribute<LethalEntrypointAttribute>() is not null || methodInfo.Name == "OnEnabled"))
                {
                    onEnabled = methodInfo;
                    continue;
                }

                if (onDisabled is null && (methodInfo.GetCustomAttribute<LethalDisableHandlerAttribute>() is not null || methodInfo.Name == "OnDisabled"))
                {
                    onDisabled = methodInfo;
                    continue;
                }

                if (onReloaded is null && (methodInfo.GetCustomAttribute<LethalReloadHandlerAttribute>() is not null || methodInfo.Name == "OnReloaded"))
                    onReloaded = methodInfo;
            }

            if (onEnabled is null)
            {
                Log.Warn($"Plugin '{type.FullName}' could not be registered because there wasn't an entrypoint method. Try adding the [LethalEntrypoint] attribute, or naming a method OnEnabled." +
                          $"Also ensure that the method has no arguments.");
                continue;
            }

            Version requiredApiVersion = type.GetCustomAttribute<LethalRequiredAPIVersionAttribute>()?.Version ?? new Version(1, 0, 0);

            ConstructorInfo? constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor is null)
            {
                Log.Warn($"Found valid plugin '{type.FullName}' but couldn't find the constructor to use. Ensure a constructor is present which has no arguments.");
                continue;
            }

            try
            {
                object typeInstance = constructor.Invoke(null);
                object? pluginInstance = typeof(AttributePlugin<,>).MakeGenericType(type, configField.FieldType).GetConstructors()[0]
                    .Invoke(new[]
                    {
                        typeInstance,
                        pluginInfo.Name,
                        pluginInfo.Description,
                        pluginInfo.Author,
                        version,
                        (Action)Delegate.CreateDelegate(typeof(Action), typeInstance, onEnabled),
                        configField,
                        requiredApiVersion,
                        onDisabled is null ? () => { } : (Action)Delegate.CreateDelegate(typeof(Action), typeInstance, onDisabled),
                        onReloaded is null ? () => { } : (Action)Delegate.CreateDelegate(typeof(Action), typeInstance, onReloaded),
                    });
                return (IPlugin<IConfig>)pluginInstance;
            }
            catch (Exception e)
            {
                Log.Warn($"Could not initialize plugin \'{type.FullName}\' due to an error.");
                Log.Debug($"{e}", ShowDebug);
            }
        }

        return null;
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
                if (MelonLoaderFound && Path.GetFileNameWithoutExtension(dependency) == "Bootstrap")
                    continue;

                Assembly? assembly = LoadAssembly(dependency);
                if (assembly is null)
                    continue;

                Locations[assembly] = dependency;

                Dependencies.Add(assembly);

                Log.Info($"Loaded &fDependency &h'&3{assembly.GetName().Name}&h'&7@&gv{assembly.GetName().Version.ToString(3)}&7");
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
    /// <returns><see langword="true"/> if passed type is derived from <see cref="Plugin{TConfig}"/> or <see cref="CorePlugin"/>, otherwise <see langword="false"/>.</returns>
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
                Log.Error($"You're running an older version of LethalAPI ({Version.ToString(3)})! {plugin.Name} won't be loaded! " +
                          $"Required version to load it: {plugin.RequiredAPIVersion.ToString(3)}");

                return true;
            }

            if (requiredVersion.Major < actualVersion.Major)
            {
                Log.Error($"You're running an older version of {plugin.Name} ({plugin.Version.ToString(3)})! " +
                    $"Its Required Major version is {requiredVersion.Major}, but the actual version is: {actualVersion.Major}. This plugin will not be loaded!");

                return true;
            }
        }

        return false;
    }
}

#pragma warning disable SA1201 // enum shouldnt follow type.
/// <summary>
/// Represents the different load methods that can be used.
/// </summary>
public enum LoadMethod
{
    /// <summary>
    /// Loaded via BepInEx.
    /// </summary>
    BepInEx = 1,

    /// <summary>
    /// Loaded via MelonLoader.
    /// </summary>
    MelonLoader = 2,

    /// <summary>
    /// Loaded via Doorstop.
    /// </summary>
    Doorstop = 4,

    /// <summary>
    /// Loaded manually.
    /// </summary>
    Manual = 8,
}