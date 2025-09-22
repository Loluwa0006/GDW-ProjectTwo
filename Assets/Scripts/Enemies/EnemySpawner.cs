using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float minSpawnDelay = 2.4f;
    [SerializeField] float maxSpawnDelay = 4.0f;
    [SerializeField] BaseEnemy enemyPrefab;
    [SerializeField] Transform spawnPos;

    [SerializeField] float tierUntilSpawning = 0;


    [SerializeField] float timeUntilTierUpgrade = 30.0f;


    int currentTier = 1;

    bool spawnerActive = false;

    private void Start()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
        {
            gameManager.newWave.AddListener(OnWaveChanged);
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
        BaseEnemy newEnemy = Instantiate(enemyPrefab);
        newEnemy.InitEnemy(currentTier);
        newEnemy.transform.position = spawnPos.position;

        //StartCoroutine(SpawnEnemy());

    }




}
