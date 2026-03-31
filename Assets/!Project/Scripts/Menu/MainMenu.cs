using UnityEngine.SceneManagement;

namespace RPGProject {
    public class MainMenu : GameEntrypoint {
        public void GameStart() => SceneManager.LoadScene("Main");

        public void GameExit() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }
    }
}
