using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections.Generic;

public class BaseEnemy : MonoBehaviour
{
    [System.Serializable]
    public class TierInfo
    {
        public int damage = 3;
        public int maxHealth = 10;
        public float attackCooldown = 1.2f;
        public float speed = 1.0f;
    }

    [SerializeField] List<TierInfo> tiers = new List<TierInfo>();

    TierInfo currentTier;
        
    public UnityEvent<BaseEnemy> enemyDefeated = new();
    public Collider enemyCollider;

    float attackTracker = 0.0f;

    int health = 0;

    NavMeshAgent agent;

    GameManager manager;

    GameObject target;


    [SerializeField] GameObject healthBarOver;
    [SerializeField] GameObject healthBarUnder;

    float originalHealthBarSize;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        manager = FindFirstObjectByType<GameManager>();
        enemyCollider = GetComponent<Collider>();
        target = manager.townHall;

        agent.SetDestination(target.transform.position);
        currentTier = tiers[0];
        originalHealthBarSize = healthBarOver.transform.localScale.x;
    }


    public void FixedUpdate()
    {
        if (attackTracker > 0.0f)
        {
            attackTracker -= Time.deltaTime;
            if (attackTracker < 0.0f ) { attackTracker = 0.0f; }
        }
    }

    public void InitEnemy(int tier)
    {
        if (tiers.Count < tier) { return; }
        currentTier = tiers[tier];
        health = currentTier.maxHealth;
        agent.speed = currentTier.speed;
    }


    public int GetAttackDamage()
    {
        if (attackTracker > 0.0f) { return 0; }
        attackTracker = currentTier.attackCooldown;
        return currentTier.damage;
    }


    public void Damage(int amount)
    {
        Debug.Log("Decreasing health of " + name + " from " + health + " to " + (health - amount));
        health -= amount;
        if (health < 0)
        {
            health = 0;
            StartCoroutine(DestroyEnemy());
        }
        float healthAsPercent = (float) health / currentTier.maxHealth;
        Vector3 newScale = healthBarOver.transform.localScale;
        newScale.x = originalHealthBarSize * healthAsPercent;
        healthBarOver.transform.localScale = newScale;
        Vector3 newPos = healthBarOver.transform.localPosition;
        newPos.x = (originalHealthBarSize - newScale.x) / 2f;
        healthBarOver.transform.localPosition  = newPos;
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
