using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGProject {
    public class GameMenu : GameEntrypoint {
        protected override void OnInitialize() {
            SetActive(false);
        }

        public void SetActive(bool isActive) {
            SetActiveCursor(isActive);
            gameObject.SetActive(isActive);
        }

        public void SetActiveCursor(bool isEnabled) {
            Cursor.lockState = isEnabled ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isEnabled;
        }

        public void SaveGame() {
            Debug.Log("Save Game");
            ISaveService saveService = EntrypointBootstrapper.Instance?.Installer?.Resolve<ISaveService>();
            saveService?.SaveGame();
        }

        public void LoadGame() {
            Debug.Log("Load Game");
            ISaveService saveService = EntrypointBootstrapper.Instance?.Installer?.Resolve<ISaveService>();
            saveService?.LoadGame();
        }

        public void ReturnToMainMenu() {
            GameStateManager gameStateManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameStateManager>();
            gameStateManager?.SetState(GameStateManager.GameState.Menu);
            SceneManager.LoadScene("UI_dev");
        }
    }
}
