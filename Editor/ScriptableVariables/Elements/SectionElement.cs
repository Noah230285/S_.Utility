using _S.Editor.UIToolkitExtras;
using UnityEditor;
using UnityEngine.UIElements;

namespace _S.Editor.UXMLElements
{
    public class SectionElement : VisualElement
    {
        public Label label;
        public VisualElement arrow;
        public VisualElement content;

        public SectionElement(string name, SerializedProperty extended)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Packages/com.gameinvader.utility/Assets/Editor/UIToolkit/UXML/Templates/Section.uxml");
            asset.CloneTree(this);
            this.AddToClassList("panel");
            this.ElementAt(0).MergeIntoParent();

            label = this.ElementAt(0) as Label;
            arrow = this.ElementAt(0).ElementAt(0);
            content = this.ElementAt(1);

            label.text = name;

            label.RegisterCallback<ClickEvent>((x) => UIToolkitUtility.FlipFlopProperty(extended));
            label.RegisterCallback<ClickEvent>((x) => SetExtended(extended));
            SetExtended(extended);
        }

        public void SetExtended(SerializedProperty extended)
        {
            if (extended.boolValue)
            {
                content.RemoveFromClassList("hidden");
                arrow.AddToClassList("rotate90Anim");
            }
            else
            {
                content.AddToClassList("hidden");
                arrow.RemoveFromClassList("rotate90Anim");
            }
            extended.serializedObject.ApplyModifiedProperties();
        }

        public SectionElement LinkedAddContent(VisualElement element)
        {
            content.LinkedAdd(element);
            return this;
        }

    }
}