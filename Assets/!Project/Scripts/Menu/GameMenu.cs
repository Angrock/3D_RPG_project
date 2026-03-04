using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {
    public GameObject loadingScreen;
    float timerLoading;
    bool isStartGame;

    public static GameMenu instance;

    void Awake() {
        instance = this;
        timerLoading = 0;
        isStartGame = true;
    }

    void FixedUpdate() {
        timerLoading += Time.fixedDeltaTime;
        if (timerLoading > 0.3f) {
            loadingScreen.SetActive(false);
            if (isStartGame) {
                gameObject.SetActive(false);
                SetActiveCursor(false);
                isStartGame = false;
            }
        }
    }

    public void SaveGame() {
        Debug.Log("TestText: Save Game");
        InternalParams.SetVector3("PlayerPosition", Player.instance.transform.position);
        InternalParams.SetVector3("PlayerRotation", Player.instance.transform.rotation.eulerAngles);
        InternalParams.SetFloat("PlayerHP", Player.instance.currentHP);
        InternalParams.SetFloat("PlayerMP", Player.instance.currentMP);
        for (int i = 0; i < GameManager.instance.enemies.Count; i++) {
            InternalParams.SetVector3($"Enemy{i}Position", GameManager.instance.enemies[i].transform.position);
            InternalParams.SetVector3($"Enemy{i}Rotation", GameManager.instance.enemies[i].transform.rotation.eulerAngles);
            InternalParams.SetFloat($"Enemy{i}HP", GameManager.instance.enemies[i].currentHP);
        }
    }

    public void LoadGame() {
        Debug.Log("TestText: Load Game");
        loadingScreen.SetActive(true);
        timerLoading = 0;
        Player.instance.transform.SetPositionAndRotation(InternalParams.GetVector3("PlayerPosition"),
            Quaternion.Euler(InternalParams.GetVector3("PlayerRotation")));
        Player.instance.SetNewHP(InternalParams.GetFloat("PlayerHP"));
        Player.instance.SetNewMP(InternalParams.GetFloat("PlayerMP"));
        for (int i = 0; i < GameManager.instance.enemies.Count; i++) {
            GameManager.instance.enemies[i].transform.SetPositionAndRotation(
                InternalParams.GetVector3($"Enemy{i}Position"),
                Quaternion.Euler(InternalParams.GetVector3($"Enemy{i}Rotation")));
            GameManager.instance.enemies[i].SetNewHP(InternalParams.GetFloat($"Enemy{i}HP"));
        }
    }

    public void ReturnToMainMenu() {
        SceneManager.LoadScene("UI_Dev");
    }

    public void SetActiveCursor(bool isEnabled) {
        Cursor.lockState = isEnabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isEnabled;
    }
}