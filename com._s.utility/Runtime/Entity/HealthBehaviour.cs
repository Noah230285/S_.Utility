using _S.Utility;
using _S.ScriptableVariables;
using UnityEngine;
using UnityEngine.Events;
using _S.Attributes;

namespace _S.Entity
{
    public class HealthBehaviour : MonoBehaviour, IDamageable
    {
        [SerializeField] bool _resetOnStart;
        [SerializeField] ClampedFloatReference _health;
        [SerializeField, HideInInspector] bool _extended;

        [SerializeField, Section("Action Enablers", new string[]{
        "_allowDeath",
        "_allowChangeHealth"})]
        bool _actionEnablersExtended;

        [SerializeField, HideInInspector] bool _allowDeath = true;
        [SerializeField, HideInInspector] bool _allowChangeHealth = true;
        public bool allowChangeHealth
        {
            get => _allowChangeHealth;
            set => _allowChangeHealth = value;
        }
        public float currentHealth { get => health; private set => health = value; }
        public float maxHealth { get => _health.max; private set => _health.max = value; }

        public event IDamageable.TakeDamageEvent OnTakeDamage;
        public event IDamageable.DeathEvent OnDeath;

        public float health
        {
            get { return _health.current; }
            set
            {
                if (!_allowChangeHealth) { return; }
                _health.current = Mathf.Clamp(value, 0, _health.max);
                OnHealthChanged();
                if (_health.current == 0)
                {
                    if (!_allowDeath) { return; }
                    OnThisDeath();
                }
            }
        }

        public UnityEvent<float> HealthChanged;
        public UnityEvent ThisDeath;

        public void Start()
        {
            if (_resetOnStart) { FullHeal(); }
            OnHealthChanged();
        }
        void OnHealthChanged()
        {
            HealthChanged.Invoke(_health.current);
        }

        public void ChangeHealthBy(float change)
        {
            health += change;
        }

        public void ChangeHealthPercent(float change)
        {
            health += change / 100 * _health.max;
        }

        void OnThisDeath()
        {
            ThisDeath.Invoke();
        }

        void FullHeal()
        {
            health = _health.max;
        }

        public void TakeDamage(float damage){
            //Debug.Log($"TakeDamage called with input damage: {damage}");
            float damageTaken = Mathf.Clamp(damage, 0, currentHealth);

            //Debug.Log($"currentHealth before taking damage: {currentHealth}");
            //Debug.Log($"damageTaken: {damageTaken}");
            currentHealth -= damageTaken;
            //Debug.Log($"currentHealth after taking damage: {currentHealth}");

            if (damageTaken != 0) {
                //Debug.Log("Invoking OnTakeDamage");
                OnTakeDamage?.Invoke(damageTaken);
            }

            if(currentHealth == 0 && damageTaken != 0){
                //Debug.Log("Invoking OnDeath");
                OnDeath?.Invoke();
            }
        }
    }
}
