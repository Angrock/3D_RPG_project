using UnityEngine;

namespace RPGProject {
    public class CameraController : GameEntrypoint {
        [Header("Настройки камеры")]
        [SerializeField] float minVerticalRotation = -5f;
        [SerializeField] float maxVerticalRotation = 30f;

        float verticalRotation;
        public Player player;

        protected override void OnInitialize() {
            verticalRotation = transform.localEulerAngles.x;
        }

        protected override void OnStart() {
            
            // player = EntrypointBootstrapper.Instance?.Installer?.Resolve<Player>();
                if (player == null) {
                    Debug.LogError("Player reference not found in CameraController.");
                }
        }

        void Update() {

            // if (!isStarted) return;
            // if (player != null && !player.IsAlive) return;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (Mathf.Approximately(mouseX, 0f) && Mathf.Approximately(mouseY, 0f)) {
                mouseX = Input.GetAxisRaw("Mouse X");
                mouseY = Input.GetAxisRaw("Mouse Y");
            }

            verticalRotation -= mouseY * Settings.MouseSensitivityY * Time.deltaTime;
            verticalRotation = Mathf.Clamp(verticalRotation, minVerticalRotation, maxVerticalRotation);

            player.transform.Rotate(mouseX * Settings.MouseSensitivityX * Time.deltaTime * Vector3.up);
            transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}
