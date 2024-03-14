using _S.ScriptableVariables;
using _S.Utility.Broadcasting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthChangedOverlay : MonoBehaviour
{

    [SerializeField] VolumeProfile _UIVolume;
    [SerializeField] float _damagedWaveSpeed;
    [SerializeField] AnimationCurve _damageIntensityCurve;
    [SerializeField] AnimationCurve _damageIntensityRangeCurve;
    [SerializeField] AnimationCurve _aberrationCurve;
    [SerializeField] AnimationCurve _aberrationRangeCurve;
    [SerializeField] AnimationCurve _aberrationSpeedCurve;
    [SerializeField] ScriptableEventChannel _healthChangedEvent;
    [SerializeField] ClampedFloatVariable _health;
    [SerializeField] float _damagePercentThreshold;

    Animator _healthOverlayAnimator;
    Vignette _vignette;
    ChromaticAberration _aberration;
    float _vignettePrevious;
    float _aberrationPrevious;
    float _currentHealthTransition;
    float _previousHealth;

    void OnEnable()
    {
        _healthChangedEvent.RaiseEvents += HealthChanged;
    }

    void OnDisable()
    {
        _healthChangedEvent.RaiseEvents -= HealthChanged;
    }

    void Start()
    {
        _healthOverlayAnimator = GetComponent<Animator>();
        _UIVolume.TryGet(out _vignette);
        _UIVolume.TryGet(out _aberration);
        _previousHealth = -1;
        if (_vignette != null)
        {
            _vignette.intensity.value = 0;
        }
        if (_aberration != null)
        {
            _aberration.intensity.value = 0;
        }
    }
    void Update()
    {
        if (_UIVolume != null)
        {
            _currentHealthTransition = Mathf.Lerp(_currentHealthTransition, 1 - _health.decimalFill, Time.deltaTime);

            float middle = _damageIntensityCurve.Evaluate(_currentHealthTransition);
            float range = _damageIntensityRangeCurve.Evaluate(_currentHealthTransition) / 2;
            if (range != 0)
            {
                _vignettePrevious += Time.deltaTime * Mathf.PI * _damagedWaveSpeed;
                _vignette.intensity.value = (Mathf.Sin(_vignettePrevious) + middle / range) * range;
            }

            middle = _aberrationCurve.Evaluate(_currentHealthTransition);
            range = _aberrationRangeCurve.Evaluate(_currentHealthTransition) / 2;
            float speed = _aberrationSpeedCurve.Evaluate(_currentHealthTransition);
            if (range != 0)
            {
                _aberrationPrevious += Time.deltaTime * Mathf.PI * speed;
                _aberration.intensity.value = (Mathf.Sin(_aberrationPrevious) + middle / range) * range;
            }
        }
    }

    public void HealthChanged()
    {
        if (_previousHealth < 0)
        {
            _previousHealth = _health.current;
            return;
        }
        if (_healthOverlayAnimator == null) { return; }
        bool takenDamage = _previousHealth > _health.current ? true : false;
        //Texture instantiateTexture = takenDamage ? _damageOverlay : _healOverlay;
        if (takenDamage)
        {
            if (((_previousHealth - _health.current) / _health.max) * 100 > _damagePercentThreshold)
            {
                _healthOverlayAnimator.SetTrigger("damaged");
            }
        }
        else
        {
            _healthOverlayAnimator.SetTrigger("healed");
        }
        _previousHealth = _health.current;
    }

}
