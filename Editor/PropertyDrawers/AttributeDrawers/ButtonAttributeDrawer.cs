using _S.Attributes;
using UnityEditor;
using UnityEngine.UIElements;

namespace _S.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonAttributeDrawer : PropertyDrawer
    {
        int _previousIndex;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            //Button ReInitializeButton = new(() => (property.GetUnderlyingValue() as Action<>)?.Invoke());
            //ReInitializeButton.text = (attribute as ButtonAttribute).name;
            //root.Add(ReInitializeButton);
            return root;
        }
    }
}