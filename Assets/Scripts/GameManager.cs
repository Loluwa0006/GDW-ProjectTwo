using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public  GameObject townHall;
    [SerializeField] TMP_Text coinTracker;
    [SerializeField] UpgradePanel upgradePanel;
    [SerializeField] LayerMask unallowedMask;



    List<BaseTower> towerList = new();
    BaseTower selectedTower = null;
    int coins = 0;

    


    private void Start()
    {
        coinTracker.text = "Coins: " + coins;
        upgradePanel.gameObject.SetActive(false);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinTracker.text = "Coins: " + coins;
        UpdateUpgradePanelDisplay(selectedTower);
    }

    public int GetCoins()
    {
        return coins;
    }

    public void AddTowerToRegistry(BaseTower tower, int cost)
    {
        if (!towerList.Contains(tower))
        {
            towerList.Add(tower);
        }
        coins -= cost;
        coinTracker.text = "Coins: " + coins;
    }

    public void RemoveTowerFromRegistry(BaseTower tower)
    {
        if (!towerList.Contains(tower))
        {
            towerList.Remove(tower);
        }
    }

    public bool AreaClear(Vector3 pos, int size)
    {
        foreach (BaseTower tower in towerList)
        {
            if (Vector3.Distance(tower.transform.position, pos) <= size)
            {
                return false;
            }
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return !Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f, unallowedMask);
    }

    public void DisplayUpgradePanel(BaseTower tower)
    {
        selectedTower = tower;
        upgradePanel.gameObject.SetActive(true);
        UpdateUpgradePanelDisplay(tower);
    }

    public void UpgradeTower()
    {
        coins -= selectedTower.GetNextUpgradeCost();
        selectedTower.UpgradeTower();
        coinTracker.text = "Coins: " + coins;
        UpdateUpgradePanelDisplay(selectedTower);
    }

    public void UpdateUpgradePanelDisplay(BaseTower tower)
    {
        if (tower == null) { return; }
        upgradePanel.TierDisplay.text = "Tier " + tower.currentTier;
        upgradePanel.ToNextTierDisplay.text = tower.currentTier == tower.maxTier ? "MAX" : tower.GetNextUpgradeCost().ToString();
        upgradePanel.upgradeButton.interactable = tower.TowerUpgradable();
        upgradePanel.descriptionDisplay.text = tower.GetDescriptionText();
    }
}
