// -----------------------------------------------------------------------
// <copyright file="EventPatchAttribute.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments.
// -----------------------------------------------------------------------
namespace LCAPI.Core.Events.Attributes
{
    using System;

    using Interfaces;

    /// <summary>
    /// An attribute to contain data about an event patch.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class EventPatchAttribute : Attribute
    {
        private readonly Type handlerType;
        private readonly string eventName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPatchAttribute"/> class.
        /// </summary>
        /// <param name="eventName">The <see cref="Type"/> of the handler class that contains the event.</param>
        /// <param name="handlerType">The name of the event.</param>
        internal EventPatchAttribute(Type handlerType, string eventName)
        {
            this.handlerType = handlerType;
            this.eventName = eventName;
        }

        /// <summary>
        /// Gets the <see cref="ILcApiEvent"/> that will be raised by this patch.
        /// </summary>
        internal ILcApiEvent Event => (ILcApiEvent)handlerType.GetProperty(eventName)?.GetValue(null);
    }
}