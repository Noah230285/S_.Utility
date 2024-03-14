using _S.Attributes;
using _S.Editor.UIToolkitExtras;
using _S.Editor.UXMLElements;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Assets.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SectionAttribute))]
    public class SectionAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SectionAttribute sectionattribute = (attribute as SectionAttribute);
            SectionElement element = new SectionElement(sectionattribute.Name, property);
            SerializedProperty previousProperty = property.FindPreviousProperty();
            foreach (var name in sectionattribute.PropertyNames)
            {
                if (previousProperty == null)
                {
                    element.LinkedAddContent(new PropertyField(property.serializedObject.FindPropertyOrFail(name)));
                }
                else
                {
                    element.LinkedAddContent(new PropertyField(previousProperty.FindPropertyRelativeOrFail(name)));
                }
            }
            return element;
        }
    }
}