using UnityEngine;
using UnityEngine.UI;
using _S.Attributes;
using _S.ScriptableVariables;
using _S.Utility.Broadcasting;

namespace _S.UI
{
    public class PowerBar : MonoBehaviour
    {
        [Tooltip("The health reference for this health bar"),
        SerializeField]
        ClampedFloatVariable _health;
        [SerializeField] ScriptableEventChannel _healthChanged;
        [Tooltip("Whether health should be checked for changes every frame. It is perferable to update the health bar through a unity event"),
        SerializeField]
        bool _updateEveryFrame;
        [Tooltip("How fast the bar visual refills"),
        SerializeField]
        float _barFillSpeedMultiplier;

        [SerializeField, Section("UI", new string[] { "_barBack", "_barCurrent", "_damageTickContainer", "_damagePrefeb" })] bool _UIExtended;
        [Tooltip("The back Image of the health bar"),
        SerializeField, HideInInspector]
        Image _barBack;
        [Tooltip("The progress Image of the health bar"),
        SerializeField, HideInInspector]
        Image _barCurrent;
        [Tooltip("The transform that the damage ticks are contained in"),
        SerializeField, HideInInspector]
        Transform _damageTickContainer;
        [Tooltip("The damage tick prefab Image that is intatiated at the end of the progress bar when it takes damage"),
        SerializeField, HideInInspector]
        GameObject _damagePrefeb;

        float _previousHealth;
        float maxWidth;
        float currentWidth => _barCurrent.rectTransform.rect.width;
        float finalWidth;

        void OnEnable()
        {
            _healthChanged.RaiseEvents += UpdateBar;
        }

        void OnDisable()
        {
            _healthChanged.RaiseEvents -= UpdateBar;
        }

        void Start()
        {
            maxWidth = _barBack.rectTransform.rect.width;
            finalWidth = maxWidth * _health.decimalFill;
        }

        void Update()
        {
            if (_updateEveryFrame) { UpdateBar(); }
            // Animate the bar
            if (currentWidth != finalWidth) { _barCurrent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(currentWidth, finalWidth, Time.deltaTime * _barFillSpeedMultiplier)); }
        }

        public void UpdateBar()
        {
            if (_health.current == _previousHealth) { return; }
            finalWidth = maxWidth * _health.decimalFill;
            if (_health.current < _previousHealth)
            {
                RectTransform damageIndicator = Instantiate(_damagePrefeb, _damageTickContainer.transform).GetComponent<RectTransform>();
                damageIndicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth - finalWidth);
                damageIndicator.anchoredPosition = new Vector2(maxWidth * _health.decimalFill, 0);
                if (currentWidth > finalWidth)
                {
                    _barCurrent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, finalWidth);
                }
            }
            else
            {
                for (int i = 0; i < _damageTickContainer.childCount; i++)
                {
                    Destroy(_damageTickContainer.GetChild(i).gameObject);
                }
            }
            _previousHealth = _health.current;
        }
    }
}
