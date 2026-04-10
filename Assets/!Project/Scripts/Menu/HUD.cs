using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject {
    public class HUD : GameEntrypoint {
        [Header("UI Elements")]
        [SerializeField] Slider sliderHP;
        [SerializeField] Slider sliderMP;
        [SerializeField] Slider sliderMageCooldown;
        [SerializeField] private TextMeshProUGUI scoresText;
        [SerializeField] GameObject gameOverPannel;

        GameMenu gameMenu;

        protected override void OnStart() {
            sliderHP.maxValue = Player.MaxHP;
            sliderHP.value = Player.MaxHP;
            
            sliderMP.maxValue = Player.MaxMP;
            sliderMP.value = Player.MaxMP;
            
            sliderMageCooldown.maxValue = 1f;

            if (!Settings.IsPeacefulGame)
                SetScoresText(0);
            else
                scoresText.color = new Color(0, 0, 0, 0);

            gameMenu = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameMenu>();
        }

        void Update() {
            if (!isStarted) return;
            sliderMageCooldown.value += Time.deltaTime;
        }

        public void SetHP(float value) {
            sliderHP.value = value;
        }

        public void SetMP(float value) {
            sliderMP.value = value;
        }

        public void SetMageCooldown(float value) {
            if (sliderMageCooldown != null) sliderMageCooldown.value = value;
        }

        public void SetScoresText(int newScore)
        {
            scoresText.text = $"Очки: {newScore}";
        }

        public void GameWin() => Debug.Log("Test text game win");
        public void GameOver() {
            Debug.Log("Test text game over");
            gameMenu.SetActive(false);
            gameMenu.SetActiveCursor(true);
            gameOverPannel.SetActive(true);
        }
    }
}
