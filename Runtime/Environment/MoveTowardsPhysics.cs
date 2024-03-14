using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveTowardsPhysics : MonoBehaviour
{
    [SerializeField] Transform _target;

    [SerializeField] Vector2 _acceleration;
    [SerializeField] float _maxHorizontalVelocity;
    [SerializeField] float _maxVerticalVelocity;
    [SerializeField] float _verticalSmooth;
    [SerializeField] float _horizontalExponentialFactor;

    Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_target != null)
        {
            Vector3 direction = _target.position - transform.position;
            Vector3 horizontalDirection = Vector3.Normalize(new Vector3(direction.x, 0, direction.z));

            float angle = Vector2.SignedAngle(new Vector2(direction.x, direction.z).normalized, new Vector2(_rb.velocity.x, _rb.velocity.z).normalized);
            float horizontalSpeedMultiplier = 1 - Mathf.Pow(Mathf.Abs(angle) / 180, Mathf.Exp(_horizontalExponentialFactor));
            float ySmoothMultiplier = Mathf.Exp(-Mathf.Pow((_target.position.y - transform.position.y) / _verticalSmooth, 2));

            Vector3 addedVelocity = new Vector3(horizontalDirection.x * _acceleration.x, _acceleration.y * Mathf.Sign(direction.y) * (-ySmoothMultiplier + 1), horizontalDirection.z * _acceleration.x);
            _rb.velocity += addedVelocity * Time.deltaTime;

            Vector2 finalHorizontalSpeed = Vector2.ClampMagnitude(new Vector2(_rb.velocity.x, _rb.velocity.z), _maxHorizontalVelocity * horizontalSpeedMultiplier);
            _rb.velocity = new Vector3(finalHorizontalSpeed.x, Mathf.Clamp(_rb.velocity.y, -_maxVerticalVelocity, _maxVerticalVelocity) * (-ySmoothMultiplier + 1), finalHorizontalSpeed.y);
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
