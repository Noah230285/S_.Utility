using _S.Utility;
using UnityEngine;

namespace _S.Entity.Player
{
    public class PushPlayer : MonoBehaviour
    {
        [SerializeField] PlayerController controller;
        CapsuleCollider _thisCollider;
        void Start()
        {
            _thisCollider = GetComponent<CapsuleCollider>();
        }
        void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == 7)
            {
                Vector3 additionalOffset = Vector3.zero;
                float radius = 0;
                float height = 0;
                var capsule = other as CapsuleCollider;
                if (capsule)
                {
                    additionalOffset = capsule.center;
                    radius = capsule.radius;
                    height = capsule.height;
                }
                var sphere = other as SphereCollider;
                if (sphere)
                {
                    additionalOffset = sphere.center;
                    radius = sphere.radius;
                    height = sphere.radius;
                }
                var box = other as BoxCollider;
                if (box)
                {
                    additionalOffset = box.center;
                    radius = (box.size.z + box.size.x) / 2;
                    height = box.size.y;
                }
                additionalOffset = new Vector3(additionalOffset.x * transform.lossyScale.x, additionalOffset.y * transform.lossyScale.y, additionalOffset.z * transform.lossyScale.z);
                Vector3 direction = (transform.position - (other.transform.position + additionalOffset));
                float pushStrength = (1 - (direction.XZ().magnitude - _thisCollider.radius) / radius) * controller.config.EnemyPushPower;
                pushStrength *= 1 - (direction.y / height);
                controller.horizontalVelocity += direction.XZ().normalized * pushStrength * Time.deltaTime;
            }
        }
    }
}