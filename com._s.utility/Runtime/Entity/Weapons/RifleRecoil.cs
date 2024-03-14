using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleRecoil : MonoBehaviour{
    private Vector3 camCurrentRotation;
    private Vector3 camTargetRotation;

    [SerializeField] private GameObject playerBody;

    private Vector3 bodyCurrentRotation;
    private Vector3 bodyTargetRotation;

    //Recoil Rotations
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    //Recoil Settings
    [SerializeField] private float snappiness;
    [SerializeField] private float centreSpeed;

    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        camTargetRotation = Vector3.Lerp(camTargetRotation, Vector3.zero, centreSpeed * Time.deltaTime);

        camCurrentRotation = Vector3.Slerp(camCurrentRotation, camTargetRotation, snappiness * Time.fixedDeltaTime);

        bodyTargetRotation = Vector3.Lerp(bodyTargetRotation, bodyCurrentRotation, centreSpeed * Time.deltaTime);

        bodyCurrentRotation = Vector3.Slerp(bodyCurrentRotation, bodyTargetRotation, snappiness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(camCurrentRotation);
        playerBody.transform.localRotation = Quaternion.Euler(bodyCurrentRotation);
    }

    //Called just after Fire() on the laser rifle script as to not make player miss
    public void RecoilFire(){
        camTargetRotation += new Vector3(recoilX, camCurrentRotation.y, Random.Range(-recoilZ, recoilZ));
        bodyTargetRotation += new Vector3(bodyCurrentRotation.x, Random.Range(-recoilY, recoilY), bodyCurrentRotation.z);
    }
}
