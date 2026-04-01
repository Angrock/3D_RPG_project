using UnityEngine.SceneManagement;

namespace RPGProject {
    public class MainMenu : GameEntrypoint {
        public void GameStart() {
            Settings.isLoadGame = false;
            SceneManager.LoadScene("Main");
        }
        
        public void LoadGame() {
            Settings.isLoadGame = true;
            SceneManager.LoadScene("Main");
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
