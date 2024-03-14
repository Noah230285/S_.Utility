using _S.ScriptableVariables;
using UnityEngine;

//DEPRECIATED OLD VERSION OF THE PLAYERS GUN
public class LazerRifleScript : MonoBehaviour{
    public int damage = 16; //Damage dealt per shot

    [SerializeField] ClampedIntegerReference _ammo;
    public int ammo
    {
        get { return _ammo.current; }
        set { _ammo.current = value; }
    }
    public int maxAmmo
    {
        get { return _ammo.current; }
        set { _ammo.current = value; }
    }
    public Transform originTransform; //Position, rotation and scale of the raycast origin (This will be the player camera when on the player)
    public Vector3 muzzlePos; //Position of the muzzle of the gun, the origin of the lazer effect

    private RaycastHit hit; //The output of the raycast used to make the gun function

    //Called by outside scripts to begin 
    public int Fire(){
        if (ammo == 0){
            //Maybe cause reload here?
            return 1;
        }

        ammo --;

        if (Physics.Raycast(originTransform.position, originTransform.forward, out hit, Mathf.Infinity)){
            //Check if the target hit can take damage
            //if()
            //{
            //Do damage here
            //}

            //TO DO: Calculate the direction from the muzzlePos to the raycasthit.point, then instanciate the lazer effect at the muzzlePos going in the calculated direction
            return 2;
        }

        return 0;
    }

    public void Reload(int ammoResource)
    {
        //Play animation

        //If player health below 10, set ammo to be highest possible without setting health below 1
        if (ammoResource < 10){
            ammo = ammoResource - 1;
            ammoResource -= ammo;
        }
        //Else set ammo to max, remove max from health
        else{
            ammo = maxAmmo;
            ammoResource -= maxAmmo;
        }
    }

    private void Update(){
        
    }
}
