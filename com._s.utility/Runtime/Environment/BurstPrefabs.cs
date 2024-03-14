using _S.Utility;
using UnityEngine;

public class BurstPrefabs : MonoBehaviour
{
    [SerializeField] bool _isPrefab;
    [SerializeField] GameObject _object;
    [SerializeField] float _initialVerticalSpeedMin;
    [SerializeField] float _initialVerticalSpeedMax;
    [SerializeField] float _initialHorizontalSpeedMin;
    [SerializeField] float _initialHorizontalSpeedMax;
    [SerializeField] int _burstCount;
    [SerializeField] int _maxOut = -1;
    [SerializeField] bool rotateToDirection;
    int _currentOut;
    public void BurstObjects()
    {
        for (int i = 0; i < _burstCount; i++)
        {
            if (_currentOut == _maxOut) { return; }
            _currentOut++;
            GameObject obj = _isPrefab ? Instantiate(_object, transform.position, _object.transform.rotation) : _object;
            obj.transform.SetParent(null);
            Rigidbody rb = obj.GetComponent<Rigidbody>();

            float randomAngle = Random.Range(0, 360);
            Vector2 horizontalVelocity = CustomMath.AngleToVector2(randomAngle) * Random.Range(_initialHorizontalSpeedMin, _initialHorizontalSpeedMax);
            float verticalSpeed = Random.Range(_initialVerticalSpeedMin, _initialVerticalSpeedMax);
            Vector3 finalVelocity = new Vector3(horizontalVelocity.x, verticalSpeed, horizontalVelocity.y);

            if (rotateToDirection)
            {
                obj.transform.rotation *= transform.rotation;
                obj.transform.rotation = Quaternion.FromToRotation(obj.transform.forward, finalVelocity.normalized);
            }

            rb.AddForce(transform.TransformDirection(finalVelocity), ForceMode.Impulse);
            if (!_isPrefab) return;
        }
    }
}
