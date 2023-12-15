// -----------------------------------------------------------------------
// <copyright file="UnityTypeContractResolver.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// From https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters
// Licensed under MIT.
// View the license here:
// https://github.com/applejag/Newtonsoft.Json-for-Unity.Converters/blob/master/LICENSE.md
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Loader.Configs.Tools;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

/// <inheritdoc />
public class UnityTypeContractResolver : DefaultContractResolver
{
    /// <inheritdoc />
    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        List<MemberInfo> members = base.GetSerializableMembers(objectType);

        members.AddRange(GetMissingMembers(objectType, members));

        return members;
    }

    /// <inheritdoc />
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);

        if (member.GetCustomAttribute<SerializeField>() != null)
        {
            jsonProperty.Ignored = false;
            jsonProperty.Writable = CanWriteMemberWithSerializeField(member);
            jsonProperty.Readable = CanReadMemberWithSerializeField(member);
            jsonProperty.HasMemberAttribute = true;
        }

        return jsonProperty;
    }

    /// <inheritdoc />
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        IList<JsonProperty> lists = base.CreateProperties(type, memberSerialization);

        return lists;
    }

    /// <inheritdoc />
    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
        JsonObjectContract jsonObjectContract = base.CreateObjectContract(objectType);

        if (typeof(ScriptableObject).IsAssignableFrom(objectType))
        {
            jsonObjectContract.DefaultCreator = () => { return ScriptableObject.CreateInstance(objectType); };
        }

        return jsonObjectContract;
    }

    private static IEnumerable<MemberInfo> GetMissingMembers(Type type, List<MemberInfo> alreadyAdded)
    {
        return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
            .Cast<MemberInfo>()
            .Concat(type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            .Where(o => o.GetCustomAttribute<SerializeField>() != null
                && !alreadyAdded.Contains(o));
    }

    private static bool CanReadMemberWithSerializeField(MemberInfo member)
    {
        if (member is PropertyInfo property)
        {
            return property.CanRead;
        }
        else
        {
            return true;
        }
    }

    private static bool CanWriteMemberWithSerializeField(MemberInfo member)
    {
        if (member is PropertyInfo property)
        {
            return property.CanWrite;
        }
        else
        {
            return true;
        }
    }
}
