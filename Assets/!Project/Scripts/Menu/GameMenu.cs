using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {
    public static GameMenu instance;

    void Awake() {
        instance = this;
        gameObject.SetActive(false);
        SetActiveCursor(false);
    }

    public void SaveGame()
    {
        Debug.Log("TestText: Save Game");
    }

    public void LoadGame()
    {
        Debug.Log("TestText: Load Game");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("UI_Dev");
    }

    public void SetActiveCursor(bool isEnabled) {
        Cursor.lockState = isEnabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isEnabled;
    }
}