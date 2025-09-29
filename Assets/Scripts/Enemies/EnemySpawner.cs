using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    const int NUMBER_OF_ENEMIES_TO_LOAD = 200;
    [SerializeField] float minSpawnDelay = 2.4f;
    [SerializeField] float maxSpawnDelay = 4.0f;
    [SerializeField] BaseEnemy enemyPrefab;
    [SerializeField] Transform spawnPos;
    [SerializeField] GameManager gameManager;

    [SerializeField] float tierUntilSpawning = 0;

    int currentTier = 1;

    bool spawnerActive = false;

    Queue<BaseEnemy> loadedEnemies = new();
    private void Start()
    {
       if (gameManager == null)  gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.newWave.AddListener(OnWaveChanged);
        }
        StartCoroutine(InitEnemies());
    }

    IEnumerator InitEnemies()
    {
        for (int i = 0; i < NUMBER_OF_ENEMIES_TO_LOAD; i++)
        {
            if (i % 5 == 0) { yield return null; }
            BaseEnemy newEnemy = Instantiate(enemyPrefab, transform);
            newEnemy.SetGameManager(gameManager);
            newEnemy.gameObject.SetActive(false);
            loadedEnemies.Enqueue(newEnemy);
        }
    }


    public void OnWaveChanged(GameManager.WaveData waveData)
    {
        if (tierUntilSpawning == waveData.waveNumber && !spawnerActive)
        {
            Debug.Log("Starting enemy spawner " + name);
            StartCoroutine(SpawnEnemy());
            spawnerActive = true;
        }
        currentTier = waveData.waveNumber - 1; // account for arrays starting at 0;
        Debug.Log("Starting wave " + waveData.waveNumber + ", tier until spawning is " + tierUntilSpawning) ;
    }


    IEnumerator SpawnEnemy()
    {
        float spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        yield return new WaitForSeconds(spawnDelay);
        BaseEnemy newEnemy = loadedEnemies.Dequeue();
        newEnemy.gameObject.SetActive(true);
        loadedEnemies.Enqueue (newEnemy);
        newEnemy.InitEnemy(currentTier);
        newEnemy.transform.position = spawnPos.position;

        StartCoroutine(SpawnEnemy());

    }




}
