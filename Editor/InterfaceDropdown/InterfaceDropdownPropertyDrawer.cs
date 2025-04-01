using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[CustomPropertyDrawer(typeof(InterfaceDropdownAttribute))]
public class InterfaceDropdownDrawer : PropertyDrawer {
    private List<Type> _implementingTypes;
    private string[] _typeNames;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        InterfaceDropdownAttribute dropdownAttribute = (InterfaceDropdownAttribute)attribute;

        if (_implementingTypes == null) {
            LoadImplementingTypes(dropdownAttribute.InterfaceType);
        }

        int currentIndex = Mathf.Max(0, Array.IndexOf(_typeNames, property.stringValue));

        EditorGUI.BeginProperty(position, label, property);
        int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, _typeNames);
        if (selectedIndex >= 0 && selectedIndex < _implementingTypes.Count) {
            property.stringValue = _typeNames[selectedIndex];
        }
        EditorGUI.EndProperty();
    }

    private void LoadImplementingTypes(Type interfaceType) {
        _implementingTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();

        _typeNames = _implementingTypes.Select(t => t.FullName).ToArray();
    }
}
