using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PoisonTower : BaseTower
{

    const float WEAKEN_SCALER = 0.3f; // weakens by 30% for each power value
    const float DURATION_SCALER = 2.0f;

    [System.Serializable]
    class UpgradeData
    {
        public float attackSpeed = 8.2f;
        public int upgradeCost = 25;
        public int attackDamage = 30;
    }

    [SerializeField] List<UpgradeData> upgradeDataList = new();
    [SerializeField] EnemyDetector detector;

  

    [HideInInspector] public bool empowered = false;

    Dictionary<int, UpgradeData> upgradeDataDict = new();


    UpgradeData currentData;

    float attackCooldown = 0.0f;



    private void Awake()
    {
        int index = 1;
        foreach (var upgradeData in upgradeDataList)
        {
            upgradeDataDict[index] = upgradeData;
            index++;
        }
    }


    public override void InitTower()
    {
        base.InitTower();
        gameManager = FindFirstObjectByType<GameManager>();
        currentData = upgradeDataDict[1];

        detector.detectorArea.enabled = false;
        detector.mesh.enabled = true;

        attackCooldown = currentData.attackSpeed;

    }

    public override void OnTowerBuilt()
    {
        base.OnTowerBuilt();
        detector.detectorArea.enabled = true;
        detector.mesh.enabled = false;
        Debug.Log("Enabling detector area");
    }

    public override void OnTowerHighlighted()
    {
        base.OnTowerHighlighted();
        detector.mesh.enabled = true;
    }

    public override void OnTowerUnhighlighted()
    {
        base.OnTowerUnhighlighted();
        detector.mesh.enabled = false;
    }

    BaseEnemy GetHealthiestEnemy()
    {
        BaseEnemy enemy = null;
        int highestHealth = int.MinValue;
        foreach (var detected in detector.detectedEnemies.Keys.ToList())
        {
            if (detected.GetHealth() > highestHealth)
            {
                enemy = detected;
                highestHealth = detected.GetHealth();
            }
        }
        return enemy;
    }

    void Fire()
    {
        BaseEnemy enemy = GetHealthiestEnemy();

        if (!detector.detectedEnemies.ContainsKey(enemy)) { return; }

        Debug.Log("looking at enemy " + enemy.name);
        detector.detectedEnemies[enemy] += Time.deltaTime;
        if (detector.detectedEnemies[enemy] >= currentData.attackSpeed)
        {
            enemy.Damage(currentData.attackDamage);
            if (empowerValue > 0)
            {
                enemy.ApplyCC(
                    BaseEnemy.CrowdControl.Weaken,
                    empowerValue * WEAKEN_SCALER,
                    empowerValue * DURATION_SCALER,
                    false,
                    "EmpoweredPoisonTowerWeaken"
                    );
            }
            detector.detectedEnemies[enemy] = 0;
            Debug.Log("Hitting enemy " + enemy.name);
        }
    }

    public override void TowerLogic()
    {

        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0.0f)
        {
            attackCooldown = currentData.attackSpeed;
            Fire();
        }

    }
    public override bool TowerUpgradable()
    {
        if (currentTier == maxTier) { return false; }
        return gameManager.GetCoins() >= upgradeDataDict[currentTier + 1].upgradeCost;
    }

    public override void UpgradeTower()
    {
        base.UpgradeTower();
        currentData = upgradeDataDict[currentTier];
    }

    public override int GetNextUpgradeCost()
    {
        return upgradeDataDict[currentTier + 1].upgradeCost;
    }

    public override string GetDescriptionText()
    {
        return "Damage Per Second : " + (currentData.attackDamage / currentData.attackSpeed).ToString("#.00");
    }

}
