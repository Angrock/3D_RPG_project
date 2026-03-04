using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public Slider sliderHP;
    public Slider sliderMP;
    public Slider sliderMageCooldown;

    public static HUD instance;

    void Awake() {
        instance = this;
    }

    void Update() {
        sliderMageCooldown.value += Time.deltaTime;
    }

    public void GameWin() {
        Debug.Log("Test text game win");
    }

    public void GameOver() {
        Debug.Log("Test text game over");
    }
}
