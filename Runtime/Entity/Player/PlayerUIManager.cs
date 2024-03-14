using _S.Utility.Broadcasting;
using UnityEngine.Rendering.Universal;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _S.Entity.Player
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] Camera _playerCam;
        [SerializeField] string _HUDScene;
        [SerializeField] string _pauseScene;
        [SerializeField] InputActionReference _pauseAction;
        [SerializeField] ScriptableEventChannel _pauseChannel;
        [SerializeField] ScriptableEventChannel _unpauseChannel;
        [SerializeField] AudioMixer _audioMixer;
        [SerializeField] string _gameVolumeName;

        bool _paused;
        Scene _hud;
        Scene _pause;
        Camera _pauseCamera;

        void OnEnable()
        {
            _pauseAction.action.performed += PausePressed;
            _pauseChannel.RaiseEvents += PauseGame;
            _unpauseChannel.RaiseEvents += ResumeGame;
        }

        void OnDisable()
        {
            _pauseAction.action.performed -= PausePressed;
            _pauseChannel.RaiseEvents -= PauseGame;
            _unpauseChannel.RaiseEvents -= ResumeGame;
        }

        void Start()
        {
            _hud = SceneLoader.LoadOrFindScene(_HUDScene);
            _pause = SceneLoader.LoadOrFindScene(_pauseScene);
            StartCoroutine(Wait());
        }
        IEnumerator Wait()
        {
            yield return null;
            _playerCam.GetUniversalAdditionalCameraData().cameraStack.Add(_hud.GetRootGameObjects()[0].GetComponent<Camera>());
            _pauseCamera = _pause.GetRootGameObjects()[0].GetComponent<Camera>();
            _pauseCamera.gameObject.SetActive(false);
        }

        void PausePressed(InputAction.CallbackContext x)
        {
            if (!_paused)
            {
                _pauseChannel.OnRaiseEvents();
            }
            else
            {
                _unpauseChannel.OnRaiseEvents();
            }
        }

        void PauseGame()
        {
            _paused = true;
            _pauseCamera.gameObject.SetActive(true);
            _playerCam.GetUniversalAdditionalCameraData().cameraStack.Add(_pauseCamera);
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            GameManager.paused = true;
            _audioMixer.SetFloat(_gameVolumeName, -80);
        }
        void ResumeGame()
        {
            _paused = false;
            _pauseCamera.gameObject.SetActive(false);
            _playerCam.GetUniversalAdditionalCameraData().cameraStack.Remove(_pauseCamera);
            Cursor.lockState = CursorLockMode.Locked;
            _audioMixer.SetFloat(_gameVolumeName, 0);
            Time.timeScale = 1;
            GameManager.paused = false;
        }
    }
}