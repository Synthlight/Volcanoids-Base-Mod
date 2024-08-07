﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace Base_Mod;

[UsedImplicitly]
public static class Extensions {
    [UsedImplicitly]
    public static bool SetPrivateField<T>(this T obj, string fieldName, object newValue) {
        var fieldInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (fieldInfo == null) {
            Debug.LogError($"Error: Unable to find private field `{fieldName}` in `{typeof(T)}`.");
            return false;
        }
        fieldInfo.SetValue(obj, Convert.ChangeType(newValue, fieldInfo.FieldType));
        return true;
    }

    [UsedImplicitly]
    public static bool SetPrivateField(this object obj, FieldInfo fieldInfo, object newValue) {
        fieldInfo.SetValue(obj, Convert.ChangeType(newValue, fieldInfo.FieldType));
        return true;
    }

    [UsedImplicitly]
    public static T GetPrivateFieldNullable<O, T>(this O obj, string fieldName) where T : class {
        var fieldInfo = typeof(O).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (fieldInfo == null) {
            Debug.LogError($"Error: Unable to find private field `{fieldName}` in `{typeof(T)}`.");
            return null;
        }
        return (T) fieldInfo.GetValue(obj);
    }

    [UsedImplicitly]
    public static T GetPrivateField<O, T>(this O obj, string fieldName) where T : struct {
        var fieldInfo = typeof(O).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (fieldInfo == null) {
            Debug.LogError($"Error: Unable to find private field `{fieldName}` in `{typeof(T)}`.");
            return default;
        }
        return (T) fieldInfo.GetValue(obj);
    }

    [UsedImplicitly]
    public static T GetPrivateField<T>(this object obj, FieldInfo fieldInfo) {
        return (T) fieldInfo.GetValue(obj);
    }

    [UsedImplicitly]
    public static bool Is(this Type source, params Type[] types) {
        return types.Any(type => type.IsAssignableFrom(source));
    }

    [UsedImplicitly]
    public static IEnumerable<T> WithComponent<T>(this IEnumerable<ItemDefinition> source) where T : MonoBehaviour {
        var done = new List<T>(); // In case it's re-used on multiple prefabs or objects.

        // For prefabs.
        foreach (var inventoryItemDef in from item in source
                                         where item.Prefabs?.Any(o => o.HasComponent<T>()) ?? false
                                         select item) {
            foreach (var prefab in inventoryItemDef.Prefabs) {
                if (prefab.TryGetComponent<T>(out var component) && !done.Contains(component)) {
                    yield return component;

                    done.Add(component);
                }
            }
        }

        // And all existing objects.
        foreach (var component in Resources.FindObjectsOfTypeAll<T>()) {
            if (!done.Contains(component)) {
                yield return component;
            }
        }
    }

    [UsedImplicitly]
    public static bool HasComponent<T>(this MonoBehaviour thing) {
        return thing.GetComponent<T>() != null;
    }

    [UsedImplicitly]
    public static void Nop(this IReadOnlyList<CodeInstruction> il, int index) {
        il.Nop(index, index);
    }

    [UsedImplicitly]
    public static void Nop(this IReadOnlyList<CodeInstruction> il, int start, int end) {
        for (var i = start; i <= end; i++) {
            il[i].opcode  = OpCodes.Nop;
            il[i].operand = null;
        }
    }

    public static void AddToAssetDatabase(this Definition def) {
        RuntimeAssetDatabase.Add(def);
        Debug.Log($"Added Definition: {def.name}, {def.AssetId}");
    }

    public static bool TryGetComponent<T>(this ItemDefinition item, out T component) {
        component = default;
        return item.Prefabs?.Length > 0 && item.Prefabs[0].TryGetComponent(out component);
    }

    public static bool TryGetComponent<T>(this ToolItemDefinition item, out T component) {
        component = default;
        return item.Prefab?.TryGetComponent(out component) == true;
    }

    public static bool HasComponent(this ItemDefinition item, Type type) {
        return item.Prefabs?.Length > 0 && item.Prefabs[0].TryGetComponent(type, out _);
    }

    public static bool HasComponent(this ToolItemDefinition item, Type type) {
        return item.Prefab?.TryGetComponent(type, out _) == true;
    }

    public static bool HasAny(this ItemDefinition item, params Type[] types) {
        foreach (var type in types) {
            if ((item is ToolItemDefinition tool && tool.HasComponent(type)) || item.HasComponent(type)) return true;
        }
        return false;
    }

    private static readonly PropertyInfo RUNTIME_METHOD_INFO_NAME = Type.GetType("System.Reflection.RuntimeMethodInfo")?.GetProperty("Name");

    public static bool IsCall(this CodeInstruction inst, string methodName) {
        if (inst.opcode != OpCodes.Call && inst.opcode != OpCodes.Callvirt && inst.opcode != OpCodes.Calli) return false;
        var name = RUNTIME_METHOD_INFO_NAME.GetValue(inst.operand) as string;
        return name == methodName;
    }

    public static bool ContainsIgnoreCase(this IEnumerable<string> arr, string needle) {
        return arr.Any(s => string.Equals(s, needle, StringComparison.CurrentCultureIgnoreCase));
    }

    public static bool ContainsIgnoreCase(this string arr, string needle) {
        return arr.Contains(needle, StringComparison.CurrentCultureIgnoreCase);
    }
}