using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace Base_Mod {
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
    }
}