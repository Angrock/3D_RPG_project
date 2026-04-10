using UnityEngine.SceneManagement;

namespace RPGProject
{
    public class MainMenuController
    {
        private MainMenuModel model;
        private MainMenuView view;

        public MainMenuController(MainMenuModel model, MainMenuView view)
        {
            this.model = model;
            this.view = view;

            view.Initialize(this);
        }

        public void OnNewGame()
        {
            //view.PlayButtonClickSound();
            model.NewGame();
            SceneManager.LoadScene(Constants.MainGameSceneName);
        }

        public void OnLoadGame()
        {
            //view.PlayButtonClickSound();
            model.LoadGame();
            SceneManager.LoadScene(Constants.MainGameSceneName);
        }

        public void OnGameQuit()
        {
            //view.PlayButtonClickSound();

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                UnityEngine.Application.Quit();
            #endif
        }

        public void OnOpenSettings()
        {
            view.PlayButtonClickSound();
            view.SetActiveSettingsPanel(true);
        }

        public void OnCloseSettings()
        {
            view.PlayButtonClickSound();
            view.SetActiveSettingsPanel(false);
        }
    }
}