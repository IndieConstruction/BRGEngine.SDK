using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BRGEngine.SDK {

    public static class BRG {

        #region Systems

        public static event Action OnAllSystemsSetupsDone;

        private static bool _isAllSystemsSetupsDone;
        public static bool IsAllSystemsSetupsDone {
            get { return _isAllSystemsSetupsDone; }
            internal set {
                _isAllSystemsSetupsDone = value;
                if (_isAllSystemsSetupsDone) {
                    OnAllSystemsSetupsDone?.Invoke();
                }
            }
        }
        
        public static Action Start;
        public static Action Update;
        public static Action Finish;

        public static List<ISystem> CurrentSystems { get; set; }
        public static GameObject SystemGameObject { get; set; }
        public static MonoBehaviour SystemMonoBehaviour { get { return SystemGameObject ? SystemGameObject.GetComponent<MonoBehaviour>() : null; } }

        public static void LoadAndSetup(List<BaseSystem> systems) {
            CurrentSystems = new List<ISystem>();
            foreach (var system in systems) {
                BaseSystem systemInstance = initSystem(system, null) as BaseSystem;
                CurrentSystems.Add(systemInstance);
            }
        }

        /// <summary>
        /// Init system and all its sub-systems using scriptable object or class instance. Finally call the Init overridable method for any system.
        /// </summary>
        /// <param name="systemToInitialize"></param>
        /// <param name="parentSystem"></param>
        /// <returns></returns>
        private static ISystem initSystem(ISystem systemToInitialize, ISystem parentSystem) {
            BaseSystem systemInstance = null;
            if (systemToInitialize.RequiresNewInstance)
                systemInstance = ScriptableObject.Instantiate(systemToInitialize as BaseSystem);
            else
                systemInstance = systemToInitialize as BaseSystem;
            systemInstance.InitInternal(parentSystem);
            foreach (ISystem subSystem in systemInstance.SubSystems) {
                if (subSystem.AutoInitAtStartup) {
                    CurrentSystems.Add(initSystem(subSystem, systemInstance));
                }
            }
            return systemInstance;
        }

        public static T SYS<T>() where T : ISystem {
            ISystem returnSystem;
            try {
                returnSystem = CurrentSystems.Find(s => s.GetType() == typeof(T) || typeof(T).IsAssignableFrom(s.GetType()));
            } catch (Exception e) {
                Debug.LogWarning($"System {typeof(T).Name} not found. {e.Message}");
                return default(T);
            }
            return (T)returnSystem;
        }

        #endregion

        #region Debug 

        public static void DebugLog(string message) {
            DebugSystem debugSystem = SYS<DebugSystem>();
            if(debugSystem != null) {
                debugSystem.Log(message);
                return;
            }

            Debug.Log(message);
        }

        #endregion

        #region Helpers

        #region Coroutine Execution

        public static void ExecuteCoroutine(IEnumerator coroutine) {
            if (SystemMonoBehaviour == null) {
                throw new InvalidOperationException("SystemGameObject does not have a MonoBehaviour component.");
            }
            SystemMonoBehaviour.StartCoroutine(coroutine);
        }

        #endregion

        public static async Task ExecuteAfterDelay(Action action, int delayInMilliseconds) {
            // Attendi per il tempo specificato
            await Task.Delay(delayInMilliseconds);

            // Esegui l'azione passata come parametro
            action?.Invoke();
        }

        #region Delayed Execution (Sync - use it for webGL) 

        /// <summary>
        /// Delayed execution of an action, using a coroutine. Use it for WebGL platform instead of ExecuteAfterDelay.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayInMilliseconds"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ExecuteAfterDelaySync(Action action, float delayInMilliseconds) {
            if (SystemMonoBehaviour == null) {
                throw new InvalidOperationException("SystemGameObject does not have a MonoBehaviour component.");
            }
            SystemMonoBehaviour.StartCoroutine(ExecuteAfterDelayCoroutine(action, delayInMilliseconds));
        }

        private static IEnumerator ExecuteAfterDelayCoroutine(Action action, float delayInMilliseconds) {
            // Converti i millisecondi in secondi
            float delayInSeconds = delayInMilliseconds / 1000f;

            // Attendi per il tempo specificato
            yield return new WaitForSeconds(delayInSeconds);

            // Esegui l'azione passata come parametro
            action?.Invoke();
        }

        #endregion

        #endregion

    }

}
