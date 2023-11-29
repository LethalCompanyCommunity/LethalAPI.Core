// -----------------------------------------------------------------------
// <copyright file="Event{T}.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments.
// -----------------------------------------------------------------------

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Events.Features;

using System;
using System.Collections.Generic;
using System.Linq;

using Interfaces;

/// <summary>
/// The custom <see cref="EventHandler"/> delegate.
/// </summary>
/// <typeparam name="TEventArgs">The <see cref="EventHandler{TEventArgs}"/> type.</typeparam>
/// <param name="ev">The <see cref="EventHandler{TEventArgs}"/> instance.</param>
public delegate void CustomEventHandler<in TEventArgs>(TEventArgs ev);

/// <summary>
/// An implementation of the <see cref="ILethalApiEvent"/> interface that encapsulates an event with arguments.
/// </summary>
/// <typeparam name="T">The specified <see cref="EventArgs"/> that the event will use.</typeparam>
public class Event<T> : ILethalApiEvent
{
    private static readonly Dictionary<Type, Event<T>> TypeToEvent = new();

    /// <summary>
    /// Indicates whether the event has been patched or not. We can utilize dynamic patching to only patch the events that we need.
    /// </summary>
    private bool patched;

    /// <summary>
    /// Initializes a new instance of the <see cref="Event{T}"/> class.
    /// </summary>
    public Event()
    {
        TypeToEvent.Add(typeof(T), this);
    }

    private event CustomEventHandler<T>? InnerEvent;

    /// <summary>
    /// Gets a <see cref="IReadOnlyCollection{T}"/> of <see cref="Event{T}"/> which contains all the <see cref="Event{T}"/> instances.
    /// </summary>
    public static IReadOnlyDictionary<Type, Event<T>> Dictionary => TypeToEvent;

    /// <summary>
    /// Subscribes a target <see cref="CustomEventHandler{TEventArgs}"/> to the inner event and checks if patching is possible, if dynamic patching is enabled.
    /// </summary>
    /// <param name="event">The <see cref="Event{T}"/> the <see cref="CustomEventHandler{T}"/> will be subscribed to.</param>
    /// <param name="handler">The <see cref="CustomEventHandler{T}"/> that will be subscribed to the <see cref="Event{T}"/>.</param>
    /// <returns>The <see cref="Event{T}"/> with the handler subscribed to it.</returns>
    public static Event<T> operator +(Event<T> @event, CustomEventHandler<T> handler)
    {
        Log.Debug($"Event {typeof(T).Name} Subscribed to by {handler.Method.Name}", Events.DebugPatches, "LethalAPI-Events");
        @event.Subscribe(handler);
        return @event;
    }

    /// <summary>
    /// Unsubscribes a target <see cref="CustomEventHandler{TEventArgs}"/> from the inner event and checks if un-patching is possible, if dynamic patching is enabled.
    /// </summary>
    /// <param name="event">The <see cref="Event{T}"/> the <see cref="CustomEventHandler{T}"/> will be unsubscribed from.</param>
    /// <param name="handler">The <see cref="CustomEventHandler{T}"/> that will be unsubscribed from the <see cref="Event{T}"/>.</param>
    /// <returns>The <see cref="Event{T}"/> with the handler unsubscribed from it.</returns>
    public static Event<T> operator -(Event<T> @event, CustomEventHandler<T> handler)
    {
        @event.Unsubscribe(handler);
        return @event;
    }

    /// <summary>
    /// Subscribes a target <see cref="CustomEventHandler{T}"/> to the inner event if the conditional is true.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    public void Subscribe(CustomEventHandler<T> handler)
    {
        if (Events.UseDynamicPatching && !patched)
        {
            Events.Instance.Patcher.Patch(this);
            patched = true;
        }

        InnerEvent += handler;
    }

    /// <summary>
    /// Unsubscribes a target <see cref="CustomEventHandler{T}"/> from the inner event if the conditional is true.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    public void Unsubscribe(CustomEventHandler<T> handler)
    {
        InnerEvent -= handler;
    }

    /// <summary>
    /// Executes all <see cref="CustomEventHandler{TEventArgs}"/> listeners safely.
    /// </summary>
    /// <param name="arg">The event argument.</param>
    /// <exception cref="ArgumentNullException">Event or its arg is <see langword="null"/>.</exception>
    public void InvokeSafely(T arg)
    {
        Log.Debug($"Event {typeof(T).Name} Invoked", Events.LogEvent, "LethalAPI-Events");

        if (InnerEvent is null)
            return;

        foreach (CustomEventHandler<T> handler in InnerEvent.GetInvocationList().Cast<CustomEventHandler<T>>())
        {
            try
            {
                handler(arg);
            }
            catch (Exception ex)
            {
                Log.Error($"Method \"{handler.Method.Name}\" of the class \"{handler.Method.ReflectedType?.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
            }
        }
    }
}