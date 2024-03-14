using _S.Hitboxes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace _S.Editor.CustomEditors
{
    [CustomEditor(typeof(CallEventsHitbox))]
    public class HitboxBehavioursEditor : DefaultEditor
    {
        PropertyField _callExitField;
        PropertyField _exitEventsField;

        SerializedProperty _callExitProperty;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = base.CreateInspectorGUI();
            _callExitProperty = serializedObject.FindProperty("_callExit");
            _callExitField = root.Q<PropertyField>("PropertyField:_callExit");
            _exitEventsField = root.Q<PropertyField>("PropertyField:ExitEvents");

            _callExitField.RegisterValueChangeCallback((x) => CallExitUpdated());
            CallExitUpdated();
            return root;
        }
        void CallExitUpdated()
        {
            _exitEventsField.SetEnabled(_callExitProperty.boolValue);
        }
    }
}