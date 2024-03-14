using _S.ScriptableVariables;
using _S.Utility.Broadcasting;
using UnityEngine;

public class ResetGameValues : MonoBehaviour
{
    public static ResetGameValues Instace
    {
        get; private set;
    }
    [SerializeField] BoolVariable[] _resetBoolVariables;
    [SerializeField] ScriptableEventChannel[] _resetEvents;
    [SerializeField] ClampedFloatVariable[] _resetClampMax;
    [SerializeField] ClampedFloatVariable[] _resetClampZero;

    void Awake()
    {
        if (Instace == null)
        {
            Instace = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public void ResetValues()
    {
        for (int i = 0; i < _resetBoolVariables.Length; i++)
        {
            _resetBoolVariables[i].Value = false;
        }

        for (int i = 0; i < _resetEvents.Length; i++)
        {
            _resetEvents[i].OnRaiseEvents();
        }

        for (int i = 0; i < _resetClampMax.Length; i++)
        {
            _resetClampMax[i].current = _resetClampMax[i].max;
        }

        for (int i = 0; i < _resetClampZero.Length; i++)
        {
            _resetClampZero[i].current = _resetClampZero[i].max;
        }

        Cursor.lockState = CursorLockMode.Confined;
    }
}
