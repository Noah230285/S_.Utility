using UnityEngine;
using UnityEngine.AI;
using _S.ScriptableVariables;

namespace _S.AI
{
    [DefaultExecutionOrder(-1)]
    public class AIAgent : MonoBehaviour
    {
        public Transform _transform;
        public NavMeshAgent _NavMeshAgent;
        public TransformReference _transformReference;
        public TransformReference _transformLookTarget;
        public IDamageable _healthInterface;
        public float _oldHealth;
        public bool _treeSkip = false;
        public bool _fire = true;
        public Transform AimPoint;
        public Transform ChestRotationDummy;
        public Transform ArmsRotationDummy;
        public Transform Hitbox;

        public Animator _animator;
        public PlayerGunSelector _enemyGunSelector;
        public GameObject _gunParent;
        public float _timer = 0f;

        // Start is called before the first frame update
        void Awake()
        {
            _treeSkip = false;
            _healthInterface = GetComponent<IDamageable>();
            _transform = GetComponent<Transform>();
            _NavMeshAgent = GetComponent<NavMeshAgent>();
            _oldHealth = _healthInterface.currentHealth;
        }

        // Update is called once per frame
        void Update()
        {
            if (_enemyGunSelector != null)
            {
                if (_enemyGunSelector.activeGun != null)
                {
                    if (_fire)
                    {
                        _timer += Time.deltaTime;
                        if (_timer > 1)
                        {
                            _enemyGunSelector.activeGun.Tick(_fire);
                            _timer = 0;
                            _fire = false;
                        }
                    }
                }
            }
        }

        public void ToggleTree(bool a)
        {
            _treeSkip = a;
        }
    }

}