// -----------------------------------------------------------------------
// <copyright file="CommentsObjectGraphVisitor.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Thanks to Antoine Aubry for this awesome work.
// Checkout more here: https://dotnetfiddle.net/8M6iIE.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs.Tools;

using System;
using System.Collections;
using System.Reflection;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;

/// <summary>
/// Gathers comments in the chain.
/// </summary>
public sealed class CommentsObjectGraphVisitor : ChainedObjectGraphVisitor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentsObjectGraphVisitor"/> class.
    /// </summary>
    /// <param name="nextVisitor">The next visitor instance.</param>
    public CommentsObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
        : base(nextVisitor)
    {
    }

    /// <inheritdoc/>
    public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
    {
        try
        {
            YamlIgnoreAttribute? ignoreAttribute = key.GetCustomAttribute<YamlIgnoreAttribute>();
            YamlIgnoreAttribute? propertyIgnoreAttribute = value.Type.GetCustomAttribute<YamlIgnoreAttribute>();

            if (ignoreAttribute is not null)
                return false;
            if (propertyIgnoreAttribute is not null)
                return false;
            YamlMemberAttribute? memberAttribute = key.GetCustomAttribute<YamlMemberAttribute>();
            YamlMemberAttribute? propertyMemberAttribute = key.GetCustomAttribute<YamlMemberAttribute>();
            DefaultValuesHandling handling = 0;
            if (memberAttribute is not null && memberAttribute.IsDefaultValuesHandlingSpecified)
            {
                handling |= memberAttribute.DefaultValuesHandling;
            }

            if (propertyMemberAttribute is not null && propertyMemberAttribute.IsDefaultValuesHandlingSpecified)
            {
                handling |= propertyMemberAttribute.DefaultValuesHandling;
            }

            if (handling == 0)
                goto SkipDefaultsCheck;

            if (handling.HasFlag(DefaultValuesHandling.OmitDefaults))
            {
                object? defaultValue = value.Type.IsValueType ? Activator.CreateInstance(value.Type) : null;
                if (Equals(value.Value, defaultValue))
                    return false;
            }

            if (handling.HasFlag(DefaultValuesHandling.OmitNull))
            {
                if (Equals(value.Value, null))
                    return false;
            }

            if (handling.HasFlag(DefaultValuesHandling.OmitEmptyCollections))
            {
                if (value.Value is ICollection { Count: 0 })
                    return false;
            }
        }
        catch (Exception e)
        {
            Log.Debug($"Yaml error caught.");
            Log.Exception(e);
        }

        SkipDefaultsCheck:

        try
        {
            if (value is CommentsObjectDescriptor commentsDescriptor)
            {
                context.Emit(new Comment(commentsDescriptor.Comment, false));
            }
        }
        catch (Exception e)
        {
            Log.Debug($"Cannot emit comment.");
            Log.Exception(e);
        }

        return base.EnterMapping(key, value, context);
    }
}
