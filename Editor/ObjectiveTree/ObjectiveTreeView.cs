using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using _S.Objectives;

namespace _S.Editor.UXMLFormatters
{
    public class ObjectiveTreeView : GraphView
    {
        public Action<ObjectiveView> OnObjectiveSelected;
        public new class UxmlFactory : UxmlFactory<ObjectiveTreeView, GraphView.UxmlTraits> { }
        ObjectiveTree tree;
        public ObjectiveTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.gameinvader.utility/Assets/Editor/UIToolkit/UXML/BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopualteView(tree);
            AssetDatabase.SaveAssets();
        }

        ObjectiveView FindObjectiveView(Objective objective)
        {
            return GetNodeByGuid(objective.Guid) as ObjectiveView;
        }

        internal void PopualteView(ObjectiveTree tree)
        {
            this.tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (tree.RootObjective == null)
            {
                tree.RootObjective = tree.CreateObjective(typeof(RootObjective)) as RootObjective;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }



            tree.Objectives.ForEach(o => CreateObjectiveView(o));

            tree.Objectives.ForEach(o =>
            {
                var children = tree.GetChildren(o);
                children.ForEach(c =>
                {
                    ObjectiveView parentView = FindObjectiveView(o);
                    ObjectiveView childView = FindObjectiveView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endport =>
            endport.direction != startPort.direction &&
            endport.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    ObjectiveView objectiveView = elem as ObjectiveView;
                    if (objectiveView != null)
                    {
                        tree.DeleteObjective(objectiveView.objective);
                    }

                    Edge edge = elem as Edge;
                    if (edge != null)
                    {
                        ObjectiveView parentView = edge.output.node as ObjectiveView;
                        ObjectiveView childView = edge.input.node as ObjectiveView;
                        tree.RemoveChild(parentView.objective, childView.objective);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    ObjectiveView parentView = edge.output.node as ObjectiveView;
                    ObjectiveView childView = edge.input.node as ObjectiveView;
                    tree.AddChild(parentView.objective, childView.objective);
                });
            }

            if (graphViewChange.movedElements != null)
            {
                nodes.ForEach((o) =>
                {
                    ObjectiveView view = o as ObjectiveView;
                    view.SortChildren();
                });
            }

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var mousePos = evt.localMousePosition;
            var target = evt.target;
            {
                var types = TypeCache.GetTypesDerivedFrom<Objective>();
                {
                    var view = target as ObjectiveView;
                    if (view != null)
                    {
                        evt.menu.AppendAction($"Find Objective In Project", (a) =>
                        {
                            EditorUtility.FocusProjectWindow();
                            Selection.activeObject = view.objective;
                            EditorGUIUtility.PingObject(view.objective);
                        });
                        var eventObjective = view.objective as EventObjective;
                        if (eventObjective != null)
                        {
                            evt.menu.AppendAction($"Call This Objective", (a) =>
                            {
                                eventObjective.EventRaised();
                            });
                        }
                    }
                }
                foreach (var type in types)
                {
                    if (type == typeof(ConditionalObjective) || type == typeof(RootObjective))
                    {
                        continue;
                    }
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateObjective(type, mousePos));
                }
            }
        }

        void CreateObjective(System.Type type, Vector2 position = new Vector2())
        {
            Objective objective = tree.CreateObjective(type);
            CreateObjectiveView(objective, position);
        }

        void CreateObjectiveView(Objective objective, Vector2 position = new Vector2())
        {
            ObjectiveView objectiveView = new ObjectiveView(objective);
            if (position != Vector2.zero)
            {
                Vector2 actualGraphPosition = viewTransform.matrix.inverse.MultiplyPoint(position);
                objectiveView.SetPosition(new Rect(actualGraphPosition.x, actualGraphPosition.y, 0, 0));
            }
            objectiveView.OnObjectiveSelected = OnObjectiveSelected;
            AddElement(objectiveView);
        }

        public void UpdateNodeStates()
        {
            nodes.ForEach(n =>
            {
                ObjectiveView view = n as ObjectiveView;
                view.UpdateState();
            });
        }
    }
}
