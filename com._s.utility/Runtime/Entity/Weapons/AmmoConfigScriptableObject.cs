using _S.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Determines when a reload is needed, if at all 
[CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Config", order = 3)]
public class AmmoConfigScriptableObject : ScriptableObject{
    public bool noReloadMode = false;
    public bool reloadReducesHealth = false;
    public int magSize = 10;
    public int magCurrent = 10;

    public HealthBehaviour healthBehaviour;

    public AmmoConfigScriptableObject Clone()
    {
        var newAmmoConfig = Instantiate(this);
        newAmmoConfig.noReloadMode = noReloadMode;
        newAmmoConfig.reloadReducesHealth = reloadReducesHealth;
        newAmmoConfig.magSize = magSize;
        newAmmoConfig.magCurrent = magCurrent;
        newAmmoConfig.healthBehaviour = healthBehaviour;

        return newAmmoConfig;
    }

    public void Reload(){
        //Debug.Log("Reload called");
        if (healthBehaviour.currentHealth > 1) {
            int magMissing = magSize - magCurrent;

            if (healthBehaviour.currentHealth < magMissing) {
                magMissing = Mathf.RoundToInt(healthBehaviour.currentHealth) - 1;
            }

            if(reloadReducesHealth){
                healthBehaviour.ChangeHealthBy(-magMissing);
            }

            magCurrent += magMissing;
        }
        else{
            //Play cant reload sound here
        }
    }

    public bool CanReload(){
        //Check if health above 1
        return magCurrent < magSize;
    }
}
