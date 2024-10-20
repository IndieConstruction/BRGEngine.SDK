using UnityEngine;

namespace BRGEngine.Core
{
    #region Inline Editing Espandable

    public class AttributesAttribute : PropertyAttribute
    {
        // You can add any parameters if you want to further customize
    }

    public class HideIfAttribute : PropertyAttribute {
        public string ConditionField { get; private set; }
        public bool DesiredCondition { get; private set; }

        public HideIfAttribute(string conditionField, bool desiredCondition) {
            ConditionField = conditionField;
            DesiredCondition = desiredCondition;
        }
    }

    #endregion

    #region Show Hide

    public class ShowIfAttribute : PropertyAttribute {
        public string ConditionField { get; private set; }
        public bool DesiredCondition { get; private set; }

        public ShowIfAttribute(string conditionField, bool desiredCondition) {
            ConditionField = conditionField;
            DesiredCondition = desiredCondition;
        }
    }

    #endregion
}