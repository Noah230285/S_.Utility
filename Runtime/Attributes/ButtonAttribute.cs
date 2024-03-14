using UnityEngine;

namespace _S.Attributes
{
    public class ButtonAttribute : PropertyAttribute
    {
        public string name { get; private set; }
        public ButtonAttribute(string name)
        {
            this.name = name;
        }
    }
}