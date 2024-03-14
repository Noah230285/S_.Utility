using System;
using UnityEngine;

namespace _S.ScriptableVariables
{
    [CreateAssetMenu(menuName = "Scriptable Variables/Bool")]
    [Serializable]
    public class BoolVariable : ScriptableVariable
    {
        public bool Value;
    }

    [Serializable]
    public class BoolReference : VariableReference
    {
        [SerializeField] bool _constantValue;
        [SerializeField] BoolVariable _variable;

        public bool value
        {
            get { return _useConstant || _variable == null? _constantValue : _variable.Value; }
        }
    }
}