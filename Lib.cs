using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Base_Mod;

public static class Lib {
    [UsedImplicitly]
    public static IEnumerable<GameObject> GetItemDefinitionWithComponent<T>(GUID guid) where T : Component {
        if (RuntimeAssetDatabase.TryGetDefinition<ItemDefinition>(guid, out var asset)) {
            foreach (var prefab in asset.Prefabs) {
                if (prefab.HasComponent<T>()) {
                    yield return prefab.gameObject;
                }
            }
        }
    }
}