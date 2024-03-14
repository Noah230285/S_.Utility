using _S.ScriptableVariables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace _S.Editor.UXMLElements
{
    public class IntegerReferenceField : ScriptableVariableField<int>
    {
        #region
        public new class UxmlFactory : UxmlFactory<IntegerReferenceField> { }
        public IntegerReferenceField() : base(null, "IntegerEmpty")
        {
            _objectField.objectType = typeof(IntegerVariable);
        }
        #endregion

        public IntegerReferenceField(SerializedProperty variableReferenceProperty, string label = "") : base(variableReferenceProperty, label)
        {
            _objectField.objectType = typeof(IntegerVariable);
        }

        protected override BaseField<int> CreateField()
        {
            return new IntegerField(label);
        }
    }
}