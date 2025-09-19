using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

public class CoinMiner : BaseTower
{
    [System.Serializable]
    class UpgradeData
    {
       public int mineSpeed = 30;
       public int upgradeCost = 50;
    }

    [SerializeField] List<UpgradeData> upgradeDataList = new();

    Dictionary<int, UpgradeData> upgradeDataDict = new();

    UpgradeData currentData;

    int frameTracker = 0;
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

    }
    public override void TowerLogic()
    {
        frameTracker++; 
        if (frameTracker % currentData.mineSpeed == 0)
        {
            gameManager.AddCoins(1);
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
        return "Coin Rate: " + (60.0f /  (float) currentData.mineSpeed).ToString("#.00");
    }
}
