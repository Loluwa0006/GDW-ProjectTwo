using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LightingTower : BaseTower 
{

    const float SLOW_SCALER = 0.1f; // slows by 10% for each power value
    const float DURATION_SCALER = 0.5f;

    [System.Serializable]
    class UpgradeData
    {
        public float attackSpeed = 0.6f;
        public int upgradeCost = 15;
        public int attackDamage = 1;
    }

    [SerializeField] List<UpgradeData> upgradeDataList = new();
    [SerializeField] EnemyDetector detector;

    [SerializeField] int attackRadius = 10;

    [HideInInspector] public bool empowered = false;

    Dictionary<int, UpgradeData> upgradeDataDict = new();


    UpgradeData currentData;

    
    

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
        detector.transform.localScale = new Vector3(attackRadius, 0.1f, attackRadius);
        detector.mesh.enabled = true;

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

    public override void TowerLogic()
    {
        foreach (var enemy in detector.detectedEnemies.Keys.ToList())
        {
            Debug.Log("looking at enemy " + enemy.name);
            detector.detectedEnemies[enemy] += Time.deltaTime;
            if (detector.detectedEnemies[enemy] >= currentData.attackSpeed)
            {
                enemy.Damage(currentData.attackDamage);
                enemy.ApplyCC(
                    BaseEnemy.CrowdControl.Slow,
                    empowerValue * SLOW_SCALER,
                    empowerValue * DURATION_SCALER,
                    false,
                    "EmpoweredLightingTowerSlow"
                    );
                detector.detectedEnemies[enemy] = 0;
                Debug.Log("Hitting enemy " + enemy.name);

            }
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
        return "Damage Per Second : " + (currentData.attackDamage/currentData.attackSpeed).ToString("#.00");
    }

    public override void OnRegistryUpdated(List<BaseTower> newRegistry)
    {
        base.OnRegistryUpdated(newRegistry);
        int newPower = 0;
        foreach (var conductor in conductorsPoweringTower)
        {
            newPower += conductor.empowerValue;
        }
        empowerValue = newPower;
    }

}
