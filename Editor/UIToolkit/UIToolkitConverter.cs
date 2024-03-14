using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

namespace PluginUtils
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class VEPropertyDrawerAttribute : Attribute
    {

    }

    public class VisualElementPropertyDrawerEditorBase : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            // Draw the legacy IMGUI base
            var imgui = new IMGUIContainer(OnInspectorGUI);
            container.Add(imgui);

            // Find all properties that are marked [HideInInspector] that have
            // a PropertyDrawer tagged with the [VEPropertyDrawer] attribute and create
            // PropertyFields for each of them.
            var type = target.GetType();
            // Create property fields.
            // Add fields to the container.
            CreatePropertyFields(container, type);
            return container;

        }

        protected void CreatePropertyFields(VisualElement container, Type objectType)
        {
            var fields = objectType.GetFields(
                  BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                var attr = fieldInfo.GetCustomAttribute<HideInInspector>();
                if (attr == null || !IsPropertyDrawerTagged(fieldInfo.FieldType))
                    continue;

                container.Add(
                    new PropertyField(serializedObject.FindProperty(fieldInfo.Name)));
            }
        }

        protected bool IsPropertyDrawerTagged(Type propertyType)
        {
            var drawerType = GetPropertyDrawerType(propertyType);
            if (drawerType == null)
                return false;

            var method = drawerType.GetMethod("CreatePropertyGUI",
                            BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, null,
                            new[] { typeof(SerializedProperty) }, null);
            return method != null;
        }


        /// <summary>
        /// Use Reflection to access ScriptAttributeUtility to find the
        /// PropertyDrawer type for a property type
        /// </summary>
        protected Type GetPropertyDrawerType(Type typeToDraw)
        {
            var scriptAttributeUtilityType = GetScriptAttributeUtilityType();

            var getDrawerTypeForTypeMethod =
                        scriptAttributeUtilityType.GetMethod(
                            "GetDrawerTypeForType",
                            BindingFlags.Static | BindingFlags.NonPublic, null,
                            new[] { typeof(Type) }, null);

            return (Type)getDrawerTypeForTypeMethod.Invoke(null, new[] { typeToDraw });
        }

        protected Type GetScriptAttributeUtilityType()
        {
            var asm = Array.Find(AppDomain.CurrentDomain.GetAssemblies(),
                                              (a) => a.GetName().Name == "UnityEditor");

            var types = asm.GetTypes();
            var type = Array.Find(types, (t) => t.Name == "ScriptAttributeUtility");

            return type;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

    }
}
