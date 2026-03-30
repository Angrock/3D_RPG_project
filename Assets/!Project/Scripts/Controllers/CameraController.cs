using UnityEngine;

namespace RPGProject
{
    /// <summary>
    /// Контроллер камеры. Управляет вращением камеры вокруг игрока.
    /// </summary>
    public class CameraController : GameEntrypoint
    {
        [Header("Настройки камеры")]
        [SerializeField] private float _minVerticalRotation = -5f;
        [SerializeField] private float _maxVerticalRotation = 30f;

        private float _verticalRotation;
        private Player _player;

        protected override void OnInitialize()
        {
            _verticalRotation = transform.localEulerAngles.x;
            Debug.Log("[CameraController] OnInitialize вызван");
        }

        protected override void OnStart()
        {
            _player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            Debug.Log($"[CameraController] OnStart вызван, Player найден: {_player != null}");
        }

        private void Update()
        {
            if (!_isStarted)
            {
                Debug.LogWarning("[CameraController] _isStarted = false");
                return;
            }
            
            if (_player == null)
            {
                Debug.LogWarning("[CameraController] _player = null");
                return;
            }

            _verticalRotation -= Input.GetAxis("Mouse Y") * Settings.MouseSensitivityY * Time.deltaTime;
            _verticalRotation = Mathf.Clamp(_verticalRotation, _minVerticalRotation, _maxVerticalRotation);

            _player.transform.Rotate(Input.GetAxis("Mouse X") * Settings.MouseSensitivityX * Time.deltaTime * Vector3.up);
            transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }
    }
}
