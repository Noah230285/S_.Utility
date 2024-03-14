using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace _S.Editor.CustomEditors
{
    [CustomEditor(typeof(Object), true, isFallback = true)]
    public class DefaultEditor : UnityEditor.Editor
    {
        protected StyleSheet defaultStyles;

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            var iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(true))
            {
                do
                {
                    defaultStyles = AssetDatabase.LoadAssetAtPath<StyleSheet>
        ("Packages/com.gameinvader.utility/Assets/Editor/UIToolkit/Styles/Main.uss");
                    container.styleSheets.Add(defaultStyles);

                    var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };
                    propertyField.Bind(serializedObject);
                    if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                        propertyField.SetEnabled(value: false);

                    container.Add(propertyField);
                }
                while (iterator.NextVisible(false));
            }

            return container;
        }
    }
}
