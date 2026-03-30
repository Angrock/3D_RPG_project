using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGProject
{
    public class GameMenu : GameEntrypoint
    {
        private static GameMenu _instance;
        public static GameMenu Instance => _instance;

        private void Awake()
        {
            _instance = this;
            gameObject.SetActive(false);
            SetActiveCursor(false);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            SetActiveCursor(isActive);
        }

        public void SaveGame()
        {
            var saveService = EntrypointBootstrapper.Instance?.Installer?.Resolve<ISaveService>();
            saveService?.SaveGame();
        }

        public void LoadGame()
        {
            var saveService = EntrypointBootstrapper.Instance?.Installer?.Resolve<ISaveService>();
            saveService?.LoadGame();
        }

        public void ReturnToMainMenu()
        {
            var gameStateManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameStateManager>();
            gameStateManager?.SetState(GameStateManager.GameState.Menu);
            SceneManager.LoadScene("UI_Dev");
        }

        public void ResumeGame()
        {
            var gameStateManager = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameStateManager>();
            gameStateManager?.SetState(GameStateManager.GameState.Playing);
        }

        public void SetActiveCursor(bool isEnabled)
        {
            Cursor.lockState = isEnabled ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isEnabled;
        }

        protected override void OnShutdown() => _instance = null;
    }
}
