using _S.ScriptableVariables;
using _S.Editor.UIToolkitExtras;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace _S.Editor.CustomEditors
{
    [CustomEditor(typeof(ScriptableVariable), true)]
    public class ScriptableVariableEditor : DefaultEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = base.CreateInspectorGUI();
            TextField descriptionField = new();
            descriptionField.multiline = true;
            descriptionField.AddToClassList("descriptionBox");
            descriptionField.BindProperty(serializedObject.FindProperty("_description"));
            root.RemoveAt(1);
            root.LinkedAdd(descriptionField, 1);
            return root;
        }
    }
}