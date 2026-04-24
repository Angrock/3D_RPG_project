using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGProject {
    public class GameMenu : GameEntrypoint {
        [SerializeField] private GameObject[] notPeacefulButtons;
        private SaveService saveService;

        protected override void OnInitialize() {
            SetActive(false);
        }

        protected override void OnStart()
        {
            saveService = EntrypointBootstrapper.Instance?.Installer?.Resolve<SaveService>();

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
            saveService.SaveGame();
        }

        public void LoadGame()
        {
            StartCoroutine(LoadGameRoutine());
        }

        public void ReturnToMainMenu() {
            Settings.isLoadGame = false;
            SceneManager.LoadScene(Constants.MainMenuSceneName);
        }

        public void RestartGame() {
            Settings.isLoadGame = false;
            SceneManager.LoadScene(Constants.MainGameSceneName);
        }

        private IEnumerator LoadGameRoutine()
        {
            if (System.IO.File.Exists(Constants.SaveFileName))
            {
                Settings.isLoadGame = true;

                // Асинхронная загрузка сцены, отключаем активацию сцены до полной готовности
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Constants.MainGameSceneName);
                asyncLoad.allowSceneActivation = true;

                // Ждем полной загрузки сцены
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }

                // Задержка для инициализации всех объектов, Start и Awake
                yield return new WaitForEndOfFrame();

                saveService.LoadGame();
            }
            else
            {
                Debug.Log("Is no any save file to load the game");
                yield break;
            }
        }
    }
}
