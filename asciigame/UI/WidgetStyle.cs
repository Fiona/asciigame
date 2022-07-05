using System.Diagnostics;
using asciigame.Data;
using Microsoft.Xna.Framework;

namespace asciigame.UI
{
    public struct WidgetStyle
    {
        public string name;
        public (int top, int right, int bottom, int left) padding;
        public (int top, int right, int bottom, int left) margin;
        public (char top, char right, char bottom, char left) border;
        public (char topLeft, char topRight, char bottomRight, char bottomleft) corners;
        public Color backgroundColour;
        public Color borderColour;
        public Color borderColourBackground;
        public (char leftChar, char rightChar) titleChars;

        public static WidgetStyle Get(string name)
        {
            var styles = WidgetStylesData.Get();
            if(!styles.styles.ContainsKey(name))
            {
                Debug.Print($"Style does not exist: {name}");
                return new WidgetStyle
                {
                    name = "none",
                    backgroundColour = new Color(0,0,0,0)
                };
            }

            return styles.styles[name];
        }
    }
}