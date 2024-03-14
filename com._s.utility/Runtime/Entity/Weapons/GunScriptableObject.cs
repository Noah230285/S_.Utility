using _S.Entity;
using _S.Hitboxes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

//The heart of all projectile weapons
[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject{
    //public ImpactType impactType;
    public GunType type;
    public string gunName;
    //Reference to the weapon model prefab from the asset list
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;
    
    public enum DamageType
    {
        hitscan,
        projectile
    }
    [SerializeField] DamageType _damageType;
    [SerializeField] GameObject _projectile;
    ObjectPool<GameObject> _projectiles;


    //Reference to the config for things like the fire rate and spread of the weapon 
    public ShootConfigurationScriptableObject shootConfig;
    //Reference to the config for how the weapons tracer should look and act
    public TrailConfigScriptableObject trailConfig;
    //Reference to the config for all things damage related
    public DamageConfigScriptableObject damageConfig;
    //Reference to the config for all things ammo related
    public AmmoConfigScriptableObject ammoConfig;


    Transform _raycastOriginTransform;
    public Transform raycastOriginTransform
    {
        get => _raycastOriginTransform;
        set => _raycastOriginTransform = value;
    }

    Transform _parentTransform;
    public Transform parentTransform
    {
        get => _parentTransform;
        set => _parentTransform = value;
    }

    MonoBehaviour activeMonoBehaviour;
    //Reference to this instance of the weapon model prefab
    GameObject _model;
    public GameObject model
    {
        get => _model;
        set => _model = value;
    }

    bool _reloadThroughAnimation;
    public bool reloadThroughAnimation
    {
        get => _reloadThroughAnimation;
        set => _reloadThroughAnimation = value;
    }
    float lastShootTime;
    float initialClickTime;
    float stopClickTime;
    bool clickedLastFrame;
    //Reference to the empty game object positioned at the muzzle of the weapon with a particle system component
    ParticleSystem shootSystem;
    //Reference to the pool of available trail renderers for tracing the bullets
    ObjectPool<TrailRenderer> trailPool;

    public Action<Vector3, Vector3, RaycastHit> GunShot;

    public GunScriptableObject Clone()
    {
        GunScriptableObject newGun = Instantiate(this);

        newGun.name = name;
        newGun.type = type;
        newGun.gunName = gunName;
        newGun.modelPrefab = modelPrefab;
        newGun.spawnPoint = spawnPoint;
        newGun.spawnRotation = spawnRotation;
        newGun.modelPrefab = modelPrefab;

        newGun.shootConfig = shootConfig;
        newGun.trailConfig = trailConfig;
        newGun.damageConfig = damageConfig;
        newGun.ammoConfig = ammoConfig.Clone();
        return newGun;
    }

    //Instantiates the corresponding weapon model to the selected weapon in the desired position and rotation, then grabs the muzzle location of the weapon
    public void Spawn(Transform parent, Transform raycastOrigin, MonoBehaviour newActiveMonoBehaviour, HealthBehaviour healthBehaviour){
        //Debug.Log("Spawn Start");

        if (_damageType == DamageType.projectile)
        {
            _projectiles = new ObjectPool<GameObject>(
                () =>
                {
                    var o = Instantiate(_projectile);
                    o.SetActive(true);
                    return o;
                },
                x =>
                {
                    x.SetActive(true);
                }, 
                x =>
                {
                    x.SetActive(false);
                }, 
                x =>
                {
                    Destroy(x);
                } , true, 10, 15);
        }
        activeMonoBehaviour = newActiveMonoBehaviour;
        lastShootTime = 0; //Last shoot time is not properly reset in editor, in build it is
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        //Debug.Log("Instantiate Weapon Model");

        model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        parentTransform = parent;
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        //Debug.Log("Get Particle System In Model");

        shootSystem = model.GetComponentInChildren<ParticleSystem>();

        ammoConfig.healthBehaviour = healthBehaviour;
        ammoConfig.magCurrent = ammoConfig.magSize;

        raycastOriginTransform = raycastOrigin;
        //Debug.Log("Spawn End");
    }

    //Is called upon by other scripts to actually fire the weapon (In this case it fires off a raycast)
    public void Shoot(){
        //Debug.Log("Gun Shoot Start");

        if(Time.time - lastShootTime - shootConfig.fireRate > Time.deltaTime) {
            //Debug.Log("Time - lastShootTime - fireRate > deltaTime");
            float lastDuration = Mathf.Clamp(
                (stopClickTime - initialClickTime), 
                0, 
                shootConfig.maxSpreadTime
            );
            float lerpTime = (shootConfig.centreSpeed - (Time.time - stopClickTime)) / shootConfig.centreSpeed;


            initialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
        }

        //If fire delay has passed
        if (Time.time > shootConfig.fireRate + lastShootTime){
            //Debug.Log("Time > lastShootTime + fireRate");

            lastShootTime = Time.time;
            //shootSystem.Play();

            Vector3 spreadAmount = shootConfig.GetSpread(Time.time - initialClickTime);
            //model.transform.forward += model.transform.parent.forward + spreadAmount;

            //Vector3 shootDirection = model.transform.forward;
            Vector3 shootDirection = raycastOriginTransform.transform.forward + spreadAmount;

            if (!ammoConfig.noReloadMode){
                ammoConfig.magCurrent--;
            }

            switch (_damageType)
            {
                case DamageType.hitscan:
                    if (Physics.Raycast(raycastOriginTransform.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, shootConfig.hitMask))
                    {
                        //Debug.Log("Raycast Hit");

                        activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, hit.point, hit));
                    }
                    else
                    {
                        //Debug.Log("Raycast Miss");

                        activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, shootSystem.transform.position + (shootDirection * trailConfig.missDistance), new RaycastHit()));
                    }
                    break;
                case DamageType.projectile:
                    var projectile = _projectiles.Get();
                    projectile.transform.position = raycastOriginTransform.transform.position;
                    projectile.transform.rotation = raycastOriginTransform.transform.rotation;
                    projectile.GetComponent<ReturnToPool>().Pool = _projectiles;
                    break;
                default:
                    break;
            }

        }

        //Debug.Log("Gun Shoot End");
    }

    public bool CanReload(){
        return ammoConfig.CanReload();
    }

    public void EndReload(){
        ammoConfig.Reload();
    }

    //Called every update inside the weapon behaviour of whatever is using it
    public void Tick(bool clickedThisFrame){
        //Handles weapon recoil

        //model.transform.localRotation = Quaternion.Lerp(
        //    model.transform.localRotation,
        //    Quaternion.Euler(spawnRotation),
        //    Time.deltaTime * shootConfig.centreSpeed
        //);

        //Debug.Log($"Is full auto: {shootConfig.fullAuto}");
        //Debug.Log($"Has not clicked last frame: {!clickedLastFrame}");
        //Debug.Log($"Has clicked this frame: {clickedThisFrame}");
        
        //If the weapon is set to fully automatic and the trigger is pressed
        //or if the weapon is set to semi automatic and the trigger was pressed this frame instead of being held since last frame
        if((shootConfig.fullAuto || !clickedLastFrame) && clickedThisFrame){
            //Debug.Log($"Should fire");
            clickedLastFrame = true;

            //Check if current ammo in gun is 0, or if the gun is set have infinite ammo
            if (ammoConfig.magCurrent > 0 || ammoConfig.noReloadMode){
                //Debug.Log($"magCurrent: {ammoConfig.magCurrent}");
                Shoot();
            }
        }
        else if(!clickedThisFrame && clickedLastFrame){
            //Debug.Log($"lastShootTime: {lastShootTime}");
            //lastShootTime = Time.time;
            clickedLastFrame = false;
        }
    }

    //Grabs a TrailRenderer out of the pool of TrailRenderers and sends it in the direction of the raycast hit, or off into the distance if the raycast missed
    private IEnumerator PlayTrail(Vector3 startPos, Vector3 endPos, RaycastHit hit)
    {
        //Debug.Log("PlayTrail Start");
        GunShot?.Invoke(startPos, endPos, hit);
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPos;
        yield return null;
        //Debug.Log("Waited A Frame 0");

        instance.emitting = true;

        float distance = Vector3.Distance(startPos, endPos);
        float remainingDistance = distance;
        while (remainingDistance > 0){
            //Debug.Log("PlayTrail While Loop");
            instance.transform.position = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(1 - (remainingDistance/distance)));
            remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;
            yield return null;
        }

        instance.transform.position = endPos;

        if (hit.collider != null){
            //SurfaceManager.Instance.HandleImpact(hit.transform.gameObject, endPos, hit.normal, ImpactType, 0);

            if (hit.collider.TryGetComponent(out IDamageable damageInterface)) {
                damageInterface.TakeDamage(damageConfig.GetDamage(distance));
            }
            else if (hit.collider.TryGetComponent(out DamageReciever damageReciever))
            {
                damageReciever.healthReference.TakeDamage(damageConfig.GetDamage(distance));
            }
        }

        yield return new WaitForSeconds(trailConfig.duration);

        //Debug.Log($"Waited For {trailConfig.duration} Seconds");

        yield return null;

        //Debug.Log("Waited A Frame 2");

        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);

        //Debug.Log("PlayTrail End");
    }

    //Creates a TrailRenderer and adds it into the pool if non are available and one is needed
    private TrailRenderer CreateTrail(){
        //Debug.Log("CreateTrail Start");

        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.color;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        //Debug.Log("CreateTrail End");

        return trail;
    }
}