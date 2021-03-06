using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnHorde : MonoBehaviour
{
    public Transform enemyPrefab;
    public Transform spawnPoint;
    public float timeBetweenWaves = 5f;
    private float countdown = 2f;

    private int waveNumber = 1;

    void Update()
    {
        if (countdown <= 0f)
        {
            SpawnWave();
            countdown = timeBetweenWaves;
        }

        countdown -= Time.deltaTime;
    }

    void SpawnWave()
    {
        Debug.Log("Wave Incoming");

        for (int i = 0; i < waveNumber; i++)
        {
            SpawnEnemy();
        }
        waveNumber++;
    }
    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
