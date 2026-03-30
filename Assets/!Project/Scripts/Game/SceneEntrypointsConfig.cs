using UnityEngine;

namespace RPGProject
{
    /// <summary>
    /// Пример конфигурации Entrypoints для сцены.
    /// </summary>
    public class SceneEntrypointsConfig : MonoBehaviour
    {
        [Header("Entrypoints для сцены Main")]
        [SerializeField] private Player _player;
        [SerializeField] private HUD _hud;
        [SerializeField] private GameMenu _gameMenu;
        [SerializeField] private CameraController _cameraController;

        private void Awake()
        {
            if (EntrypointBootstrapper.Instance == null)
            {
                var bootstrapperObj = new GameObject("EntrypointBootstrapper");
                var bootstrapper = bootstrapperObj.AddComponent<EntrypointBootstrapper>();

                bootstrapper.GetType()
                    .GetField("_entrypoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(bootstrapper, new GameEntrypoint[]
                    {
                        _player,
                        _hud,
                        _gameMenu,
                        _cameraController
                    });

                DontDestroyOnLoad(bootstrapperObj);
                bootstrapper.Initialize();
            }
        }
    }
}
