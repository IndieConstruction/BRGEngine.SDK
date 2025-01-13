using System.Collections.Generic;
using UnityEngine;

namespace BRGEngine.SDK {
    [DefaultExecutionOrder(-100000)]
    public class BRGEngineSystemsManager : MonoBehaviour {
        [SerializeField, Inline] private List<BaseSystem> systems = new List<BaseSystem>();

        private bool _isInitialized = false;

        /// <summary>
        /// If true, the system manage will be a singleton and will not be destroyed when a new scene is loaded.
        /// </summary>
        [SerializeField] private bool useSingleton = true;

        private static BRGEngineSystemsManager _instance;

        private void Awake() {
            if(useSingleton) {
                    if (_instance == null) {
                        _instance = this;
                        DontDestroyOnLoad(gameObject);
                    } else {
                        Destroy(gameObject);
                        return;
                    }
                }
            BRG.SystemGameObject = gameObject;
            BRG.LoadAndSetup(systems);
        }

        private void Start() {
            BRG.IsAllSystemsSetupsDone = true;
            BRG.Start?.Invoke();
        }

        private void Update() {
            if (BRG.IsAllSystemsSetupsDone) {
                BRG.Update?.Invoke();
            }
        }

        private void OnDestroy() {
            foreach (var system in BRG.CurrentSystems) {
                system.Finish();
            }
            BRG.IsAllSystemsSetupsDone = false;
            BRG.Finish?.Invoke();
        }

    }
}