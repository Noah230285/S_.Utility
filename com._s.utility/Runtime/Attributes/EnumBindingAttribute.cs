using UnityEngine;

namespace _S.Attributes
{
    public class EnumBindingAttribute : PropertyAttribute
    {
        public string[] PropertyNames;
        public EnumBindingAttribute(string[] propertyNames)
        {
            PropertyNames = propertyNames;
        }
    }
}