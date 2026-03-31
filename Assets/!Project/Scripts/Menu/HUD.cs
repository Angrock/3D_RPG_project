using UnityEngine;
using UnityEngine.UI;

namespace RPGProject {
    public class HUD : GameEntrypoint {
        [Header("UI Elements")]
        [SerializeField] Slider sliderHP;
        [SerializeField] Slider sliderMP;
        [SerializeField] Slider sliderMageCooldown;

        protected override void OnStart() {
            sliderHP.maxValue = Player.MaxHP;
            sliderHP.value = Player.MaxHP;
            Debug.Log($"[HUD] HP слайдер настроен: max={sliderHP.maxValue}, value={sliderHP.value}");
            
            sliderMP.maxValue = Player.MaxMP;
            sliderMP.value = Player.MaxMP;
            Debug.Log($"[HUD] MP слайдер настроен: max={sliderMP.maxValue}, value={sliderMP.value}");
            
            sliderMageCooldown.maxValue = 1f;
        }

        void Update() {
            if (!isStarted) return;
            sliderMageCooldown.value += Time.deltaTime;
        }

        public void SetHP(float value) {
            Debug.Log($"[HUD] SetHP: {value} / {sliderHP.maxValue}");
            sliderHP.value = value;
        }

        public void SetMP(float value) {
            sliderMP.value = value;
        }

        public void SetMageCooldown(float value) {
            if (sliderMageCooldown != null) sliderMageCooldown.value = value;
        }

        public void GameWin() => Debug.Log("Test text game win");
        public void GameOver() => Debug.Log("Test text game over");
    }
}
