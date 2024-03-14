using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

namespace _S.Editor.UXMLFormatters
{
    public class InspectorView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, GraphView.UxmlTraits> { }

        UnityEditor.Editor editor;

        public InspectorView() { }

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);
            editor = UnityEditor.Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}