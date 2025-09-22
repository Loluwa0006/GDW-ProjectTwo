using UnityEngine;
using UnityEngine.EventSystems;

public class BaseTower : MonoBehaviour
{
    enum TowerState
    {
        Placing,
        Constructing,
        Built
    }

    public GameManager gameManager;

    public int unallowedRange = 5;

    public int currentTier = 1;
    public int maxTier = 3;



    [SerializeField] float placementDuration = 2.0f;
    [SerializeField] float placementLockout = 0.4f; //must wait this long before you're able to press

    [SerializeField] Material placingMaterialUnallowed;
    [SerializeField] Material placingMaterialAllowed;
    [SerializeField] Material builtMaterial;
    [SerializeField] Material constructingMaterial;
    [SerializeField] MeshRenderer mesh;
    [SerializeField] GameObject groundObject;



    TowerState towerState = TowerState.Placing;
    float placementTracker = 0.0f;
    float lockoutTracker = 0.0f;

    LayerMask groundMask;





   [HideInInspector] public int towerValue = 0;

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

    private void FixedUpdate()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        

            switch (towerState)
        {


            case TowerState.Placing:

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f, groundMask ))
                {
                    Debug.Log("Ray hitting " + hitInfo.collider.gameObject.name +", layer mask is  " + LayerMask.LayerToName(hitInfo.collider.gameObject.layer));
                    
                    transform.position = Vector3Int.RoundToInt(hitInfo.point);
                }
                Debug.Log("Transform position is " + transform.position);
                Debug.Log("Hit info point is " + hitInfo.point);
                bool areaAllowed = gameManager.AreaClear(transform.position, unallowedRange);

                mesh.material = areaAllowed ? placingMaterialAllowed : placingMaterialUnallowed;

                if (Input.GetMouseButtonDown(0) && lockoutTracker <= 0.0f && areaAllowed)
                {
                    gameManager.AddTowerToRegistry(this, cost);
                    towerState = TowerState.Constructing;
                    placementTracker = placementDuration;
                    lockoutTracker = 0.0f;
                    mesh.material = constructingMaterial;
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
                if (Physics.Raycast(ray, out RaycastHit mouseInfo, 1000.0f))
                {
                    Debug.Log("Hit obj " + mouseInfo.collider.gameObject);
                   if (mouseInfo.collider.gameObject == gameObject && Input.GetMouseButtonDown(0))
                    {
                        gameManager.DisplayUpgradePanel(this);
                    }
                }
                break;
        }
    }

    public virtual void OnTowerBuilt()
    {
        placementTracker = 0.0f;
        towerState = TowerState.Built;
        mesh.material = builtMaterial;
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
   
}
