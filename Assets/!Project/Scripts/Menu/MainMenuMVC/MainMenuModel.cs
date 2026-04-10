namespace RPGProject
{
    public class MainMenuModel
    {
        public void NewGame()
        {
            Settings.IsPeacefulGame = false;
            Settings.isLoadGame = false;
        }

        public void LoadGame()
        {
            Settings.IsPeacefulGame = false;
            Settings.isLoadGame = true;
        }

        public void PeacefulGame()
        {
            Settings.IsPeacefulGame = true;
        }
    }
}