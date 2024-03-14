using UnityEngine;
using System;

namespace _S.ScriptableVariables
{
    [Serializable]
    public class ScriptableVariable : ScriptableObject
    {
        [SerializeField] string _description;
    }

    public class VariableReference
    {
        [SerializeField] protected bool _useConstant;
        //public event EventHandler ModifiedFromInspector;
    }
}