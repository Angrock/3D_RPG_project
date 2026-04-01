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
            SaveService saveService = EntrypointBootstrapper.Instance?.Installer?.Resolve<SaveService>();
            saveService.SaveGame();
        }

        public void LoadGame() {
            Debug.Log("Load Game");
            SaveService saveService = EntrypointBootstrapper.Instance?.Installer?.Resolve<SaveService>();
            saveService.LoadGame();
        }

        public void ReturnToMainMenu() {
            SceneManager.LoadScene("UI_dev");
        }

        public void RestartGame() {
            Settings.isLoadGame = false;
            SceneManager.LoadScene("Main");
        }
    }
}
