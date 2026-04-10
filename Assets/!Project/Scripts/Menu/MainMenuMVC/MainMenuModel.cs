namespace RPGProject
{
    public class MainMenuModel
    {
        public void NewGame()
        {
            Settings.isLoadGame = false;
        }

        public void LoadGame()
        {
            Settings.isLoadGame = true;
        }
    }
}