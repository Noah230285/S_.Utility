using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using _S.UIToolkit;

public class SliderBindingsDrawer : MonoBehaviour
{
    [CustomPropertyDrawer(typeof(SliderBindings.AudioMixerValues))]
    public class AudioMixerValuesDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("Mixer")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("ValueName")));
            return root;
        }
    }
}
