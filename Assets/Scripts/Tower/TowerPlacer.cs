using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TowerPlacer : MonoBehaviour
{

    [SerializeField] GameManager gameManager;
    [System.Serializable]
    public class TowerInfo
    {
        public BaseTower tower;
        public TowerReference name;
        public int cost;
        public Button creatorButton;
    }

    [System.Serializable]
    public enum TowerReference
    {
        CoinCreator,
        LightingTurret,
        FlameTower,
        Conductor,
        PoisonTower
    }

    [SerializeField]  List<TowerInfo> availableTowers = new();

    Dictionary<TowerReference, TowerInfo> towerDictionary = new();

    private void Awake()
    {
        foreach (var tower in availableTowers)
        {
            towerDictionary[tower.name] = tower;
            tower.creatorButton.onClick.AddListener( () => CreateNewTower(tower.name));
        }
    }

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
        gameManager.onCoinsUpdated.AddListener(OnCoinsUpdated);
        OnCoinsUpdated(gameManager.GetCoins());
    }
    public void CreateNewTower(TowerReference reference)
    {
        int cost = towerDictionary[reference].cost;
        if (cost > gameManager.GetCoins()) return;
        BaseTower newTower = Instantiate(towerDictionary[reference].tower);
        newTower.gameManager = gameManager;
        newTower.BeginPlacement(cost);
    }


    public void OnCoinsUpdated(int newAmount)
    {
        foreach (var info in availableTowers)
        {
            info.creatorButton.interactable = (newAmount >= info.cost);
        }
    }


}
