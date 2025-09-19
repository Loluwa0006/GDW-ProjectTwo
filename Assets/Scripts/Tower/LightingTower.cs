using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] int attackRadius = 10;

    Dictionary<int, UpgradeData> upgradeDataDict = new();

    Dictionary<BaseEnemy, float> enemiesInRadius = new();

    UpgradeData currentData;

    LayerMask enemyMask;
    public override void InitTower()
    {
        base.InitTower();
        gameManager = FindFirstObjectByType<GameManager>();
        int index = 1;
        foreach (var upgradeData in upgradeDataList)
        {
            upgradeDataDict[index] = upgradeData;
            index++;
        }
        currentData = upgradeDataDict[1];
        enemyMask = LayerMask.GetMask("Enemy");

    }

    public override void TowerLogic()
    {

        List<Collider> enemyScan = Physics.OverlapBox(transform.position, new Vector3(attackRadius / 2, 1.0f, attackRadius / 2), Quaternion.identity, enemyMask).ToList();
        Dictionary<BaseEnemy, float> tempDict = new(enemiesInRadius);
        foreach (var enemy in enemiesInRadius.Keys)
        {
            if (!enemyScan.Contains(enemy.enemyCollider))
            {
                tempDict.Remove(enemy);
            }
        }
        enemiesInRadius = tempDict;

        foreach (var enemy in enemyScan)
        {
            if (!enemy.TryGetComponent(out BaseEnemy scannedEnemy)) { continue; }
            if (!enemiesInRadius.ContainsKey(scannedEnemy))
            {
                    enemiesInRadius.Add(scannedEnemy, currentData.attackSpeed); //immediately shoot whatever enters the tower's range
            }
        }

        foreach (var enemy in enemiesInRadius.Keys.ToList())
        {
            enemiesInRadius[enemy] += Time.deltaTime;
            if (enemiesInRadius[enemy] >= currentData.attackSpeed)
            {
                enemy.Damage(currentData.attackDamage);
                enemiesInRadius[enemy] = 0;
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
