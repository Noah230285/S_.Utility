using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using _S.Objectives;
public class ObjectiveView : Node
{
    public Action<ObjectiveView> OnObjectiveSelected;
    public Objective objective;
    public Port input;
    public Port output;

    public ObjectiveView(Objective objective) : base("Assets/Editor/UIToolkit/UXML/Templates/ObjectiveTree/ObjectiveView.UXML")
    {
        this.objective = objective;
        title = objective.name;
        string dummyTitle = (objective.name.Substring(objective.name.Length - 7, 7) == "(Clone)") ? objective.name.Substring(0, objective.name.Length - 7) : objective.name;
        dummyTitle = (dummyTitle.Substring(dummyTitle.Length - 4, 4) == "Node") ? objective.name.Substring(0, dummyTitle.Length - 4) : dummyTitle;

        title = System.Text.RegularExpressions.Regex.Replace(dummyTitle, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1").Trim();
        viewDataKey = objective.Guid;

        style.left = objective.Position.x;
        style.top = objective.Position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "ObjectiveText";
        descriptionLabel.Bind(new SerializedObject(objective));
    }

    private void SetupClasses()
    {
        if (objective is ConditionalObjective)
        {
            AddToClassList("composite");
        }
        else if (objective is EventObjective)
        {
            AddToClassList("action");
        }
        else if (objective is RootObjective)
        {
            AddToClassList("root");
        }
    }

    private void CreateInputPorts()
    {
        if (!(objective is RootObjective))
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }

        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {

        if (objective is ConditionalObjective)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (objective is RootObjective)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;  
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(objective, "Objective Tree (Set Position)");
        objective.Position.x = newPos.xMin;
        objective.Position.y = newPos.yMin;
        EditorUtility.SetDirty(objective);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnObjectiveSelected?.Invoke(this);
    }

    public void SortChildren()
    {
        ConditionalObjective composite = objective as ConditionalObjective;
        if (composite)
        {
            composite.Conditions.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(Objective left, Objective right)
    {
        return left.Position.x < right.Position.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");
        if (Application.isPlaying)
        {
            switch (objective.State)
            {
                case Objective.CompletionState.InProgress:
                    AddToClassList("running");
                    break;
                case Objective.CompletionState.Failed:
                    AddToClassList("failure");
                    break;
                case Objective.CompletionState.Succeeded:
                    AddToClassList("success");
                    break;
            }
        }
    }
}
