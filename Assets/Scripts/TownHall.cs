using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TownHall : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] TMP_Text healthDisplay;

    int health = 0;

    List<BaseEnemy> attackingEnemies = new();


    private void Awake()
    {
        health = maxHealth;
        healthDisplay.text = "Health: " + health;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out BaseEnemy enemy ))
        {
            attackingEnemies.Add(enemy);
            enemy.enemyDefeated.AddListener(OnAttackerDefeated);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out BaseEnemy enemy))
        {
            OnAttackerDefeated(enemy);
        }
    }

    private void FixedUpdate()
    {
        foreach (var enemy in attackingEnemies)
        {
            health -= enemy.GetAttackDamage();
            healthDisplay.text = "Health: " + health;
        }
    }

    public void OnAttackerDefeated(BaseEnemy enemy)
    {
        if (attackingEnemies.Contains(enemy))
        {
            attackingEnemies.Remove(enemy);
        }
    }

}
