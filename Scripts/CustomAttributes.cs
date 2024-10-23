using System;
using UnityEngine;

namespace BRGEngine.Core
{
    #region Inline Editing Espandable

    public class InlineAttribute : PropertyAttribute
    {
        // You can add any parameters if you want to further customize
    }

    #endregion

    #region Show Hide

    public class HideIfAttribute : PropertyAttribute {
        public string ConditionField { get; private set; }
        public bool DesiredCondition { get; private set; }

        public HideIfAttribute(string conditionField, bool desiredCondition) {
            ConditionField = conditionField;
            DesiredCondition = desiredCondition;
        }
    }

    public class ShowIfAttribute : PropertyAttribute {
        public string ConditionField { get; private set; }
        public bool DesiredCondition { get; private set; }

        public ShowIfAttribute(string conditionField, bool desiredCondition) {
            ConditionField = conditionField;
            DesiredCondition = desiredCondition;
        }
    }

    #endregion

    #region Button

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute {
        public string ButtonText { get; }
        public string[] ParameterNames { get; }

        public ButtonAttribute(string buttonText, params string[] parameterNames) {
            ButtonText = buttonText;
            ParameterNames = parameterNames;
        }
    }

    #endregion
}