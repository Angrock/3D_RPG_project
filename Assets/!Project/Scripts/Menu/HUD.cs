using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public Slider sliderHP;

    public static HUD instance;

    void Awake() {
        instance = this;
    }

    public void GameWin() {
        Debug.Log("Test text game win");
    }

    public void GameOver() {
        Debug.Log("Test text game over");
    }
}
