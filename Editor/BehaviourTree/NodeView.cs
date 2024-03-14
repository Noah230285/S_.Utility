using _S.AI;
using _S.AI.CompositeNodes;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public _S.AI.Node node;
    public Port input;
    public Port output;

    public NodeView(_S.AI.Node node) : base("Assets/Editor/UIToolkit/UXML/Templates/BehaviourTree/NodeView.UXML")
    {
        this.node = node;
        title = node.name;
        string dummyTitle = (node.name.Substring(node.name.Length - 7, 7) == "(Clone)") ? node.name.Substring(0, node.name.Length - 7) : node.name;
        dummyTitle = (dummyTitle.Substring(dummyTitle.Length - 4, 4) == "Node") ? node.name.Substring(0, dummyTitle.Length - 4) : dummyTitle;

        title = System.Text.RegularExpressions.Regex.Replace(dummyTitle, "((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", " $1").Trim();
        viewDataKey = node.Guid;

        style.left = node.Position.x;
        style.top = node.Position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    private void SetupClasses()
    {
        if (node is ConditionNode)
        {
            AddToClassList("condition");
        }
        else if (node is ActionNode)
        {
            AddToClassList("action");
        }
        else if (node is CompositeNode)
        {
            AddToClassList("composite");
        }
        else if (node is DecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if (node is RootNode)
        {
            AddToClassList("root");
        }

    }

    private void CreateInputPorts()
    {
        if (node is ActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is CompositeNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is DecoratorNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (node is RootNode)
        {
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
        if (node is ActionNode)
        {
        }
        else if (node is CompositeNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (node is DecoratorNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (node is RootNode)
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
        Undo.RecordObject(node, "Behaviour Tree (Set Position)");
        node.Position.x = newPos.xMin;
        node.Position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    public void SortChildren()
    {
        CompositeNode composite = node as CompositeNode;
        if (composite)
        {
            composite.Children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(_S.AI.Node left, _S.AI.Node right)
    {
        return left.Position.x < right.Position.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("success");
        RemoveFromClassList("failure");
        if (Application.isPlaying && node.active)
        {
            switch (node.state)
            {
                case _S.AI.Node.State.RUNNING:
                    if (node.started)
                    {
                        AddToClassList("running");
                    }
                    break;
                case _S.AI.Node.State.SUCCESS:
                    AddToClassList("success");
                    break;
                case _S.AI.Node.State.FAILURE:
                    AddToClassList("failure");
                    break;
            }
        }
    }
}
