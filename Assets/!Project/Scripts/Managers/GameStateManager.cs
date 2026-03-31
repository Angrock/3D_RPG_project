using UnityEngine;

namespace RPGProject {
    public class GameStateManager : GameEntrypoint {
        public enum GameState {
            Menu,
            Playing,
            Paused,
            GameOver,
            Victory
        }

        GameState currentState;
        public GameState CurrentState => currentState;

        public event System.Action<GameState> OnStateChanged;

        protected override void OnInitialize() {
            currentState = GameState.Menu;
        }

        protected override void OnStart() {
            SetState(GameState.Playing);
        }

        public void SetState(GameState newState) {
            if (currentState == newState) return;

            GameState oldState = currentState;
            currentState = newState;

            Debug.Log($"[GameStateManager] Состояние: {oldState} -> {newState}");
            OnStateChanged?.Invoke(newState);

            switch (newState) {
                case GameState.Menu:
                    Time.timeScale = 1f;
                    break;

                case GameState.Playing:
                    Time.timeScale = 1f;
                    GameMenu gameMenu = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameMenu>();
                    gameMenu?.SetActive(false);
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    GameMenu gameMenuPaused = EntrypointBootstrapper.Instance?.Installer?.Resolve<GameMenu>();
                    gameMenuPaused?.SetActive(true);
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;

                case GameState.Victory:
                    Time.timeScale = 0f;
                    HUD hud = EntrypointBootstrapper.Instance?.Installer?.Resolve<HUD>();
                    hud?.GameWin();
                    break;
            }
        }

        public void TogglePause() {
            if (currentState == GameState.Playing) SetState(GameState.Paused);
            else if (currentState == GameState.Paused) SetState(GameState.Playing);
        }

        public void StartGame() => SetState(GameState.Playing);
        public void Victory() => SetState(GameState.Victory);
        public void GameOver() => SetState(GameState.GameOver);
    }
}
