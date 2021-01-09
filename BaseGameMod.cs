using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Base_Mod.Models;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace Base_Mod {
    [UsedImplicitly]
    public abstract class BaseGameMod : GameMod {
        protected abstract string ModName    { get; }
        protected virtual  bool   UseHarmony { get; } = false;
        protected          string ConfigFile => Path.Combine(AssemblyDirectory, $"{ModName}.json");

        public override void Load() {
            Debug.Log($"{ModName} loading.");

            if (UseHarmony) {
                Assembly.LoadFrom(Path.Combine(AssemblyDirectory, "0Harmony.dll"));

                var harmony = new Harmony(GUID.Create().ToString());
                harmony.PatchAll(GetType().Assembly);

                var i = 0;
                foreach (var patchedMethod in harmony.GetPatchedMethods()) {
                    Debug.Log($"Patched: {patchedMethod.DeclaringType?.FullName}:{patchedMethod}");
                    i++;
                }
                Debug.Log($"Patched {i} methods.");
            }

            Init();

            Debug.Log($"{ModName} loaded.");
        }

        protected abstract void Init();

        public override void Unload() {
            DoAllUnloadedPatches();
        }

        protected void DoAllIslandSceneLoadedPatches() {
            foreach (var method in GetAllMethodsWithIslandSceneLoaded<OnIslandSceneLoadedAttribute>()) {
                method.Invoke(null, null);
            }
        }

        protected void DoAllUnloadedPatches() {
            foreach (var method in GetAllMethodsWithIslandSceneLoaded<OnUnloadedAttribute>()) {
                method.Invoke(null, null);
            }
        }

        private IEnumerable<MethodInfo> GetAllMethodsWithIslandSceneLoaded<T>() where T : Attribute {
            return from type in GetType().Assembly.GetTypes()
                   from method in type.GetMethods()
                   where method.GetCustomAttributes(typeof(T), false).Any()
                   select method;
        }

        protected static string AssemblyDirectory {
            get {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri      = new UriBuilder(codeBase);
                var path     = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}