using System.Collections.Generic;
using UnityEngine;

namespace BRGEngine.SDK {
    [DefaultExecutionOrder(-100000)]
    public class BRGEngineSystemsManager : MonoBehaviour
    {
        [SerializeField, Inline] private List<BaseSystem> systems = new List<BaseSystem>();

        private bool _isInitialized = false;

        private void Awake()
        {
            BRG.SystemGameObject = gameObject;
            BRG.LoadAndSetup(systems);
        }

        private void Start() {
            BRG.IsAllSystemsSetupsDone = true;
            BRG.Start?.Invoke();
        }

        private void Update()
        {
            if (BRG.IsAllSystemsSetupsDone)
            {
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