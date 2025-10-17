using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;      // The enemy to spawn
    [SerializeField] private float spawnInterval = 5f;    // Time between spawns
    [SerializeField] private int maxPrefabEnemies = 10;         // max number of enemies (may handel in game manager instead

    private float timer;

    //private int enemiesSpawned = 0;

    void Update()
    {
        if (!enemyPrefab) return;

        timer += Time.deltaTime; // every frame increment the timer until spawn interval met
        if (timer < spawnInterval) return;
        timer = 0f; // reset the timer

        var enemyComponent = enemyPrefab.GetComponent<EnemyBase>();

        if (!enemyComponent)
        {
            Debug.LogWarning($"{name}: Prefab {enemyPrefab.name} has no EnemyBase component.");
            return;
        }
        Type t = enemyComponent.GetType();

        if (EnemyBase.GetCountByType(t) >= maxPrefabEnemies) return; // don't spawn past cap

        Vector3 pos = transform.position;
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}
