using UnityEngine;

namespace _S.Attributes
{
    public class SectionAttribute : PropertyAttribute
    {
        public string Name;
        public string[] PropertyNames;
        public SectionAttribute(string name, string[] propertyName)
        {
            Name = name;
            PropertyNames = propertyName;
        }
    }
}