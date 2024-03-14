using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Determines how the visual tracer created by the weapon looks and acts
[CreateAssetMenu(fileName = "TrailConfig", menuName = "Guns/Gun Trail Config", order = 4)]
public class TrailConfigScriptableObject : ScriptableObject{
    //The material that will provide the texture for the trail
    public Material material;
    //The thickness of the trail from head to tail
    public AnimationCurve widthCurve;
    //Length of time before the trail disappears
    public float duration = 0.5f;
    public float minVertexDistance = 0.1f;
    //Sets the alpha levels of the trail from head to tail, changing color settings from white to anything else does not matter as it does not change anything
    public Gradient color;

    //Distance that the trail will travel when tracing a missed shot before stopping
    public float missDistance = 100f;
    //Speed at which the trail will go from the origin point to the end pos
    public float simulationSpeed = 100f;
}
