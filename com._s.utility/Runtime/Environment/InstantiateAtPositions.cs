using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Pool;



#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor;
using Unity.VisualScripting;
#endif

//public class InstantiateAtPositions : MonoBehaviour
//{
//    [SerializeField] Transform _parent;
//    [SerializeField] GameObject _defaultPrefab;
//    [SerializeField] GlobalObjectPool _defaultPool;

//    [Serializable]
//    public struct Spawn
//    {
//        public Transform Point;
//        public GameObject Prefab;
//        public GlobalObjectPool Pool;
//    }
//    [SerializeField] Spawn[][] spawnGroups;
//    public void SpawnGroup(int index)
//    {
//        for (int i = 0; i < spawnGroups[index].Length; i++)
//        {
//            if (spawnGroups[index][i].Point != null)
//            {
//                var prefab = spawnGroups[index][i].Prefab ?? _defaultPrefab;
//                var pool = spawnGroups[index][i].Pool ?? _defaultPool;

//                GameObject newObject = _defaultPool.Get();
//                newObject.transform.position = spawnGroups[index][i].Point.position;
//                newObject.AddComponent<ReturnToPool>().Pool = pool.objectPool;
//            }
//        }
//    }

//}


//#if UNITY_EDITOR
//[CustomPropertyDrawer(typeof(Spawn))]
//public class SpawnDrawer : PropertyDrawer
//{
//    public override VisualElement CreatePropertyGUI(SerializedProperty property)
//    {
//        VisualElement root = new();
//        root.Add(new PropertyField(property.FindPropertyRelativeOrFail("Point")));
//        root.Add(new PropertyField(property.FindPropertyRelativeOrFail("Prefab")));
//        root.Add(new PropertyField(property.FindPropertyRelativeOrFail("Pool")));
//        return root;
//    }
//}
//#endif