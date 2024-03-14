using _S.Hitboxes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles dealing damage for the tazer
public class TazerCollision : MonoBehaviour{
    public int tazerDamage = 20;

    private void OnTriggerEnter(Collider other){
        //If collider can be damaged, deal damage
        if(other.TryGetComponent(out IDamageable damageInterface)){
            damageInterface.TakeDamage(tazerDamage);
        }
        else if (other.TryGetComponent(out DamageReciever damageReciever))
        {
            damageReciever.healthReference.TakeDamage(tazerDamage);
        }
    }
}
