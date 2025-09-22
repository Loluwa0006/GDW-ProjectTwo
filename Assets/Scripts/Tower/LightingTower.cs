using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LightingTower : BaseTower 
{

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
        detector.detectorArea.enabled = false;

    }

    public override void InitTower()
    {
        base.InitTower();
        gameManager = FindFirstObjectByType<GameManager>();
        currentData = upgradeDataDict[1];


        detector.detectorArea.radius = attackRadius;

    }

    public override void OnTowerBuilt()
    {
        base.OnTowerBuilt();
        detector.detectorArea.enabled = true;
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


}
