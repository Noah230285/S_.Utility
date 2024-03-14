using _S.Attributes;
using _S.ScriptableVariables;
using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using static MovementPath.MovePointWrapper;

[DefaultExecutionOrder(-2)]
public class MovementPath : MonoBehaviour
{
    [SerializeField] MovePointWrapper[] _movePoints;
    [SerializeField] bool _useGlobalSpeed;
    [SerializeField] bool _ignoreInitialPosition;
    [SerializeField] float _speed;

    [SerializeField, Section("Debug", new string[] { "_currentPoint", "_previousPosition", "_currentTime", "_goalTime", "_startPosition" } )] bool _debugExtended;
    [SerializeField, ReadOnly, HideInInspector] int _currentPoint;
    [SerializeField, ReadOnly, HideInInspector] Vector3 _previousPosition;
    [SerializeField, ReadOnly, HideInInspector] float _currentTime;
    [SerializeField, ReadOnly, HideInInspector] float _goalTime;
    [SerializeField, ReadOnly, HideInInspector] Vector3 _startPosition;

    int _previousLength;
    Vector3 goToStartPosition => _ignoreInitialPosition ? _movePoints[0].movePoint.GetPositionBetweenPrevious(Vector3.zero, 1) : _startPosition;

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.localPosition;
        if (_ignoreInitialPosition)
        {
            _currentPoint++;
        }
        StartMove(goToStartPosition);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_movePoints.Length != _previousLength) { StartMove(goToStartPosition); }
        _previousLength = _movePoints.Length;
        float finalSpeed = _useGlobalSpeed ? _speed : _movePoints[_currentPoint].localSpeed;
        _currentTime += finalSpeed / 60;

        if (_currentTime >= 1)
        {
            Vector3 endPosition = _movePoints[_currentPoint].movePoint.GetPositionBetweenPrevious(_previousPosition, 1);
            _currentPoint = _currentPoint + 1 >= _movePoints.Length ? 0 : _currentPoint + 1;
            StartMove(endPosition);
            return;
        }
        transform.localPosition = _movePoints[_currentPoint].movePoint.GetPositionBetweenPrevious(_previousPosition, _currentTime);
    }

    void StartMove(Vector3 goToPosition)
    {
        transform.localPosition = goToPosition;
        _previousPosition = transform.localPosition;
        _currentTime = 0;
    }

    [Serializable]
    public class MovePointWrapper
    {
        [SerializeField] float _localSpeed;
        public float localSpeed => _localSpeed;

        [SerializeField, EnumBinding(new string[] { "_transform", "_position", "_curve" } )] PointTypes _pointType;
        public PointTypes pointType => _pointType;

        public enum PointTypes
        {
            transform,
            position,
            curve,
        }

        public MovePoint movePoint
        {
            get
            {
                switch (pointType)
                {
                    case PointTypes.transform:
                        return _transform;
                    case PointTypes.position:
                        return _position;
                    case PointTypes.curve:
                        return _curve;
                    default:
                        return null;
                }
            }
        }
        [Serializable]
        public abstract class MovePoint
        {
            [SerializeField] protected bool _addFromPrevious;
            //public abstract float GetStartTime();
            //public abstract float GetEndTime();
            public abstract Vector3 GetEndPosition();

            public abstract Vector3 GetPositionBetweenPrevious(Vector3 previousPoint, float time);

            protected Vector3 AdjustedPosition(Vector3 previousPosition)
            {
                return _addFromPrevious ? previousPosition + GetEndPosition() : GetEndPosition();
            }
        }

        [SerializeField, HideInInspector] TransformPoint _transform = new();
        [Serializable]

        public class TransformPoint : MovePoint
        {
            [SerializeField] Transform _point;
            [SerializeField] AnimationCurve _positionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0, 1));

            //public override float GetStartTime()
            //{
            //    return PositionCurve.keys[0].time;
            //}

            //public override float GetEndTime()
            //{
            //    return PositionCurve.keys[PositionCurve.keys.Length - 1].time;
            //}

            public override Vector3 GetEndPosition()
            {
                return _point.localPosition;
            }

            public override Vector3 GetPositionBetweenPrevious(Vector3 previousPoint, float time)
            {
                if (_point == null)
                {
                    return Vector3.zero;
                }
                return Vector3.Lerp(previousPoint, AdjustedPosition(previousPoint), _positionCurve.Evaluate(time));
            }
        }

        [SerializeField, HideInInspector] PositionPoint _position = new();
        [Serializable]
        public class PositionPoint : MovePoint
        {
            [SerializeField] Vector3 _position;
            [SerializeField] AnimationCurve _speedCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

            //public override float GetStartTime()
            //{
            //    return PositionCurve.keys[0].time;
            //}

            //public override float GetEndTime()
            //{
            //    return PositionCurve.keys[PositionCurve.keys.Length - 1].time;
            //}

            public override Vector3 GetEndPosition()
            {
                return _position;
            }

            public override Vector3 GetPositionBetweenPrevious(Vector3 previousPoint, float time)
            {
                return Vector3.Lerp(previousPoint, AdjustedPosition(previousPoint), _speedCurve.Evaluate(time));
            }
        }

        [SerializeField, HideInInspector] CurvePoint _curve = new();
        [System.Serializable]
        public class CurvePoint : MovePoint
        {
            [SerializeField] Vector3 _position;
            [SerializeField] bool _positionAsWeight;
            [SerializeField] AnimationCurve _xCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0, 1));
            [SerializeField] AnimationCurve _yCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0, 1));
            [SerializeField] AnimationCurve _zCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0, 1));

            //public override float GetStartTime()
            //{
            //    float startTime = XCurve.keys[0].time;
            //    if (startTime > YCurve.keys[0].time) { startTime = YCurve.keys[0].time; }
            //    if (startTime > ZCurve.keys[0].time) { startTime = ZCurve.keys[0].time; }
            //    return startTime;
            //}

            //public override float GetEndTime()
            //{
            //    Keyframe xKey = XCurve.keys[XCurve.keys.Length - 1];
            //    Keyframe yKey = YCurve.keys[YCurve.keys.Length - 1];
            //    Keyframe zKey = ZCurve.keys[ZCurve.keys.Length - 1];

            //    float startTime = xKey.time;
            //    if (startTime < yKey.time) { startTime = yKey.time; }
            //    if (startTime < zKey.time) { startTime = zKey.time; }
            //    return startTime;
            //}

            public override Vector3 GetEndPosition()
            {
                return _position;
                //Keyframe xKey = XCurve.keys[XCurve.keys.Length - 1];
                //Keyframe yKey = YCurve.keys[YCurve.keys.Length - 1];
                //Keyframe zKey = ZCurve.keys[ZCurve.keys.Length - 1];

                //return new Vector3(xKey.value * WeightMultiplier.x, yKey.value * WeightMultiplier.y, zKey.value * WeightMultiplier.z);
            }

            public override Vector3 GetPositionBetweenPrevious(Vector3 previousPoint, float time)
            {
                Vector3 adjustedPosition;
                if (_positionAsWeight)
                {
                    adjustedPosition = _addFromPrevious ? previousPoint : Vector3.zero;
                    return new Vector3(
                        adjustedPosition.x + _position.x * _xCurve.Evaluate(time),
                        adjustedPosition.y + _position.y * _yCurve.Evaluate(time),
                        adjustedPosition.z + _position.z * _zCurve.Evaluate(time));
                }
                else
                {
                    adjustedPosition = AdjustedPosition(previousPoint);
                    return new Vector3(
                        Mathf.Lerp(previousPoint.x, adjustedPosition.x, _xCurve.Evaluate(time)),
                        Mathf.Lerp(previousPoint.y, adjustedPosition.y, _yCurve.Evaluate(time)),
                        Mathf.Lerp(previousPoint.z, adjustedPosition.z, _zCurve.Evaluate(time)));
                }
            }
        }
    }
}
