// -----------------------------------------------------------------------
// <copyright file="TypeAssigningEventEmitter.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments.
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Loader.Configs.Tools;

using System;

using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

/// <summary>
/// Event emitter which wraps all strings in double quotes.
/// </summary>
public class TypeAssigningEventEmitter : ChainedEventEmitter
{
    private readonly char[] multiline = new char[] { '\r', '\n', '\x85', '\x2028', '\x2029' };

    /// <inheritdoc cref="ChainedEventEmitter"/>
    public TypeAssigningEventEmitter(IEventEmitter nextEmitter)
        : base(nextEmitter)
    {
    }

    /// <inheritdoc/>
    public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
    {
        if (eventInfo.Source.StaticType != typeof(object) && Type.GetTypeCode(eventInfo.Source.StaticType) == TypeCode.String && !UnderscoredNamingConvention.Instance.Properties.Contains(eventInfo.Source.Value!))
        {
            if (eventInfo.Source.Value == null || eventInfo.Source.Value.ToString().IndexOfAny(multiline) is -1)
                eventInfo.Style = Serialization.ScalarStyle;
            else
                eventInfo.Style = Serialization.MultiLineScalarStyle;
        }

        base.Emit(eventInfo, emitter);
    }
}