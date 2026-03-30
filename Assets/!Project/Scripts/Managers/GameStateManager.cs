using UnityEngine;

namespace RPGProject
{
    /// <summary>
    /// Менеджер состояния игры.
    /// </summary>
    public class GameStateManager : GameEntrypoint
    {
        public enum GameState
        {
            Menu,
            Playing,
            Paused,
            GameOver,
            Victory
        }

        private GameState _currentState;
        public GameState CurrentState => _currentState;

        public event System.Action<GameState> OnStateChanged;

        protected override void OnInitialize()
        {
            _currentState = GameState.Menu;
        }

        protected override void OnStart()
        {
            // При старте сразу устанавливаем состояние игры
            SetState(GameState.Playing);
        }

        public void SetState(GameState newState)
        {
            if (_currentState == newState) return;

            var oldState = _currentState;
            _currentState = newState;

            Debug.Log($"[GameStateManager] Состояние: {oldState} -> {newState}");
            OnStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.Menu:
                    Time.timeScale = 1f;
                    break;

                case GameState.Playing:
                    Time.timeScale = 1f;
                    var gameMenu = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameMenu>();
                    gameMenu?.SetActive(false);
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    var gameMenuPaused = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameMenu>();
                    gameMenuPaused?.SetActive(true);
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;

                case GameState.Victory:
                    Time.timeScale = 0f;
                    var hud = EntrypointBootstrapper.Instance?.Installer?.Resolve<HUD>();
                    hud?.GameWin();
                    break;
            }
        }

        public void TogglePause()
        {
            if (_currentState == GameState.Playing)
                SetState(GameState.Paused);
            else if (_currentState == GameState.Paused)
                SetState(GameState.Playing);
        }

        public void StartGame() => SetState(GameState.Playing);
        public void Victory() => SetState(GameState.Victory);
        public void GameOver() => SetState(GameState.GameOver);
    }
}
