using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace _S.Editor.UXMLFormatters
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
    }
}