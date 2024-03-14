using UnityEngine;
using UnityEngine.InputSystem;

namespace _S.Entity.Player
{
    //Handles the player input regarding the weapons
    [DisallowMultipleComponent]
    public class PlayerWeaponBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PlayerGunSelector gunSelector;
        [SerializeField]
        private GameObject tazer;
        [SerializeField]
        private InputActionReference fire;
        [SerializeField]
        private InputActionReference reload;
        [SerializeField]
        private InputActionReference melee;
        [SerializeField]
        private bool autoReload = false;

        [SerializeField]
        Animator _weaponAnimator;

        bool _inMelee;
        public bool inMelee
        {
            get => _inMelee;
            set => _inMelee = value;
        }
        bool _isReloading;
        public bool isReloading
        {
            get => _isReloading;
            set => _isReloading = value;
        }

        private void Start()
        {
            tazer.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            melee.action.performed += MeleePerformed;
        }

        void OnDisable()
        {
            melee.action.performed -= MeleePerformed;
        }

        public void MeleePerformed(InputAction.CallbackContext context)
        {
            //Debug.Log("OnMelee Called");
            if (!_inMelee)
            {
                //Debug.Log("canMelee == true");
                _inMelee = true;
                _isReloading = false;
                _weaponAnimator.SetTrigger("Melee");
                //StartCoroutine(Melee());
            }
        }

        private void Update()
        {
            //Shoot
            if (gunSelector.activeGun != null && !_isReloading && !_inMelee)
            {
                gunSelector.activeGun.Tick(fire.action.IsPressed());
            }

            if ((AutoReload() || OnReload()) && !_isReloading && !_inMelee)
            {
                if (_weaponAnimator != null)
                {
                    _weaponAnimator.SetTrigger("Reloading");
                    isReloading = true;
                    _inMelee = false;
                }
                else
                {
                    ReloadGun();
                }
            }
        }

        public void EndReload()
        {
            isReloading = false;
            //gunSelector.activeGun.EndReload();
        }

        //Manual reload
        public bool OnReload()
        {
            return !isReloading && reload.action.WasPressedThisFrame() && gunSelector.activeGun.CanReload();
        }

        //Automatic reload
        private bool AutoReload()
        {
            return !isReloading && autoReload && gunSelector.activeGun.ammoConfig.magCurrent == 0 && gunSelector.activeGun.CanReload();
        }

        public void ReloadGun()
        {
            gunSelector.activeGun.ammoConfig.Reload();
        }

        //IEnumerator Melee()
        //{
        //    Debug.Log("Melee Coroutine Started");
        //    tazer.gameObject.SetActive(true);
        //    Debug.Log("Waiting For 1 Second");
        //    yield return new WaitForSeconds(1);
        //    Debug.Log("Waited For 1 Second");
        //    tazer.gameObject.SetActive(false);
        //    canMelee = true;
        //    Debug.Log("End Of Coroutine");
        //}
    }
}