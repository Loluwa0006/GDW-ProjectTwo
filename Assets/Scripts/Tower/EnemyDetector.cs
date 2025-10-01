using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent (typeof(Rigidbody))]
public class EnemyDetector : MonoBehaviour
{

    public Dictionary<BaseEnemy, float> detectedEnemies = new();

    public SphereCollider detectorArea;
    public MeshRenderer mesh;


    private void Start()
    {
        detectorArea.isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entity " + other.name + " entered");

        BaseEnemy enemy = other.transform.parent.GetComponent<BaseEnemy>();
        if (enemy == null) { return; }
        if (!detectedEnemies.Keys.Contains(enemy))
        {
            Debug.Log("detected enemy " + enemy);
            AddEnemyToDictionary(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Object " + other.name + " exited");

        BaseEnemy enemy = other.transform.parent.GetComponent<BaseEnemy>();
        if (enemy == null) { return; }
        if (detectedEnemies.Keys.Contains(enemy))
        {
            Debug.Log("enemy " + enemy + " exited");
            detectedEnemies.Remove(enemy);
        }
    }

    public void AddEnemyToDictionary(BaseEnemy enemy)
    {
        detectedEnemies.Add(enemy, 0.0f);
        enemy.enemyDefeated.AddListener(OnEnemyDefeated);
    }

    void OnEnemyDefeated(BaseEnemy enemy)
    {
        if (detectedEnemies.ContainsKey(enemy))
        {
            detectedEnemies.Remove(enemy);
        }
    }


}
