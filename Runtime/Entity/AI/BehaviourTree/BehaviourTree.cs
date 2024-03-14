using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace _S.AI
{
    [CreateAssetMenu(fileName = "Behaviour Tree", menuName = "AI/Behaviour Tree")]
    public class BehaviourTree : ScriptableObject
    {
        public Node rootNode;
        public Node.State treeState = Node.State.RUNNING;
        public List<Node> nodes = new();
        public Blackboard blackboard = new();
        public AIAgent agent;

        public void Start()
        {
            rootNode.OnStart();
        }

        public void Stop()
        {
            rootNode.OnStop();
        }

        public Node.State Update()
        {
            foreach (var node in nodes)
            {
                node.active = false;
            }
            if (rootNode.state == Node.State.RUNNING)
            {
                treeState = rootNode.Update();
            }
            return treeState;
        }


#if UNITY_EDITOR
        public Node CreateNode(System.Type type) 
        {
            Node node = ScriptableObject.CreateInstance(type) as Node;
            node.name = type.Name;
            node.Guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            nodes.Add(node);
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(node, this);
            }
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            nodes.Remove(node);

            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
                decorator.Child = child;
                EditorUtility.SetDirty(decorator);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
                rootNode.Child = child;
                EditorUtility.SetDirty(rootNode);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.Children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
                decorator.Child = null;
                EditorUtility.SetDirty(decorator);
            }

            RootNode rootNode = parent as RootNode;
            if (rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
                rootNode.Child = null;
                EditorUtility.SetDirty(rootNode);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.Children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }
#endif

        public List<Node> GetChildren(Node parent)
        {
            List<Node> children = new List<Node>();

            DecoratorNode decorator = parent as DecoratorNode;
            if (decorator && decorator.Child != null)
            {
                children.Add(decorator.Child);
            }


            RootNode rootNode = parent as RootNode;
            if (rootNode && rootNode.Child != null)
            {
                children.Add(rootNode.Child);
            }

            CompositeNode composite = parent as CompositeNode;
            if (composite)
            {
                return composite.Children;
            }

            return children;

        }
        public void Traverse(Node node, System.Action<Node> visitor)
        {
            if (node)
            {
                visitor.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visitor));
            }
        }

        public BehaviourTree Clone()
        {
            BehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();
            Traverse(tree.rootNode, (n) =>
            {
                tree.nodes.Add(n);
            });
            return tree;
        }

        public void Bind(AIAgent agent)
        {
            Traverse(rootNode, node =>
            {
                node.agent = agent;
                node.blackboard = blackboard;
            });
        }
    }
}