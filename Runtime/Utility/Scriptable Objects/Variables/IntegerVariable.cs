using System;
using UnityEngine;

namespace _S.ScriptableVariables
{
    [CreateAssetMenu(menuName = "Scriptable Variables/Integer")]
    [Serializable]
    public class IntegerVariable : ScriptableVariable
    {
        public int Value;
    }

    [Serializable]
    public class IntegerReference : VariableReference
    {
        [SerializeField] int _constantValue;
        [SerializeField] IntegerVariable _variable;

        public int value
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