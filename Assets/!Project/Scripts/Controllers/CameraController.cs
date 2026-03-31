using UnityEngine;

namespace RPGProject {
    public class CameraController : GameEntrypoint {
        [Header("Настройки камеры")]
        [SerializeField] float minVerticalRotation = -5f;
        [SerializeField] float maxVerticalRotation = 30f;

        float verticalRotation;
        Player player;

        protected override void OnInitialize() {
            verticalRotation = transform.localEulerAngles.x;
            Debug.Log("[CameraController] OnInitialize вызван");
        }

        protected override void OnStart() {
            player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
            Debug.Log($"[CameraController] OnStart вызван, Player найден: {player != null}");
        }

        private void Update() {
            if (!isStarted) {
                Debug.LogWarning("[CameraController] isStarted = false");
                return;
            }

            verticalRotation -= Input.GetAxis("Mouse Y") * Settings.MouseSensitivityY * Time.deltaTime;
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalRotation, maxVerticalRotation);

            player.transform.Rotate(Input.GetAxis("Mouse X") * Settings.MouseSensitivityX * Time.deltaTime * Vector3.up);
            transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}
