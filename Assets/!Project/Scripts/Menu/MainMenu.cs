using UnityEngine.SceneManagement;

namespace RPGProject {
    public class MainMenu : GameEntrypoint {
        public void GameStart() {
            Settings.isLoadGame = false;
            SceneManager.LoadScene(Constants.MainGameSceneName);
        }
        
        public void LoadGame() {
            Settings.isLoadGame = true;
            SceneManager.LoadScene(Constants.MainGameSceneName);
        }

        public void GameExit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }
    }
}

// this class is no longer in use