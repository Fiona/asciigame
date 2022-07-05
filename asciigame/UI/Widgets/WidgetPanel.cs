using asciigame.Core;

namespace asciigame.UI.Widgets
{
    public class WidgetPanel: Widget
    {
        private string title;

        public WidgetPanel(Widget parent) : base(parent)
        {
            title = "";
        }

        public void SetTitle(string newTitle)
        {
            title = newTitle;
        }

        protected override void DrawAdditional(Canvas canvas)
        {
            if(title == "")
                return;
            var completeTitle = $"{style.titleChars.leftChar} {title} {style.titleChars.rightChar}";
            canvas.WriteAt(completeTitle, bounds.x + 1, bounds.y, clearTiles: true,
                colour: style.borderColour, clearColour: style.borderColourBackground);
        }
    }
}