using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public static MainMenu instance;

    void Awake() {
        instance = this;
    }

    public void GameStart() {
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
