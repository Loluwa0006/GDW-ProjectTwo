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
    }

    [System.Serializable]
    public enum TowerReference
    {
        CoinCreator
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
        BaseTower newTower = Instantiate(towerDictionary[TowerReference.CoinCreator].tower);
        newTower.gameManager = gameManager;
        newTower.BeginPlacement();
    }
}
