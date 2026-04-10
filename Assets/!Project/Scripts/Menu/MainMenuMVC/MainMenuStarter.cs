using UnityEngine;

namespace RPGProject
{
    public class MainMenuStarter : MonoBehaviour
    {
        [SerializeField] private MainMenuView view;

        private void Start()
        {
            MainMenuModel model = new MainMenuModel();
            MainMenuController controller = new MainMenuController(model, view);
        }
    }
}