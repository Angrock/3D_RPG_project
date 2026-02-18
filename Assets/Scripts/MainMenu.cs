using UnityEngine;

public class MainMenu : MonoBehaviour {
    public static MainMenu instance;

    void Awake() {
        instance = this;
    }

    public void GameExit() {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void GameWin() {
        Debug.Log("Test text game win");
    }

    public void GameOver() {
        Debug.Log("Test text game over");
    }
}
