using System.Collections.Generic;
using asciigame.Game.UI;
using asciigame.UI;
using asciigame.UI.Widgets;

namespace asciigame.Game
{
    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        private UIManager uiManager;
        private Dictionary<string, BaseScreen> uiScreens;

        public GameManager(UIManager uiManager)
        {
            _instance = this;
            this.uiManager = uiManager;
            uiScreens = new Dictionary<string, BaseScreen>{
                {"title", new ScreenTitle((WidgetPanel)uiManager.root)}
            };
        }
    }
}