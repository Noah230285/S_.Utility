using System;
using System.Collections.Generic;
using UnityEngine;

namespace _S.Objectives
{
    [CreateAssetMenu(menuName = "Utility/ObjectiveManager")]
    public class ObjectiveManager : ScriptableObject
    {
        [SerializeField] List<ObjectiveTree> _objectives;
        public List<ObjectiveTree> objectives
        {
            get => _objectives;
            set => _objectives = value;
        }

        public void AddObjective(ObjectiveTree add)
        {
            AddedObjective?.Invoke(add);
            _objectives.Add(add);
            add.OnRegister();
        }

        public void RemoveObjective(ObjectiveTree remove)
        {
            RemovedObjective?.Invoke(remove);
            _objectives.Remove(remove);
            remove.OnRemove();
        }

        public event Action<ObjectiveTree> AddedObjective;
        public event Action<ObjectiveTree> RemovedObjective;
    }

}
