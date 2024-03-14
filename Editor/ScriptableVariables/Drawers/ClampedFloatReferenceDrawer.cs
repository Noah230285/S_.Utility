using _S.Editor.UXMLElements;
using _S.ScriptableVariables;
using UnityEditor;
using UnityEngine.UIElements;

namespace _S.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ClampedFloatReference))]
    public class ClampedFloatReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ClampedFloatElement element = new(property);
            return element;
        }
    }
}