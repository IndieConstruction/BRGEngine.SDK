using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class InterfaceDropdownAttribute : PropertyAttribute {
    public Type InterfaceType { get; private set; }

    public InterfaceDropdownAttribute(Type interfaceType) {
        if (!interfaceType.IsInterface) {
            Debug.LogError($"The type {interfaceType} is not an interface!");
            return;
        }
        InterfaceType = interfaceType;
    }
}
