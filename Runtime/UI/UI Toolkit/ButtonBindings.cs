using System;
using UnityEngine.Events;

namespace _S.UIToolkit
{
    [Serializable]
    public struct ButtonBindings
    {
        public string Name;
        public UnityEvent ClickEvents;
    }
}