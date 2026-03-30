using UnityEngine;
using UnityEngine.UI;

namespace RPGProject
{
    public class HUD : GameEntrypoint
    {
        [Header("UI Elements")]
        [SerializeField] private Slider _sliderHP;
        [SerializeField] private Slider _sliderMP;
        [SerializeField] private Slider _sliderMageCooldown;

        private static HUD _instance;
        public static HUD Instance => _instance;

        private void Awake()
        {
            _instance = this;
            Debug.Log($"[HUD] Awake вызван, Instance = {_instance != null}");
            Debug.Log($"[HUD] sliderHP = {_sliderHP != null}, sliderMP = {_sliderMP != null}");
        }

        protected override void OnStart()
        {
            if (_sliderHP != null)
            {
                _sliderHP.maxValue = Player.MaxHP;
                _sliderHP.value = Player.MaxHP;
                Debug.Log($"[HUD] HP слайдер настроен: max={_sliderHP.maxValue}, value={_sliderHP.value}");
            }
            if (_sliderMP != null)
            {
                _sliderMP.maxValue = Player.MaxMP;
                _sliderMP.value = Player.MaxMP;
                Debug.Log($"[HUD] MP слайдер настроен: max={_sliderMP.maxValue}, value={_sliderMP.value}");
            }
            if (_sliderMageCooldown != null)
            {
                _sliderMageCooldown.maxValue = 1.0f;
            }
        }

        private void Update()
        {
            if (!_isStarted) return;
            if (_sliderMageCooldown != null)
                _sliderMageCooldown.value += Time.deltaTime;
        }

        public void SetHP(float value)
        {
            if (_sliderHP == null)
            {
                Debug.LogWarning("[HUD] SetHP: sliderHP = null!");
                return;
            }
            
            Debug.Log($"[HUD] SetHP: {value} / {_sliderHP.maxValue}");
            _sliderHP.value = value;
        }

        public void SetMP(float value)
        {
            if (_sliderMP == null)
            {
                Debug.LogWarning("[HUD] SetMP: sliderMP = null!");
                return;
            }
            
            _sliderMP.value = value;
        }

        public void SetMageCooldown(float value)
        {
            if (_sliderMageCooldown != null) _sliderMageCooldown.value = value;
        }

        public void GameWin() => Debug.Log("Test text game win");
        public void GameOver() => Debug.Log("Test text game over");

        protected override void OnShutdown() => _instance = null;
    }
}
