using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float minSpawnDelay = 2.4f;
    [SerializeField] float maxSpawnDelay = 4.0f;
    [SerializeField] BaseEnemy enemyPrefab;
    [SerializeField] Transform spawnPos;



    [SerializeField] float timeUntilTierUpgrade = 30.0f;

    int currentTier = 1;
    private void Start()
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(UpgradeTier());
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
        yield return new WaitForSeconds(spawnDelay);
        BaseEnemy newEnemy = Instantiate(enemyPrefab);
        newEnemy.transform.position = spawnPos.position;
        StartCoroutine(SpawnEnemy());
        newEnemy.InitEnemy(currentTier);
    }

    IEnumerator UpgradeTier()
    {
        yield return new WaitForSeconds(timeUntilTierUpgrade * currentTier);
        currentTier += 1;
        Debug.Log("Upgrading to tier " + currentTier);
        StartCoroutine(UpgradeTier());
    }


}
