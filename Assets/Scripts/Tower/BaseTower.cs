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
   

    [SerializeField] float placementDuration = 2.0f;
    [SerializeField] float placementLockout = 0.4f; //must wait this long before you're able to press

    [SerializeField] Material placingMaterialUnallowed;
    [SerializeField] Material placingMaterialAllowed;
    [SerializeField] Material builtMaterial;
    [SerializeField] Material constructingMaterial;

    public int unallowedRange = 5;

    TowerState towerState = TowerState.Placing;
    float placementTracker = 0.0f;
    float lockoutTracker = 0.0f;

    LayerMask groundMask;
    MeshRenderer mesh;




    public int currentTier = 1;
    public int maxTier = 3;

    int cost = 0;
    private void Awake()
    {
        groundMask = LayerMask.GetMask("Ground");
        mesh = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
         InitTower();
    }
    public void BeginPlacement(int cost)
    {
        lockoutTracker = placementLockout;
        towerState = TowerState.Placing;
        this.cost = cost;
    }

    private void FixedUpdate()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        

            switch (towerState)
        {


            case TowerState.Placing:

                if (Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f, groundMask ))
                {
                    transform.position = Vector3Int.RoundToInt(hitInfo.point);
                    Debug.Log("Mouse at pos " +  transform.position);
                }
                bool areaAllowed = gameManager.AreaClear(transform.position, unallowedRange);

                mesh.material = areaAllowed ? placingMaterialAllowed : placingMaterialUnallowed;

                if (Input.GetMouseButtonDown(0) && lockoutTracker <= 0.0f && areaAllowed)
                {
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
                if (placementTracker <= 0.0f)
                {
                    placementTracker = 0.0f;
                    towerState = TowerState.Built;
                    mesh.material = builtMaterial;
                    gameManager.AddTowerToRegistry(this, cost);
                }
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
