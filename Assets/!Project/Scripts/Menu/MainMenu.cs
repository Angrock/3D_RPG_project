using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGProject
{
    public class MainMenu : GameEntrypoint
    {
        private static MainMenu _instance;
        public static MainMenu Instance => _instance;

        private void Awake()
        {
            _instance = this;
        }

        public void GameStart() => SceneManager.LoadScene("Main");

        public void GameExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        protected override void OnShutdown() => _instance = null;
    }
}
