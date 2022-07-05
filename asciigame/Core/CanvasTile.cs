using System;
using System.Collections.Generic;
using System.Linq;
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

        public override bool Equals(object value)
        {
            return (
                value is CanvasTileLayer layer &&
                Equals(character, layer.character) &&
                Equals(colour, layer.colour) &&
                Math.Abs(layer.rotation - rotation) < 0.01f
            );
        }
    }

    public class CanvasTile
    {
        public Color background;
        public List<CanvasTileLayer> layers;

        public override string ToString()
        {
            return
                $"Tile[ BG: {background.ToString()}, Layers: {layers} ]";
        }

        public override bool Equals(object value)
        {
            return (
                value is CanvasTile tile &&
                Equals(background, tile.background) &&
                layers.SequenceEqual(tile.layers)
            );
        }
    }
}