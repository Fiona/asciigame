using System.Collections.Generic;
using asciigame.UI;

namespace asciigame.Data
{
    public class WidgetStylesData: Data<WidgetStylesData>
    {
        public Dictionary<string, WidgetStyle> styles;

        protected override string GetDataFilePath()
        {
            return "WidgetStyles.json";
        }

        public override void Initialize()
        {
            foreach(var style in styles)
            {
                var alteredStyle = styles[style.Key];
                alteredStyle.name = style.Key;
                styles[style.Key] = alteredStyle;
            }
        }

    }
}