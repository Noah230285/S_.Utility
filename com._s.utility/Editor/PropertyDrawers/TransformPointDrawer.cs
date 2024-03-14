using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace _S.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(MovementPath.MovePointWrapper.TransformPoint))]
    public class TransformPointDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_addFromPrevious")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_point")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_positionCurve")));
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(MovementPath.MovePointWrapper.PositionPoint))]
    public class PositionPointDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_addFromPrevious")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_position")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_speedCurve")));
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(MovementPath.MovePointWrapper.CurvePoint))]
    public class CurvePointDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_addFromPrevious")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_position")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_positionAsWeight")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_xCurve")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_yCurve")));
            root.Add(new PropertyField(property.FindPropertyRelativeOrFail("_zCurve")));
            return root;
        }
    }
}