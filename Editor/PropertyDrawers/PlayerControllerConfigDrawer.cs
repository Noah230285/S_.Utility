using _S.Configuration;
using _S.Editor.UIToolkitExtras;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace _S.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(PlayerControllerConfig))]
    public class PlayerControllerConfigDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var basePanel = new VisualElement().LinkedAddClass("largePanel");
            var field = new ObjectField("Config").LinkedBindProperty(property);
            field.RegisterValueChangedCallback((x) => UIToolkitUtility.UpdateConfig(basePanel, x));
            basePanel.Add(field);
            UIToolkitUtility.AddSections(basePanel, property.objectReferenceValue);
            return basePanel;
        }
    }
}