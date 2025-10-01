using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FlameTower : BaseTower
{


    const float STUN_SCALER = 0.2f; // stuns for 0.1 sec for each power value


    [System.Serializable]
    class UpgradeData
    {
        public float attackSpeed = 0.6f;
        public int upgradeCost = 15;
        public int attackDamage = 1;
        public int attackRadius = 2;
    }

    [SerializeField] List<UpgradeData> upgradeDataList = new();
    [SerializeField] EnemyDetector detector;
    [SerializeField] ParticleSystem flameParticles;
    [SerializeField] Light flameLight;
    [SerializeField] AudioSource audioSource;
    [Header("Tower SFX")]
    [SerializeField] AudioClip fireSFX;
    [SerializeField] AudioClip blastSFX;


    Dictionary<int, UpgradeData> upgradeDataDict = new();


    UpgradeData currentData;

    float reloadDuration = 0.0f;
    private void Awake()
    {
        int index = 1;
        foreach (var upgradeData in upgradeDataList)
        {
            upgradeDataDict[index] = upgradeData;
            index++;
        }
    }

 

    public override void OnTowerHighlighted()
    {
        base.OnTowerHighlighted();
        detector.mesh.enabled = true;
    }

    public override void OnTowerUnhighlighted()
    {
        base.OnTowerUnhighlighted();
        detector.mesh.enabled = false;
    }

    public override void OnTowerEmpowered()
    {
        base.OnTowerEmpowered();
        flameParticles.Play();
        flameLight.enabled = true;
        audioSource.PlayOneShot(fireSFX);
    }

    public override void OnTowerDepowered()
    {
        base.OnTowerDepowered();
        flameParticles.Stop();
        flameLight.enabled = false;
    }
    public override void InitTower()
    {
        base.InitTower();
        gameManager = FindFirstObjectByType<GameManager>();

        currentData = upgradeDataDict[1];

        detector.transform.localScale = new Vector3(currentData.attackRadius, 0.1f, currentData.attackRadius);

        detector.mesh.enabled = true;
        detector.detectorArea.enabled = false;
    }

    public override void OnTowerBuilt()
    {
        base.OnTowerBuilt();

        detector.detectorArea.enabled = true;
        detector.mesh.enabled = false;
    }

    public override void TowerLogic()
    {
        reloadDuration -= Time.deltaTime;
        if (reloadDuration <= 0.0f)
        {
            foreach (var enemy in detector.detectedEnemies.Keys.ToList())
            {
                enemy.Damage(currentData.attackDamage);
                if (empowerValue > 0)
                {
                    enemy.ApplyCC(
                        BaseEnemy.CrowdControl.Stun,
                        empowerValue * STUN_SCALER,
                        empowerValue * STUN_SCALER,
                        false,
                        "EmpoweredFlameTowerStun"
                        );
                }
                Debug.Log("Hitting enemy " + enemy.name);
            }

            if (detector.detectedEnemies.Keys.Count > 0)
            {
                reloadDuration = currentData.attackSpeed;
                audioSource.PlayOneShot(blastSFX, 0.2f);
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
        detector.transform.localScale = new Vector3(currentData.attackRadius, 0.1f, currentData.attackRadius);
    }

    public override int GetNextUpgradeCost()
    {
        return upgradeDataDict[currentTier + 1].upgradeCost;
    }

    public override string GetDescriptionText()
    {
        return "Damage Per Second : " + (currentData.attackDamage / currentData.attackSpeed).ToString("#.00");
    }

    public override string GetEmpoweredText()
    {
        return "Empowered: Stunning for " + (empowerValue * STUN_SCALER) + " seconds.";
    }

   


}
