
using UnityEngine;
using System.Collections.Generic;

public class BaseTower : MonoBehaviour
{
    enum TowerState
    {
        Placing,
        Constructing,
        Built
    }
    [HideInInspector] public int towerValue = 0;
    [HideInInspector] public int empowerValue = 0;
    [HideInInspector] public HashSet<Conductor> conductorsPoweringTower = new();

    public GameManager gameManager;

    public int unallowedRange = 5;

    public int currentTier = 1;
    public int maxTier = 3;

    [SerializeField] float placementDuration = 2.0f;
    [SerializeField] float placementLockout = 0.4f; //must wait this long before you're able to press

    [SerializeField] BuildingMaterials buildingMaterials;
    [SerializeField] MeshRenderer mesh;




    TowerState towerState = TowerState.Placing;
    float placementTracker = 0.0f;
    float lockoutTracker = 0.0f;

    LayerMask groundMask;

    int cost = 0;


    private void Start()
    {
        InitTower();
        groundMask = LayerMask.GetMask("Ground");
    }
    public void BeginPlacement(int cost)
    {
        lockoutTracker = placementLockout;
        towerState = TowerState.Placing;
        this.cost = cost;
        towerValue =Mathf.FloorToInt((float) cost / 2);
    }

    private void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
            switch (towerState)
        {
            case TowerState.Placing:

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f, groundMask ))
                {                    
                    transform.position = Vector3Int.RoundToInt(hitInfo.point);
                }
                bool areaAllowed = gameManager.AreaClear(transform.position, unallowedRange);

                mesh.material = areaAllowed ?buildingMaterials. placingMaterialAllowed : buildingMaterials.placingMaterialUnallowed;

                if (Input.GetMouseButtonDown(0) && lockoutTracker <= 0.0f && areaAllowed)
                {
                    gameManager.AddTowerToRegistry(this, cost);
                    towerState = TowerState.Constructing;
                    placementTracker = placementDuration;
                    lockoutTracker = 0.0f;
                    mesh.material = buildingMaterials.constructingMaterial;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    Destroy(this.gameObject);
                }
                lockoutTracker -= Time.deltaTime;
                break;

            case TowerState.Constructing:
                placementTracker -= Time.deltaTime;
                if (placementTracker <= 0.0f) OnTowerBuilt();
                break;

            case TowerState.Built:
                TowerLogic();
                break;
        }
    }


    public virtual void OnTowerClicked()
    {
        if (towerState == TowerState.Built)
        {
            gameManager.DisplayUpgradePanel(this);
        }
        OnTowerHighlighted();
    }
    public virtual void OnTowerHighlighted()
    {

    }

    public virtual void OnTowerUnhighlighted()
    {

    }
    public virtual void OnTowerBuilt()
    {
        placementTracker = 0.0f;
        towerState = TowerState.Built;
        mesh.material = buildingMaterials. builtMaterial;
    }

    public virtual void OnTowerUpgraded()
    {

    }

    public virtual void OnTowerSold(List<BaseTower> remainingTowers)
    {

    }
    public virtual void InitTower()
    {

    }
    public virtual void TowerLogic()
    {

    }

    public virtual bool TowerUpgradable()
    {
        return false;
    }

    public virtual void UpgradeTower()
    {
        currentTier += 1;
        currentTier = Mathf.Clamp(currentTier, 1, maxTier);
    }


    public virtual int GetNextUpgradeCost()
    {
        return -1;
    }

    public virtual string GetDescriptionText()
    {
        return string.Empty;
    }
    public virtual void OnRegistryUpdated(List<BaseTower> newRegistry)
    {
        int newPower = 0;
        foreach (var conductor in conductorsPoweringTower)
        {
            newPower += conductor.powerConducted;
            Debug.Log("Adding " + conductor.powerConducted + " to tower " + name + ", new power is now " + newPower);
        }
        empowerValue = newPower;

        Debug.Log("New empower value is " + empowerValue);
    }

}
