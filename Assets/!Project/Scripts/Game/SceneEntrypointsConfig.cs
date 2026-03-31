using UnityEngine;

namespace RPGProject {
    public class SceneEntrypointsConfig : MonoBehaviour {
        [Header("Entrypoints для сцены Main")]
        [SerializeField] Player player;
        [SerializeField] HUD hud;
        [SerializeField] GameMenu gameMenu;
        [SerializeField] CameraController cameraController;
        [SerializeField] GameManager gameManager;

        void Awake() {
            if (EntrypointBootstrapper.Instance == null) {
                GameObject bootstrapperObj = new GameObject("EntrypointBootstrapper");
                EntrypointBootstrapper bootstrapper = bootstrapperObj.AddComponent<EntrypointBootstrapper>();

                bootstrapper.GetType().GetField("entrypoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(bootstrapper, new GameEntrypoint[]{
                        player,
                        hud,
                        gameMenu,
                        cameraController,
                        gameManager
                    });

                DontDestroyOnLoad(bootstrapperObj);
                bootstrapper.Initialize();
            }
        }
    }
}
