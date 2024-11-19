using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerPSXL : MonoBehaviour
{

    public GameObject[] enemyPrefab;
    private BoxCollider boxCollider;

    bool followPointLock = true;
    
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        
    }

    // ------------------------------------------- Spawn Enemy

    public void Spawn_Enemy()
    {
        Vector3 randomPosition = GetRandomPointInBox();

        int randomNum = Random.Range(0, enemyPrefab.Length);
            
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab[randomNum], randomPosition, Quaternion.identity);
        }
    }

    // ------------------------------------------- Get Random Point

    Vector3 GetRandomPointInBox()
    {
        Vector3 size = boxCollider.size;
        Vector3 center = boxCollider.center;

        Vector3 worldCenter = boxCollider.transform.TransformPoint(center);
        Vector3 halfExtents = size * 0.5f;

        float randomX = Random.Range(-halfExtents.x, halfExtents.x);
        float randomY = Random.Range(-halfExtents.y, halfExtents.y);
        float randomZ = Random.Range(-halfExtents.z, halfExtents.z);

        Vector3 localRandomPosition = new Vector3(randomX, randomY, randomZ);
        Vector3 worldRandomPosition = boxCollider.transform.TransformPoint(localRandomPosition);

        return worldRandomPosition;
    }
}
