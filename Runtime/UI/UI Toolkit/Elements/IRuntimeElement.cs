using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public interface IRuntimeElement
{
    public UnityAction loadFinished { get; set; }
    public bool loaded { get; set; }
    public void LoadAssets(string address)
    {
#if UNITY_EDITOR
        var tree  = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(address);
        tree.CloneTree(this as VisualElement);
        loaded = true;
        loadFinished?.Invoke();
#else
        UILoader.instance.LoadAssetFromKey(address, OnLoadFinished);
        //var asset = UILoader..LoadAssetAsync<VisualTreeAsset>(address);
        //    asset.Completed += OnLoadFinished;
#endif
    }
    void OnLoadFinished(AsyncOperationHandle<VisualTreeAsset> x)
    {
        var root = this as VisualElement;
        int count = root.childCount;
        VisualElement childContainer = new();
        for (int i = 0; i < count; i++)
        {
            childContainer.Add(root.ElementAt(0));
        }
        x.Result.CloneTree(root);
        root.Add(childContainer);
        loaded = true;
        loadFinished?.Invoke();
    }
}
