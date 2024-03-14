using UnityEditor;
using UnityEngine.UIElements;

namespace _S.Editor.UXMLElements
{
    public class BoolReferenceField : ScriptableVariableField<bool>
    {
        #region
        public new class UxmlFactory : UxmlFactory<BoolReferenceField> { }
        public BoolReferenceField() : base(null, "")
        {
        }
        #endregion

        public BoolReferenceField(SerializedProperty variableReferenceProperty, string label = "") : base(variableReferenceProperty, label)
        {
        }

        protected override BaseField<bool> CreateField()
        {
            return new Toggle(label);
        }
    }
}
