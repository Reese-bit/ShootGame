using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : Singleton<EnemyManager>
{
    public GameObject randomEnemy => enemyList.Count == 0 ? null : enemyList[Random.Range(0, enemyList.Count)];
    public int WaveNumber => waveNumber;
    public float TimeBetweenWaves => timeBetweenWaves; 
    
    [SerializeField]private bool spawnEnemy = true;
    [SerializeField]private GameObject waveUI;
    [SerializeField]private GameObject[] enemyPrefabs;
    [SerializeField]private float timeBetweenSpawns = 1f;
    [SerializeField]private float timeBetweenWaves = 1f;
    [SerializeField]private int minEnemyAmount = 4;
    [SerializeField]private int maxEnemyAmount = 10;

    [Header("----BOSS SETTINGS----")] 
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private int bossWaveNumber;

    private int waveNumber = 1;
    private int enemyAmount;

    private List<GameObject> enemyList; 
    
    private WaitForSeconds waitTimeBetweenSpawns; 
    private WaitForSeconds waitTimeBetweenWaves;
    private WaitUntil waitUntilNoEnemy;

    protected override void Awake()
    {
        base.Awake();
        enemyList = new List<GameObject>();
        waitTimeBetweenSpawns = new WaitForSeconds(timeBetweenSpawns);
        waitTimeBetweenWaves = new WaitForSeconds(timeBetweenWaves);
        //waitUntilNoEnemy = new WaitUntil(NoEnemy);
        waitUntilNoEnemy = new WaitUntil(() => enemyList.Count == 0);
    }

    // bool NoEnemy()
    // {
    //     return enemyList.Count == 0;
    // }

    IEnumerator Start()
    {
        while (spawnEnemy && GameManager.GameState != GameState.GameOver)
        {
            waveUI.SetActive(true);
            
            yield return waitTimeBetweenWaves;
            
            waveUI.SetActive(false);
            
            yield return StartCoroutine(nameof(RandomlySpawnCoroutine));
        }
    }

    IEnumerator RandomlySpawnCoroutine()
    {
        if (waveNumber % bossWaveNumber == 0)
        {
            // Spawn Boss
            var boss = PoolManager.Release(bossPrefab);
            enemyList.Add(boss);
        }
        else
        {
            //increase one enemy every two waves
            enemyAmount = Mathf.Clamp(enemyAmount, minEnemyAmount + waveNumber / 3, maxEnemyAmount);
            for (int i = 0; i < enemyAmount; i++)
            {
                //var enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                //PoolManager.Release(enemy);
                enemyList.Add(PoolManager.Release(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]));

                yield return waitTimeBetweenSpawns;
            }
        }

        // prevent enemy dead when just borned
        yield return waitUntilNoEnemy;

        waveNumber++;
    }

    public void RemoveFromList(GameObject enemy)
    {
        enemyList.Remove(enemy);
    }
}
