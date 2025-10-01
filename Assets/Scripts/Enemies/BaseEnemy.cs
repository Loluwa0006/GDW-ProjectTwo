
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

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
    public UnityEvent<BaseEnemy> enemyDefeated = new();
    public Collider enemyCollider;



   public class CCInfo
    {
        public CrowdControl cc;
        public float amount;
        public float duration;

        public CCInfo(float amount, float duration, CrowdControl cc )
        {
            this.amount = amount;
            this.duration = duration;
            this.cc = cc;
        }
    }
   public enum CrowdControl
    {
        Slow,
        Stun,
        Weaken
    }

    [SerializeField] List<TierInfo> tiers = new ();

    [SerializeField] NavMeshAgent agent;


    [SerializeField] GameObject healthBarOver;
    [SerializeField] GameObject healthBarUnder;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spawnSFX;
    [SerializeField] AudioClip hurtSFX;

    [SerializeField] ParticleSystem deathParticles;


    Dictionary<string, CCInfo> ccDictionary = new();
    TierInfo currentTier;
        


    int health = 0;
    GameManager manager;
    float attackTracker = 0.0f;

    float originalHealthBarSize;

    private void Awake()
    {
        currentTier = tiers[0];
        originalHealthBarSize = healthBarOver.transform.localScale.x;
    }

    private void Start()
    {
        deathParticles.Stop();
    }


    public void FixedUpdate()
    {
        if (!EnemyStunned())
        {
            if (attackTracker > 0.0f)
            {
                attackTracker -= Time.deltaTime;
                if (attackTracker < 0.0f) { attackTracker = 0.0f; }
            }
        }

        var effectiveSpeed = currentTier.speed;

        foreach (var ccSource in ccDictionary.Keys.ToList())
        {
            switch (ccDictionary[ccSource].cc)
            {
                case CrowdControl.Slow:
                    float speedBefore = effectiveSpeed;
                    effectiveSpeed *= ( 1 - ccDictionary[ccSource].amount);
                    if (effectiveSpeed < 0.0f) effectiveSpeed = 0.0f;
                    Debug.Log("Speed being reduced from " + speedBefore + " to new speed "  + effectiveSpeed);
                    break;
                case CrowdControl.Stun:
                    effectiveSpeed = 0.0f;
                    Debug.Log("Enemy being stunned for " + ccDictionary[ccSource].duration + " seconds");
                    break;
            }
            ccDictionary[ccSource].duration -= Time.deltaTime;
            if (ccDictionary[ccSource].duration < 0.0f)
            {
                ccDictionary.Remove(ccSource);
            }
        }
        agent.speed = effectiveSpeed;
    }

    public bool EnemyStunned()
    {
        foreach (var ccSource in ccDictionary.Keys.ToList())
        {
            if (ccDictionary[ccSource].cc == CrowdControl.Stun)
            {
                return true;
            }
        }
        return false;
    }
    public void SetGameManager(GameManager manager)
    {
        this.manager = manager;
    }
    public void InitEnemy(int tier)
    {
        if (tiers.Count < tier) { return; }
        currentTier = tiers[tier];
        health = currentTier.maxHealth;
        agent.speed = currentTier.speed;
        agent.SetDestination(manager.GetAreaNearTownHall());
        transform.LookAt( agent.destination );

        int rand = Random.Range(0, 10);
        if (rand == 5)
        {
            audioSource.PlayOneShot(spawnSFX);
        }
    }


    public int GetAttackDamage()
    {
        if (attackTracker > 0.0f) { return 0; }
        attackTracker = currentTier.attackCooldown;
        return currentTier.damage;
    }

    public int GetHealth()
    {
        return health;
    }

    public void Damage(int amount)
    {
        Debug.Log("Decreasing health of " + name + " from " + health + " to " + (health - amount));
        float bonusDMGAsPercent = 1.0f;
        foreach (var ccSource in ccDictionary.Keys.ToList())
        {
            switch (ccDictionary[ccSource].cc)
            {
                case CrowdControl.Weaken:
                    bonusDMGAsPercent += ccDictionary[ccSource].amount;
                    Debug.Log("Dealing " + ccDictionary[ccSource].amount + "% extra damage");
                    break;
            }
        }
        health -= Mathf.RoundToInt(amount * bonusDMGAsPercent);
        if (health <= 0)
        {
            health = 0;
            DestroyEnemy();
            return;
        }
        else
        {
            int rand = Random.Range(0, 5);
            if (rand == 0)
            {
                audioSource.PlayOneShot(hurtSFX);
            }
        }
            float healthAsPercent = (float)health / currentTier.maxHealth;
        Vector3 newScale = healthBarOver.transform.localScale;
        newScale.x = originalHealthBarSize * healthAsPercent;
        healthBarOver.transform.localScale = newScale;
        Vector3 newPos = healthBarOver.transform.localPosition;
        newPos.x = (originalHealthBarSize - newScale.x) / 2f;
        healthBarOver.transform.localPosition  = newPos;
    }

    void DestroyEnemy()
    {
        deathParticles.Play();
        manager.AddCoins(1);
        enemyDefeated.Invoke(this);
        gameObject.SetActive(false);
    }

    public void ApplyCC(CrowdControl cc, float amount, float duration, bool stacking, string source)
    {
        if (ccDictionary.ContainsKey(source) && !stacking) { return; }

        if (ccDictionary.ContainsKey(source))
        {
            ccDictionary[source].duration += duration;
        }
        else
        {
            ccDictionary.Add(source, new CCInfo(amount, duration, cc) );
        }
        Debug.Log("Added new cc type " + cc.ToString() + " from source " + source);

    }


}
