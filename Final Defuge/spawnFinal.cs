using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class spawnFinal : MonoBehaviour
{
    [SerializeField] private Transform enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] private List<Transform> spawnpoints;
    //[SerializeField] private Text waveCountdownText;
    private float countdown = 10f;
    private int waveNumber = 0;                                

    void Update()
    {
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }

        countdown -= Time.deltaTime;
        //waveCountdownText.text = Mathf.Round(countdown).ToString();
    }

    IEnumerator SpawnWave()
    {
        waveNumber++;
        for (int i = 0; i < waveNumber; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f);
        }

    }
    void SpawnEnemy()
    {
        for(int i = 0; i < spawnpoints.Count; i++)
        {
            Instantiate(enemyPrefab, spawnpoints[i].position, spawnpoints[i].rotation);
        }
    }
}
