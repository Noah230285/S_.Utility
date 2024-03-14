using System;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] EnemiesWave[] _spawnGroups;
    [SerializeField] PrefabContainer _enemyPrefabs;
    [SerializeField] Transform _enemyParent;
    [SerializeField] bool _spawnFirstOnEnable;

    int currentWave;
    ObjectPool<GameObject> MeleeEnemies;
    ObjectPool<GameObject> RifleEnemies;
    ObjectPool<GameObject> BossEnemies;

    public void OnEnable()
    {
        if (_spawnFirstOnEnable)
        {
            NextWave();
        }
    }
    public void NextWave()
    {
        for (int i = 0; i < _spawnGroups[currentWave].Enemies.Length; i++)
        {
            GameObject Enemy = null;
            if (_spawnGroups[currentWave].Enemies[i].EnableExisting)
            {
                _spawnGroups[currentWave].Enemies[i].ExistingEnemy.SetActive(true);
            }
            else
            {
                switch (_spawnGroups[currentWave].Enemies[i].Enemy)
                {
                    case Spawn.EnemyType.Melee:
                        Enemy = Instantiate(_enemyPrefabs.Prefabs[0], _enemyParent.parent);
                        break;
                    case Spawn.EnemyType.Rifle:
                        Enemy = Instantiate(_enemyPrefabs.Prefabs[1], _enemyParent.parent);
                        break;
                    case Spawn.EnemyType.Boss:
                        Enemy = Instantiate(_enemyPrefabs.Prefabs[2], _enemyParent.parent);
                        break;
                    default:
                        break;
                }
                Transform spawnTransform = _spawnGroups[currentWave].Enemies[i].SpawnPosition;
                Enemy.transform.position = spawnTransform.position;
                Enemy.transform.rotation = spawnTransform.rotation;
            }
        }
        currentWave++;
    }
}

[Serializable]
public struct EnemiesWave
{
    public Spawn[] Enemies;
}
[Serializable]
public struct Spawn
{
    public bool EnableExisting;
    public GameObject ExistingEnemy;
    public enum EnemyType
    {
        Melee,
        Rifle,
        Boss
    }
    public EnemyType Enemy;
    public Transform SpawnPosition;
}