using _S.Editor.CustomEditors;
using UnityEditor;
using UnityEngine.UIElements;
using _S.Objectives;

[CustomEditor(typeof(EventObjective))]
public class EventObjectiveEditor : DefaultEditor
{
    EventObjective _base;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = base.CreateInspectorGUI();
        _base = target as EventObjective;
        Button raiseEventButton = new Button(_base.EventRaised);
        raiseEventButton.text = "Call This Objective";
        root.Add(raiseEventButton);
        return root;
    }
}