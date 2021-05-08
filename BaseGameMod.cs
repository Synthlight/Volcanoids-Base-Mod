using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Base_Mod.Models;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Base_Mod {
    [UsedImplicitly]
    public abstract class BaseGameMod : GameMod {
        protected abstract string ModName    { get; }
        protected virtual  bool   UseHarmony { get; } = false;
        protected          string ConfigFile => Path.Combine(AssemblyDirectory, $"{ModName}.json");

        public override void Load() {
            var strMsg = $"{ModName} loading.";

            if (UseHarmony) {
                Assembly.LoadFrom(Path.Combine(AssemblyDirectory, "0Harmony.dll"));

                var harmony = new Harmony(GUID.Create().ToString());
                harmony.PatchAll(GetType().Assembly);

                var i = 0;
                foreach (var patchedMethod in harmony.GetPatchedMethods()) {
                    strMsg += $"\r\nPatched: {patchedMethod.DeclaringType?.FullName}:{patchedMethod}";
                    i++;
                }
                strMsg += $"\r\nPatched {i} methods.";
            }

            Init();

            strMsg += $"\r\n{ModName} loaded.";
            Debug.Log(strMsg);
        }

        protected virtual void Init() {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneLoaded += OnDataSetupBase;
        }

        private void OnDataSetupBase(Scene scene, LoadSceneMode loadSceneMode) {
            if (scene.name != "Island") return;
            SceneManager.sceneLoaded -= OnDataSetupBase;

            DoAllIslandSceneLoadedPatches();
            OnDataSetup();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
            if (scene.name != "Island") return;

            OnIslandSceneLoaded(scene, loadSceneMode);
        }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual void OnIslandSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual void OnDataSetup() {
        }

        public override void Unload() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            DoAllUnloadedPatches();
        }

        protected void DoAllIslandSceneLoadedPatches() {
            foreach (var method in GetAllMethodsWithIslandSceneLoaded<OnIslandSceneLoadedAttribute>()) {
                try {
                    method.Invoke(null, null);
                    Debug.Log($"Applying Patch: `{method.DeclaringType?.FullName}:{method}`");
                } catch (Exception e) {
                    Debug.LogError($"Error running `{method.Name}`: {e}");
                }
            }
        }

        protected void DoAllUnloadedPatches() {
            foreach (var method in GetAllMethodsWithIslandSceneLoaded<OnUnloadedAttribute>()) {
                try {
                    method.Invoke(null, null);
                } catch (Exception e) {
                    Debug.LogError($"Error running `{method.Name}`: {e}");
                }
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