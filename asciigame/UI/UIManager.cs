using asciigame.Core;

namespace asciigame.UI
{
    public class UIManager
    {
        public Widget root;

        private (int width, int height) uiSize;

        public UIManager(Widget root)
        {
            this.root = root;
        }

        public void Resize((int width, int height) newUiSize)
        {
            uiSize = newUiSize;
            root.UpdatePosSize((0, 0), newUiSize);
        }

        public void Draw(Canvas canvas)
        {
            root.Draw(canvas);
        }
    }
}