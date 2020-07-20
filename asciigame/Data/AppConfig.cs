using asciigame.Core;
using Microsoft.Xna.Framework;

namespace asciigame.Data
{
    public class AppConfig: Data<AppConfig>
    {
        public int[] windowSize;
        public Color windowClearColour;
        public int glyphPaletteSize;
        public int canvasTileSize;
        public Color canvasClearColour;
        public string resourcesDirectory;
        public string fallbackFontFile;

        protected override string GetDataFilePath()
        {
            return "AppConfig.json";
        }
    }
}