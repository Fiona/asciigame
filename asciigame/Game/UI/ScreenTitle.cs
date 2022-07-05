using asciigame.UI;
using asciigame.UI.Widgets;


namespace asciigame.Game.UI
{
    public class ScreenTitle: BaseScreen
    {

        private WidgetPanel testPanel;
        private WidgetPanel testPanel2;

        public ScreenTitle(WidgetPanel root) : base(root)
        {
            testPanel = new WidgetPanel((Widget)root);
            testPanel.SetStyle("primaryPanel");
            testPanel.SetLayout(WidgetLayout.MaxWidthHeight);
            testPanel.SetTitle("Asciigame!");
            testPanel.showDebugText = true;

            testPanel2 = new WidgetPanel(testPanel);
            testPanel2.SetStyle("testPanel");
            testPanel2.SetLayout(WidgetLayout.MaxWidthHeight);
            testPanel2.showDebugText = true;

        }
    }
}