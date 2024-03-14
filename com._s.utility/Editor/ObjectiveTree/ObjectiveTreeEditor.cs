using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using _S.Editor.UXMLFormatters;
using UnityEditor.Callbacks;
using _S.Objectives;

public class ObjectiveTreeEditor : EditorWindow
{
    ObjectiveTreeView treeView;
    ObjectiveInspectorView inspectorView;

    SerializedObject treeObject;
    ObjectiveTree tree;

    [MenuItem("BehaviourTreeEditor/ObjectiveTree ...")]
    public static void OpenWindow()
    {
        ObjectiveTreeEditor wnd = GetWindow<ObjectiveTreeEditor>();
        wnd.titleContent = new GUIContent("ObjectiveTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject is ObjectiveTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIToolkit/UXML/ObjectiveTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UIToolkit/UXML/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<ObjectiveTreeView>();
        inspectorView = root.Q<ObjectiveInspectorView>();

        treeView.OnObjectiveSelected = OnObjectiveSelectionChanged;
        OnSelectionChange();
    }

    void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
            default:
                break;
        }
    }

    private void OnSelectionChange()
    {
        ObjectiveTree tree = Selection.activeObject as ObjectiveTree;
        if (!tree)
        {
            //if (Selection.activeGameObject)
            //{
            //    BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
            //    if (runner)
            //    {
            //        tree = runner.tree;
            //    }
            //}
        }

        if (Application.isPlaying)
        {
            if (tree != null && treeView != null)
            {
                treeView.PopualteView(tree);
            }
        }
        else
        {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()) && treeView != null)
            {
                treeView.PopualteView(tree);
            }
        }

        if (tree != null)
        {
            treeObject = new SerializedObject(tree);
            //blackboardProperty = treeObject.FindProperty("blackboard");
        }
    }

    void OnObjectiveSelectionChanged(ObjectiveView objective)
    {
        inspectorView.UpdateSelection(objective);
    }

    void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }
}