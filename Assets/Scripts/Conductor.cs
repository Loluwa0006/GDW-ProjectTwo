using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class Conductor : BaseTower
{

    const int CONDUCTOR_SEARCH_RANGE = 3;

    [HideInInspector] public int powerConducted = 0;

    [HideInInspector] BaseTower powerSource;

    public override void OnTowerBuilt()
    {
        base.OnTowerBuilt();
        
    }
    public override void OnTowerClicked()
    {
    }


    public void UpdateConductorPower()
    {
        List<BaseTower> towers = gameManager.GetNearbyTowers(this, CONDUCTOR_SEARCH_RANGE);

        //first check if we have power
            foreach (BaseTower t in towers)
            {
                Debug.Log(t.name + " is nearby");
            if (t is CoinMiner)
            {
                Debug.Log("Tower is a harvester, conducting power");
                powerConducted = t.currentTier;
                powerSource = t;
                    break;
            }
            else if (t is Conductor nearbyConductor)
            {
                if (nearbyConductor.powerConducted > 0)
                {
                    Debug.Log("Nearby conductor conducting power, so now we are too");
                    powerConducted = nearbyConductor.powerConducted;
                    powerSource = nearbyConductor.powerSource;
                        break;
                }
            }
            }
            //then we check if we can give power 
            foreach (BaseTower t in towers)
        {
            if (t != powerSource)
            {
                Debug.Log("adding conductor power to tower " + t.name);
                t.conductorsPoweringTower.Add(this);
            }
        }
         
        }

    public override void OnTowerSold(List<BaseTower> remainingTowers)
    {
        foreach (BaseTower tower in remainingTowers)
        {
            if (tower.conductorsPoweringTower.Contains(this))
            {
                tower.conductorsPoweringTower.Remove(this);
                tower.OnRegistryUpdated(remainingTowers);
            }
        }
    }
}
