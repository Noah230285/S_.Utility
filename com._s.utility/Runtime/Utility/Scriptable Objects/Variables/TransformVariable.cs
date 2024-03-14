using System;
using UnityEngine;

namespace _S.ScriptableVariables
{
    [CreateAssetMenu(menuName = "Scriptable Variables/Transform")]
    [Serializable]
    public class TransformVariable : ScriptableVariable
    {
        public Transform Value;
        public Transform value
        {
            get { return Value;}
            set { Value = value;}
        }
    }

    [Serializable]
    public class TransformReference : VariableReference
    {
        [SerializeField] Transform _constantValue;
        [SerializeField] TransformVariable _variable;

        public Transform value
        {
            get { return _useConstant || _variable == null ? _constantValue : _variable.Value; }
            set
            {
                if (_useConstant)
                {
                    _constantValue = value;
                }
                else
                {
                    _variable.Value = value;
                }
            }
        }
    }
}