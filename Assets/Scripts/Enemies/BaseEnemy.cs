using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] int damage = 3;
    [SerializeField] int health = 10;
    [SerializeField] float attackCooldown = 1.2f;

    public UnityEvent<BaseEnemy> enemyDefeated = new();

    float attackTracker = 0.0f;

    NavMeshAgent agent;

    GameManager manager;

    GameObject target;



    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        manager = FindFirstObjectByType<GameManager>();
        target = manager.townHall;

        agent.SetDestination(target.transform.position);
    }


    public void FixedUpdate()
    {
        if (attackTracker > 0.0f)
        {
            attackTracker -= Time.deltaTime;
            if (attackTracker < 0.0f ) { attackTracker = 0.0f; }
        }
    }

    public int GetAttackDamage()
    {
        if (attackTracker > 0.0f) { return 0; }
        attackTracker = attackCooldown;
        return damage;
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            health = 0;
            StartCoroutine(DestroyEnemy());
        }
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForFixedUpdate();
        enemyDefeated.Invoke(this);
        gameObject.SetActive(false);
        float waitDuration = Random.Range(0.7f, 1.3f);
        yield return new WaitForSeconds(waitDuration);
        Destroy(this.gameObject);
    }



}
