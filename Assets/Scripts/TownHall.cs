using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TownHall : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] TMP_Text healthDisplay;
    [SerializeField] GameManager gameManager;

    int health = 0;

    List<BaseEnemy> attackingEnemies = new();
    private void Awake()
    {
        health = maxHealth;
        healthDisplay.text = "Health: " + health;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.parent.TryGetComponent(out BaseEnemy enemy ))
        {
            attackingEnemies.Add(enemy);
            enemy.enemyDefeated.AddListener(OnAttackerDefeated);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.TryGetComponent(out BaseEnemy enemy))
        {
            OnAttackerDefeated(enemy);
        }
    }

    private void FixedUpdate()
    {
        foreach (var enemy in attackingEnemies)
        {
            health -= enemy.GetAttackDamage();
            if (health <= 0)
            {
                health = 0;
                gameManager.OnGameOver(false);
            }
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
