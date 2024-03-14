using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;
using _S.Audio;

namespace _S.Entity.Player
{
    [RequireComponent(typeof(PlayerGunSelector)), DefaultExecutionOrder(1)]
    public class RifleAnimation : MonoBehaviour
    {
        [SerializeField] GameObject _rifleShotPrefab;
        [SerializeField] Animator _firstPersonAnimator;
        [SerializeField] AudioManager _audioManager;


        Animator _rifleVFXAnimator;
        GunScriptableObject _playerRifle;
        ObjectPool<GameObject> _rifleShotVFXPool;
        // Start is called before the first frame update

        void Start()
        {
            _rifleShotVFXPool = new ObjectPool<GameObject>(CreateNewShot, TakeShotFromPool, ReturnShotToPool, OnDestroyShot, true, 8, 13);
        }

        void OnEnable()
        {
            if (_playerRifle == null)
            {
                _playerRifle = GetComponent<PlayerGunSelector>().guns[0];
                _rifleVFXAnimator = _playerRifle.model.GetComponentInChildren<Animator>();
            }
            _playerRifle.GunShot += GunShot;
        }

        void OnDisable()
        {
            _playerRifle.GunShot -= GunShot;
        }
        // Update is called once per frame
        void GunShot(Vector3 to, Vector3 from, RaycastHit hit)
        {
            if (_rifleVFXAnimator != null)
            {
                _rifleVFXAnimator.SetTrigger("Play");
            }

            if (_firstPersonAnimator != null)
            {
                _firstPersonAnimator.SetTrigger("ShootRifle");
            }

            if (_rifleShotPrefab != null)
            {
                var Shot = _rifleShotVFXPool.Get();
                Shot.transform.position = from;
                Shot.transform.LookAt(to);
                VisualEffect VFX = Shot.GetComponent<VisualEffect>();
                VFX.SetVector3("hitFlashScale", new Vector3(VFX.GetVector3("hitFlashScale").x, VFX.GetVector3("hitFlashScale").y, Vector3.Distance(from, to)));
                Shot.GetComponent<ReturnToPool>().Pool = _rifleShotVFXPool;
            }

            if (_audioManager != null)
            {
                _audioManager.PlayIsolated("Shoot");
            }
        }

        GameObject CreateNewShot()
        {
            var shot = Instantiate(_rifleShotPrefab, null);
            return shot;
        }

        void TakeShotFromPool(GameObject shot)
        {
            shot.SetActive(true);
        }

        void ReturnShotToPool(GameObject shot)
        {
            shot.SetActive(false);
        }

        void OnDestroyShot(GameObject shot)
        {
            Destroy(shot);
        }
    }
}

