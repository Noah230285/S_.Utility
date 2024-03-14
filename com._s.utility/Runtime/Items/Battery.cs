using _S.Hitboxes;
using _S.ScriptableVariables;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MoveTowardsPhysics), typeof(Collider))]
public class Battery : MonoBehaviour
{
    [SerializeField] FloatReference _healAmount;
    [SerializeField] float _waitToAllowCollectTime;
    bool _attracted;
    [SerializeField] Collider _attractCollider; 
    public void OnEnable()
    {
        if (_waitToAllowCollectTime > 0)
        {
            _attractCollider.enabled = false;
            StartCoroutine(WaitBeforeAllowingCollect());
        }
    }
    public void CollidedPlayer(Collider collider)
    {
        if (collider.transform.CompareTag("Player"))
        {
            if (collider.transform.gameObject.TryGetComponent(out DamageReciever dr))
            {
                dr.healthReference.health += _healAmount.value;
            }
            else
            {
                Debug.LogWarning($"Could not find DamageReciever component on collided player {collider.transform.parent.name}");
            }
            Destroy(gameObject);
        }
        else
        {
            if (_attracted) { return; }
            GetComponent<MoveTowardsPhysics>().SetTarget(collider.transform);
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            _attracted = true;
        }
    }

    IEnumerator WaitBeforeAllowingCollect()
    {
        yield return new WaitForSeconds(_waitToAllowCollectTime);
        _attractCollider.enabled = true;
    }
}
