using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using _S.Attributes;

namespace _S.Objectives
{
    [CreateAssetMenu(fileName = "Objective Tree", menuName = "Objectives/Objective Tree")]
    public class ObjectiveTree : ScriptableObject
    {
        [HideInInspector] public RootObjective RootObjective;
        [ReadOnly] public List<Objective> Objectives = new();
        public int priority;
        public event Action<ObjectiveTree> Register;
        public event Action<ObjectiveTree> Remove;
        public Objective.CompletionState State;
        public virtual void OnRegister()
        {
            Register?.Invoke(this);
            RootObjective.StartObjective();
            RootObjective.CallParentEnded = ObjectiveCompleted;
        }
        public virtual void OnRemove()
        {
            RootObjective.ResetObjective();
            RootObjective.CallParentEnded = null;
            Remove?.Invoke(this);
        }

        void ObjectiveCompleted(Objective objective)
        {
            RootObjective.CallParentEnded = null;
            State = RootObjective.State;
            OnRemove();
        }

#if UNITY_EDITOR
        public Objective CreateObjective(System.Type type) 
        {
            Objective objective = ScriptableObject.CreateInstance(type) as Objective;
            objective.name = type.Name;
            objective.Guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Objective Tree (CreateObjective)");
            Objectives.Add(objective);
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(objective, this);
            }
            Undo.RegisterCreatedObjectUndo(objective, "Objective Tree (CreateObjective)");

            AssetDatabase.SaveAssets();
            return objective;
        }

        public void DeleteObjective(Objective objective)
        {
            Undo.RecordObject(this, "Objective Tree (DeleteObjective)");
            Objectives.Remove(objective);

            Undo.DestroyObjectImmediate(objective);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Objective parent, Objective child)
        {
            child.Parent = parent;
            RootObjective rootObjective = parent as RootObjective;
            if (rootObjective)
            {
                Undo.RecordObject(rootObjective, "Objective Tree (AddChild)");
                rootObjective.Child = child;
                EditorUtility.SetDirty(rootObjective);
            }

            ConditionalObjective composite = parent as ConditionalObjective;
            if (composite)
            {
                Undo.RecordObject(composite, "Objective Tree (AddChild)");
                composite.Conditions.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(Objective parent, Objective child)
        {
            child.Parent = null;
            RootObjective rootObjective = parent as RootObjective;
            if (rootObjective)
            {
                Undo.RecordObject(rootObjective, "Objective Tree (RemoveChild)");
                rootObjective.Child = null;
                EditorUtility.SetDirty(rootObjective);
            }

            ConditionalObjective composite = parent as ConditionalObjective;
            if (composite)
            {
                Undo.RecordObject(composite, "Objective Tree (RemoveChild)");
                composite.Conditions.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }
#endif

        public List<Objective> GetChildren(Objective parent)
        {
            List<Objective> children = new List<Objective>();

            RootObjective rootNode = parent as RootObjective;
            if (rootNode && rootNode.Child != null)
            {
                children.Add(rootNode.Child);
            }

            ConditionalObjective composite = parent as ConditionalObjective;
            if (composite)
            {
                return composite.Conditions;
            }

            return children;

        }
        public void Traverse(Objective objective, System.Action<Objective> visitor)
        {
            if (objective)
            {
                visitor.Invoke(objective);
                var children = GetChildren(objective);
                children.ForEach((n) => Traverse(n, visitor));
            }
        }

        public ObjectiveTree Clone()
        {
            ObjectiveTree tree = Instantiate(this);
            tree.RootObjective = tree.RootObjective.Clone() as RootObjective;
            tree.Objectives = new List<Objective>();
            Traverse(tree.RootObjective, (o) =>
            {
                tree.Objectives.Add(o);
            });
            return tree;
        }
    }
}