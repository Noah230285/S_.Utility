using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class UILoader
{
    static UILoader _instance;
    static public UILoader instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Creating new UILoader");
                _instance = new();
                return _instance;
            }
            return _instance;
        }
    }
    public bool isLoading => _operationDictionary == null ? false : _operationDictionary.Count != 0;

    public event Action LoadComplete;
    //public bool loaded => _assetsLoaded >= _assets;
    //int _assets;
    //int _assetsLoaded;

    struct AsyncWrapper
    {
        public AsyncOperationHandle<VisualTreeAsset> Operation;
        public Action<AsyncOperationHandle<VisualTreeAsset>> FinalComplete;
        public void Init(AsyncOperationHandle<VisualTreeAsset> operation, Action<AsyncOperationHandle<VisualTreeAsset>> finalComplete)
        {
            Operation = operation;
            FinalComplete = finalComplete;
            Operation.Completed += FinalComplete;
        }
        public void AddComplete(Action<AsyncOperationHandle<VisualTreeAsset>> action)
        {
            Operation.Completed -= FinalComplete;
            Operation.Completed += action;
            Operation.Completed += FinalComplete;
        }
    }
    Dictionary<string, AsyncWrapper> _operationDictionary = new();


    public void LoadAssetFromKey(string path, Action<AsyncOperationHandle<VisualTreeAsset>> completeAction)
    {
        if (_operationDictionary.ContainsKey(path))
        {
            _operationDictionary[path].AddComplete(completeAction);
            return;
        }
        var load = Addressables.LoadAssetAsync<VisualTreeAsset>(path);
        load.Completed += completeAction;
        AsyncWrapper wrapper = new AsyncWrapper();
        wrapper.Init(load, (operation) =>
        {
            _operationDictionary.Remove(path);
            if (_operationDictionary.Count == 0)
            {
                LoadComplete?.Invoke();
            }
        });
        _operationDictionary.Add(path, wrapper);
    }

    private void Awake()
    {
        //if (_visualTreeAddressables.) { }
        //_assets = _visualTreeAddressables.Length;
        //foreach (var addressable in _visualTreeAddressables)
        //{
        //    var assetLoad = Addressables.LoadAssetAsync<VisualTreeAsset>(addressable.Path);
        //    assetLoad.Completed += handle =>
        //    {
        //        addressable.LoadAddressable(handle);
        //        AssetLoaded();
        //    };
        //}
    }

    //void AssetLoaded()
    //{
    //    _assetsLoaded++;
    //    if (_assetsLoaded >= _assets)
    //    {
    //        foreach (var obj in _setActiveOnLoad)
    //        {
    //            obj.SetActive(true);
    //        }
    //    }
    //}

    //public void AddSetActive(GameObject obj)
    //{
    //    _setActiveOnLoad.Add(obj);
    //}
    //public VisualTreeAsset FindAssetAtPath(string path)
    //{
    //    foreach (var addressable in _visualTreeAddressables)
    //    {
    //        if (path == addressable.Path)
    //        {
    //            return addressable.asset;
    //        }
    //    }
    //    return null;
    //}
}
