using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{

    public  GameObject townHall;
    public UnityEvent<WaveData> newWave = new();
    [SerializeField] TMP_Text coinTracker;
    [SerializeField] TMP_Text waveTracker;
    [SerializeField] TMP_Text waveDurationTracker;
    [SerializeField] UpgradePanel upgradePanel;
    [SerializeField] LayerMask unallowedMask;

    [SerializeField] int coins = 20;




    List<BaseTower> towerList = new();
    BaseTower selectedTower = null;


    [System.Serializable]
    public class WaveData
    {
        public int waveNumber;
        public float waveDuration;
    }

    [SerializeField] List<WaveData> waveData = new();

    WaveData currentWave = null;


    private void Start()
    {
        currentWave = waveData[0];
        Debug.Log("Current Wave is " + currentWave.waveNumber + " will last for " + currentWave.waveDuration + " seconds ");
        InitUI();
        StartCoroutine(WaveLogic());
    }

    public void InitUI()
    {
        coinTracker.text = "Coins: " + coins;
        upgradePanel.gameObject.SetActive(false);

        waveTracker.text = "Prepare";
        waveDurationTracker.text = "?";
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
        if (towerList.Contains(tower)) return; 
        towerList.Add(tower);
        coins -= cost;
        coinTracker.text = "Coins: " + coins;
    }

    public void RemoveTowerFromRegistry(BaseTower tower)
    {
        if (towerList.Contains(tower))
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
        int upgradeCost = selectedTower.GetNextUpgradeCost();
        coins -= upgradeCost;
        selectedTower.UpgradeTower();
        coinTracker.text = "Coins: " + coins;
        UpdateUpgradePanelDisplay(selectedTower);
       selectedTower.towerValue = Mathf.FloorToInt((float)upgradeCost / 2);

    }

    public void SellTower()
    {
        coins += selectedTower.towerValue;
        coinTracker.text = "Coins: " + coins;
        upgradePanel.gameObject.SetActive(false);
        RemoveTowerFromRegistry(selectedTower);

        Destroy(selectedTower.gameObject);
        selectedTower = null;
    }

 

    public void UpdateUpgradePanelDisplay(BaseTower tower)
    {
        if (tower == null) { return; }
        upgradePanel.TierDisplay.text = "Tier " + tower.currentTier;
        upgradePanel.ToNextTierDisplay.text = tower.currentTier == tower.maxTier ? "MAX" : tower.GetNextUpgradeCost().ToString();
        upgradePanel.upgradeButton.interactable = tower.TowerUpgradable();
        upgradePanel.descriptionDisplay.text = tower.GetDescriptionText();
    }

    IEnumerator WaveLogic()
    {
        float timer = currentWave.waveDuration;

        while (timer > 0.0f)
        {
            TimeSpan time = TimeSpan.FromSeconds(timer);

            waveDurationTracker.text = string.Format("{0:D2}:{1:D2}:{2:D3}",
            time.Minutes, time.Seconds, time.Milliseconds);
            yield return null;

            timer -= Time.deltaTime;
        }
        waveDurationTracker.text = "00:00:000";
        if (waveData.Count > currentWave.waveNumber)
        {
            currentWave = waveData[currentWave.waveNumber + 1];
            newWave.Invoke(currentWave);
            StartCoroutine(WaveLogic());
            
            waveTracker.text = "Wave " + currentWave.waveNumber;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0.0f)
            {
                Time.timeScale = 1.0f;
            }
            else
            {
                Time.timeScale = 0.0f;
            }
        }
    }
}
