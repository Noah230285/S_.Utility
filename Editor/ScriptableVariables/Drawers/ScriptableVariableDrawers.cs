using _S.Attributes;
using _S.Editor.UXMLElements;
using _S.ScriptableVariables;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _S.Editor.PropertyDrawers
{
    public class VariableReferenceDrawer<T> : PropertyDrawer
    {
    }

    [CustomPropertyDrawer(typeof(BoolReference))]
    public class BoolReferenceDrawer : VariableReferenceDrawer<bool>
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            BoolReferenceField element = new(property, property.displayName);
            return element;
        }
    }
    [CustomPropertyDrawer(typeof(FloatReference))]
    public class FloatReferenceDrawer : VariableReferenceDrawer<float>
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            FloatReferenceField element = new(property, property.displayName);
            return element;
        }
    }
    [CustomPropertyDrawer(typeof(IntegerReference))]
    public class IntegerReferenceDrawer : VariableReferenceDrawer<int>
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            IntegerReferenceField element = new(property, property.displayName);
            return element;
        }
    }
    [CustomPropertyDrawer(typeof(TransformReference))]
    public class TransformReferenceDrawer : VariableReferenceDrawer<Transform>
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            TransformReferenceField element = new(property, property.displayName);
            return element;
        }
    }

    [CustomPropertyDrawer(typeof(ReadOnlyVariableAttribute))]
    public class ReadOnlyVariableAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var type = property.GetUnderlyingType();
            if (type.BaseType.Name == typeof(VariableReference).Name)
            {
                VisualElement element;
                switch (type.Name)
                {
                    //case "bool":
                    //    element = new ScriptableVariableField<bool>(property, property.displayName);
                    //    break;
                    case "FloatReference":
                        element = new FloatReferenceField(property, property.displayName);
                        (element as FloatReferenceField).SetReadOnly();
                        break;
                    //case "int":
                    //    element = new ScriptableVariableField<int>(property, property.displayName); 
                    //    break;
                    case "TransformReference":
                        element = new TransformReferenceField(property, property.displayName);
                        (element as TransformReferenceField).SetReadOnly();
                        break;
                    default:
                        element = null;
                        Debug.LogWarning($"Variable reference {type.BaseType.GetGenericArguments()[0].Name} has not been defined in ReadOnlyVariableAttribute");
                        break;
                }
                return element;
            }
            return null;
        }
    }
}