using UnityEngine;
using System.Collections.Generic;

public class TowerPlacer : MonoBehaviour
{

    [SerializeField] GameManager gameManager;
    [System.Serializable]
    public class TowerInfo
    {
        public BaseTower tower;
        public TowerReference name;
        public int cost;
    }

    [System.Serializable]
    public enum TowerReference
    {
        CoinCreator,
        LightingTurret
    }

    [SerializeField]  List<TowerInfo> availableTowers = new();

    Dictionary<TowerReference, TowerInfo> towerDictionary = new();

    private void Awake()
    {
        foreach (var tower in availableTowers)
        {
            towerDictionary[tower.name] = tower;
        }
    }

    public void CreateNewCoinCreator()
    {
        if (towerDictionary[TowerReference.CoinCreator].cost > gameManager.GetCoins()) return ;
        BaseTower newTower = Instantiate(towerDictionary[TowerReference.CoinCreator].tower);
        InitNewTower(newTower, towerDictionary[TowerReference.CoinCreator].cost);
    }

    public void CreateNewLightingTurret()
    {
        if (towerDictionary[TowerReference.LightingTurret].cost > gameManager.GetCoins()) return;
        BaseTower newTower = Instantiate(towerDictionary[TowerReference.LightingTurret].tower);
        InitNewTower(newTower, towerDictionary[TowerReference.LightingTurret].cost);
    }

    public void InitNewTower(BaseTower newTower, int cost)
    {
        newTower.gameManager = gameManager;
        newTower.BeginPlacement(cost);
    }

}
