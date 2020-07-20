using Microsoft.Xna.Framework;

namespace asciigame.Core
{
    public class CanvasTileLayer
    {
        public char character;
        public Color colour;
        public float rotation;

        public override string ToString()
        {
            return
                $"Layer[ Chr: {character}, Col: {colour.ToString()} ]";
        }

    }

    public class CanvasTile
    {
        public Color background;
        public CanvasTileLayer LowerLayer;
        public CanvasTileLayer MiddleLayer;
        public CanvasTileLayer TopLayer;

        public override string ToString()
        {
            return
                $"Tile[ BG: {background.ToString()}, L: {LowerLayer}, M: {MiddleLayer}, T: {TopLayer} ]";
        }
    }
}