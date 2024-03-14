using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.VisualScripting;

[CustomPropertyDrawer(typeof(Spawn))]
public class SpawnDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new();
        var EnableExisting = property.FindPropertyRelativeOrFail("EnableExisting");
        var EnableExistingField = new PropertyField(EnableExisting);
        root.Add(EnableExistingField);
        var ExistingEnemyField = new PropertyField(property.FindPropertyRelativeOrFail("ExistingEnemy"));
        root.Add(ExistingEnemyField);
        var newContainer = new VisualElement();
        newContainer.Add(new PropertyField(property.FindPropertyRelativeOrFail("Enemy")));
        newContainer.Add(new PropertyField(property.FindPropertyRelativeOrFail("SpawnPosition")));
        root.Add(newContainer);
        EnableExistingField.RegisterValueChangeCallback(x =>
        {
            if (EnableExisting.boolValue)
            {
                ExistingEnemyField.style.display = DisplayStyle.Flex;
                newContainer.style.display = DisplayStyle.None;
            }
            else
            {
                ExistingEnemyField.style.display = DisplayStyle.None;
                newContainer.style.display = DisplayStyle.Flex;
            }
        });
        return root;
    }
}