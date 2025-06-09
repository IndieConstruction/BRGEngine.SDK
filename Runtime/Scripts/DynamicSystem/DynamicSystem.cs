using System;
using UnityEngine;
using BRGEngine.SDK;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace BRGEngine.SDK {

    [Serializable]
    public class SystemEntry
    {
        public string key;
        public BaseSystem value;
    }

    /// <summary>
    /// This system choose dynamically a system from a list of systems to be added to the BRG systems list.
    /// <see href="https://tangible-waste-db5.notion.site/Dynamic-System-20dd97a90d5c8056b044eb60cc187189">Docs</see>
    /// </summary>
    [CreateAssetMenu(fileName = "DynamicSystemConfig", menuName = "BRGEngine/Systems/Dynamic")]
    public class DynamicSystem : BaseSystem {

        [SerializeField] private string _variableName;
        public string VariableName {
            get { return _variableName; }
            protected set { _variableName = value; }
        }
        
        [Tooltip("If true and Variable value is not found in systems list, the first one in list is set as target.")] 
        [SerializeField] private bool _getSystemAsDefault = true;
        [SerializeField] private List<SystemEntry> systems;

        public override void Init() {
             
        }

        public override void PreInit() {
            if (systems != null && systems.Count > 0)
            {
                ISystem newSystem = null;
                // If VariableName is set, try to find the system by its name in the systems list
                if (!string.IsNullOrEmpty(VariableName) && PlayerPrefs.HasKey(VariableName)) {
                    string systemName = PlayerPrefs.GetString(VariableName);
                    foreach (var entry in systems) {
                        if (entry.key.Equals(systemName, StringComparison.OrdinalIgnoreCase)) {
                            newSystem = entry.value;
                            break;
                        }
                    }
                }
                if (newSystem == null) { 
                    // If no system found by name, use the first one in the list if getSystemAsDefault is true
                    if (_getSystemAsDefault && systems.Count > 0) {
                        newSystem = systems[0].value;
                        Debug.LogWarning($"No system found for VariableName '{VariableName}'. Using default system: {newSystem.ToString()}");
                    } else {
                        Debug.LogWarning($"No system found for VariableName '{VariableName}' and getSystemAsDefault is false.");
                        return; // Exit if no system is found and not using default
                    }
                }
                SubSystems.Add(newSystem as BaseSystem);
            }
        }

        [Button("SetVariableValue")]
        private void SetVariableValue(string value) {
            // Set player pref variable named VariableName to value
            if (string.IsNullOrEmpty(VariableName)) return;
            if (value == null) value = string.Empty;
            if (PlayerPrefs.HasKey(VariableName)) {
                PlayerPrefs.SetString(VariableName, value);
            } else {
                PlayerPrefs.SetString(VariableName, value);
            }
        }

        [Button("DeleteVariable")]
        private void DeleteVariable() {
            // Delete player pref variable named VariableName
            if (string.IsNullOrEmpty(VariableName)) return;
            if (PlayerPrefs.HasKey(VariableName)) {
                PlayerPrefs.DeleteKey(VariableName);
                Debug.Log($"Variable {VariableName} deleted.");
            } else {
                Debug.LogWarning($"Variable {VariableName} not found to delete.");
            }
        }

        [Button("GetVariableValue")]
        private string GetVariableValue() {
            // Get player pref variable named VariableName
            if (string.IsNullOrEmpty(VariableName)) return null;
            if (PlayerPrefs.HasKey(VariableName)) {
                Debug.Log($"Variable {VariableName} found with value: {PlayerPrefs.GetString(VariableName)}");
                return PlayerPrefs.GetString(VariableName);
            } else {
                Debug.LogWarning($"Variable {VariableName} not found.");
                return null;
            }
        }

    }

}