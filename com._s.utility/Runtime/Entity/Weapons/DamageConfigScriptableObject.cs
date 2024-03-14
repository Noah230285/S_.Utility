using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

//Determines how much damage the weapon will do and its damage drop-off curve
[CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
public class DamageConfigScriptableObject : ScriptableObject{
    public MinMaxCurve damageCurve;

    private void Reset(){
        damageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float distance = 0){
        return Mathf.CeilToInt(damageCurve.Evaluate(distance,Random.value));
    }
}
