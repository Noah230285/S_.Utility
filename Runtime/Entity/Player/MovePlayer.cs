using UnityEngine;

namespace _S.Entity.Player
{
    [DefaultExecutionOrder(-1), RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class MovePlayer : MonoBehaviour
    {
        Vector3 _lastPosition;
        Vector3 _bufferPosition;

        public Vector3 lastMove => transform.position - _lastPosition;

        private void FixedUpdate()
        {
            _lastPosition = _bufferPosition;
            _bufferPosition = transform.position;
        }

        //public Vector3 ReturnPush(Vector3 faceNormal)
        //{
        //    Debug.Log($"<color=red>{_lastPosition.x}</color>");
        //    Debug.Log($"<color=red>{_bufferPosition.x}</color>");
        //    Debug.Log($"<color=red>{transform.position.x}</color>");
        //    Debug.Log($"<color=red>{Vector3.Project(transform.position - _lastPosition, faceNormal).x}</color>");
        //    return Vector3.Project(transform.position - _lastPosition, faceNormal);
        //}

        void OnCollisionEnter(Collision collision)
        {
            Collision(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            Collision(collision);
        }

        void Collision(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") && collision.transform.TryGetComponent(out KineticPlayerController playerController))
            {

                var contact = collision.GetContact(0);
                Vector3 normal = -contact.normal;

                float x = normal.x > 0.01f || normal.x < -0.01f ? 1 * Mathf.Sign(normal.x) : 0;
                float y = normal.y > 0.01f || normal.y < -0.01f ? 1 * Mathf.Sign(normal.y) : 0;
                float z = normal.z > 0.01f || normal.z < -0.01f ? 1 * Mathf.Sign(normal.z) : 0;

                normal = new Vector3(x, y, z);

                playerController.transform.position += Vector3.Project(lastMove + normal * playerController.skinWidth, normal).normalized * Vector3.Dot(lastMove + normal * playerController.skinWidth, normal);
            }
        }
    }

}
