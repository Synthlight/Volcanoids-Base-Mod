using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Base_Mod.Models;
using HarmonyLib;
using JetBrains.Annotations;

namespace Base_Mod {
    [UsedImplicitly]
    public abstract class BaseGameMod : GameMod {
        protected abstract string ModName    { get; }
        protected virtual  bool   UseHarmony => false;

        public override void Load() {
            var log = new LogBuffer();
            log.Write($"{ModName} loading.");

            if (UseHarmony) {
                var harmony = new Harmony(GUID.Create().ToString());
                harmony.PatchAll(GetType().Assembly);

                var i = 0;
                foreach (var patchedMethod in harmony.GetPatchedMethods()) {
                    log.Write($"\r\nPatched: {patchedMethod.DeclaringType?.FullName}:{patchedMethod}");
                    i++;
                }
                log.Write($"\r\nPatched {i} methods.");
            }

            Init();

            log.Write($"\r\n{ModName} loaded.");
            log.Flush();
        }

        protected virtual void Init() {
        }

        public override void OnInitData() {
            DoAllIslandSceneLoadedPatches();
        }

        protected void DoAllIslandSceneLoadedPatches() {
            var log = new LogBuffer();
            foreach (var method in GetAllMethodsWithIslandSceneLoaded<OnIslandSceneLoadedAttribute>()) {
                try {
                    method.Invoke(null, null);
                    log.WriteLine($"Applying Patch: `{method.DeclaringType?.FullName}:{method}`");
                } catch (Exception e) {
                    log.WriteLine($"Error running `{method.Name}`: {e}");
                }
            }
            log.Flush();
        }

        private IEnumerable<MethodInfo> GetAllMethodsWithIslandSceneLoaded<T>() where T : Attribute {
            return from type in GetType().Assembly.GetTypes()
                   from method in type.GetMethods()
                   where method.GetCustomAttributes(typeof(T), false).Any()
                   select method;
        }

        protected string GetConfigFile() {
            Directory.CreateDirectory(PersistentDataDir);
            return Path.Combine(PersistentDataDir, $"{ModName}.json");
        }
    }
}