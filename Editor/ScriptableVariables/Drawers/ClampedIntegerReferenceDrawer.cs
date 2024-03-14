using _S.Editor.UXMLElements;
using _S.ScriptableVariables;
using UnityEditor;
using UnityEngine.UIElements;

namespace _S.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ClampedIntegerReference))]
    public class ClampedIntegerReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ClampedIntegerElement element = new(property);
            return element;
        }
    }
}