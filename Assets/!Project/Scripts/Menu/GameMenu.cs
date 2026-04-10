using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGProject {
    public class GameMenu : GameEntrypoint {
        [SerializeField] private GameObject[] notPeacefulButtons;

        protected override void OnInitialize() {
            SetActive(false);
        }

        protected override void OnStart()
        {
            if (Settings.IsPeacefulGame)
            {
                for (int i = 0; i < notPeacefulButtons.Length; i++)
                    notPeacefulButtons[i].SetActive(false);
            }
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
            SceneManager.LoadScene(Constants.MainMenuSceneName);
        }

        public void RestartGame() {
            Settings.isLoadGame = false;
            SceneManager.LoadScene(Constants.MainGameSceneName);
        }
    }
}
