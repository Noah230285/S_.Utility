using _S.Entity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Allows the player and AI to have a weapon
[DisallowMultipleComponent]
public class PlayerGunSelector : MonoBehaviour{
    [SerializeField]
    private GunType gun;
    [SerializeField]
    private Transform gunParent;
    [SerializeField]
    private Transform raycastOrigin;
    [SerializeField]
    private List<GunScriptableObject> _guns;
    public List<GunScriptableObject> guns
    {
        get => _guns;
        set => _guns = value; 
    }
    [SerializeField]
    private HealthBehaviour healthBehaviour;

    [SerializeField] bool _cloneGunObjects;

    [Space]
    [Header("Runtime Filled")]
    public GunScriptableObject activeGun;

    //Instantiate desired weapon and model
    void Awake()
    {
        if (_cloneGunObjects)
        {
            for (int i = 0; i < guns.Count; i++)
            {
                guns[i] = guns[i].Clone();
            }
        }

        GunScriptableObject newGun = guns.Find(newGun => newGun.type == gun);

        if (newGun == null)
        {
            Debug.LogError($"No GunScriptableObject found for GunType: {newGun}");
            return;
        }

        activeGun = newGun;
        newGun.Spawn(gunParent, raycastOrigin, this, healthBehaviour);
    }

    //Calls the tick funciton on the current selected weapon (Tick runs some logic before calling the shoot function)
    public void ShootActiveGun(){
        if (activeGun != null){
            activeGun.Tick(true);
        }
    }
}
