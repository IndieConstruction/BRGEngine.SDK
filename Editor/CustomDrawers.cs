using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BRGEngine.SDK {

    #region Inline Editing Espandable

    [CustomPropertyDrawer(typeof(InlineAttribute))]
    public class InlineDrawers : PropertyDrawer {
        private Editor editor = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            // Disegna il campo dell'oggetto serializzato (riferimento)
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property, label);

            // Sposta la posizione per il foldout
            position.y += EditorGUIUtility.singleLineHeight;

            // Se l'oggetto esiste, disegna il foldout per espandere o comprimere i dettagli
            if (property.objectReferenceValue != null) {
                // Foldout per espandere/comprimere
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, "Show Details");

                // Se espanso, crea e disegna l'editor per l'oggetto serializzato
                if (property.isExpanded) {
                    EditorGUI.indentLevel++;

                    // Crea un editor per l'oggetto serializzato se necessario
                    if (editor == null || editor.target != property.objectReferenceValue) {
                        editor = Editor.CreateEditor(property.objectReferenceValue);
                    }

                    if (editor != null) {
                        // Disegna l'editor inline
                        editor.OnInspectorGUI();
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            // Calcola l'altezza in base allo stato espanso e alla presenza dell'oggetto
            if (property.isExpanded && property.objectReferenceValue != null) {
                if (editor != null) {
                    return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.singleLineHeight * 2; // Altezza per l'oggetto e il foldout
                }
            }

            // Altezza normale se non espanso o se non c'è un oggetto
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }

    #endregion

    #region Show Hide

    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ConditionalHideDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Recupera l'attributo
            HideIfAttribute hideIf = attribute as HideIfAttribute;
            ShowIfAttribute showIf = attribute as ShowIfAttribute;

            // Trova la variabile condizione
            bool shouldHide = false;
            if (hideIf != null) {
                shouldHide = ShouldHide(property, hideIf.ConditionField, hideIf.DesiredCondition);
            } else if (showIf != null) {
                shouldHide = !ShouldHide(property, showIf.ConditionField, showIf.DesiredCondition);
            }

            // Se il campo non deve essere nascosto, lo mostriamo
            if (!shouldHide) {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            // Recupera l'attributo
            HideIfAttribute hideIf = attribute as HideIfAttribute;
            ShowIfAttribute showIf = attribute as ShowIfAttribute;

            // Trova la variabile condizione
            bool shouldHide = false;
            if (hideIf != null) {
                shouldHide = ShouldHide(property, hideIf.ConditionField, hideIf.DesiredCondition);
            } else if (showIf != null) {
                shouldHide = !ShouldHide(property, showIf.ConditionField, showIf.DesiredCondition);
            }

            // Se il campo deve essere nascosto, l'altezza sarà zero
            return shouldHide ? 0 : EditorGUI.GetPropertyHeight(property, label);
        }

        private bool ShouldHide(SerializedProperty property, string conditionField, bool desiredCondition) {
            // Ottieni l'oggetto contenente la variabile condizione
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionField);

            if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean) {
                // Verifica se la condizione corrisponde
                return conditionProperty.boolValue == desiredCondition;
            } else {
                Debug.LogWarning($"[ConditionalHideDrawer] Variabile condizione '{conditionField}' non trovata o non è un booleano.");
                return false;
            }
        }
    }

    #endregion

    #region Button

    namespace BRGEngine.SDK {

        #region Button

        [CustomEditor(typeof(MonoBehaviour), true)]
        public class ButtonMethodDrawerForMonoBehaviour : Editor {
            // Dizionario per memorizzare i valori temporanei dei parametri per ogni oggetto e metodo
            private static Dictionary<string, string> parameterInputs = new Dictionary<string, string>();

            public override void OnInspectorGUI() {
                base.OnInspectorGUI();

                var type = target.GetType();
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var method in methods) {
                    var buttonAttribute = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));
                    if (buttonAttribute != null) {
                        var parameters = method.GetParameters();
                        object[] parameterValues = null;

                        if (parameters.Length > 0) {
                            parameterValues = new object[parameters.Length];
                            for (int i = 0; i < parameters.Length; i++) {
                                var parameter = parameters[i];
                                string key = $"{target.GetInstanceID()}_{method.Name}_{i}";
                                string inputValue = parameterInputs.ContainsKey(key) ? parameterInputs[key] : string.Empty;

                                // Solo per string, puoi estendere per altri tipi se necessario
                                if (parameter.ParameterType == typeof(string)) {
                                    inputValue = EditorGUILayout.TextField(parameter.Name, inputValue);
                                    parameterInputs[key] = inputValue;
                                    parameterValues[i] = inputValue;
                                } else {
                                    EditorGUILayout.LabelField($"{parameter.Name}: tipo non supportato ({parameter.ParameterType.Name})");
                                    parameterValues[i] = null;
                                }
                            }
                        }

                        if (GUILayout.Button(buttonAttribute.ButtonText)) {
                            if (parameters.Length == 0) {
                                method.Invoke(target, null);
                            } else {
                                method.Invoke(target, parameterValues);
                            }
                        }
                    }
                }
            }
        }

        [CustomEditor(typeof(ScriptableObject), true)]
        public class ButtonMethodDrawerForScriptableObject : Editor {
            private static Dictionary<string, string> parameterInputs = new Dictionary<string, string>();

            public override void OnInspectorGUI() {
                base.OnInspectorGUI();

                var type = target.GetType();
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var method in methods) {
                    var buttonAttribute = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));
                    if (buttonAttribute != null) {
                        var parameters = method.GetParameters();
                        object[] parameterValues = null;

                        if (parameters.Length > 0) {
                            parameterValues = new object[parameters.Length];
                            for (int i = 0; i < parameters.Length; i++) {
                                var parameter = parameters[i];
                                string key = $"{target.GetInstanceID()}_{method.Name}_{i}";
                                string inputValue = parameterInputs.ContainsKey(key) ? parameterInputs[key] : string.Empty;

                                if (parameter.ParameterType == typeof(string)) {
                                    inputValue = EditorGUILayout.TextField(parameter.Name, inputValue);
                                    parameterInputs[key] = inputValue;
                                    parameterValues[i] = inputValue;
                                } else {
                                    EditorGUILayout.LabelField($"{parameter.Name}: tipo non supportato ({parameter.ParameterType.Name})");
                                    parameterValues[i] = null;
                                }
                            }
                        }

                        if (GUILayout.Button(buttonAttribute.ButtonText)) {
                            if (parameters.Length == 0) {
                                method.Invoke(target, null);
                            } else {
                                method.Invoke(target, parameterValues);
                            }
                        }
                    }
                }
            }
        }

        #endregion

    }


    #endregion

}