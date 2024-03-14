using _S.Editor.UXMLElements;
using _S.Utility;
using UnityEditor;
using UnityEngine.UIElements;

namespace _S.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Cooldown))]
    public class CooldownDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            CooldownElement element = new(property.displayName, property);
            return element;
        }
    }
}