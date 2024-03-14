using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using _S.Objectives;

namespace _S.Editor.UXMLFormatters
{
    public class ObjectiveInspectorView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<ObjectiveInspectorView, GraphView.UxmlTraits> { }

        UnityEditor.Editor editor;

        public ObjectiveInspectorView() { }

        internal void UpdateSelection(ObjectiveView objectiveView)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            if (objectiveView.objective is EventObjective)
            {
                editor = EventObjectiveEditor.CreateEditorWithContext(new UnityEngine.Object[] { objectiveView.objective as EventObjective }, null, typeof(EventObjectiveEditor));
            }
            else
            {
                editor = UnityEditor.Editor.CreateEditor(objectiveView.objective);
            }
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