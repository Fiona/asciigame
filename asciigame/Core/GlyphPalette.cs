using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using asciigame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TrueTypeSharp;

namespace asciigame.Core
{
    public class GlyphPalette
    {
        public char[] glyphs;
        private int lastInsertedIndex = -1;
        private TrueTypeFont font;
        private float fontScale;
        private Texture2D texture;
        private int textureSize;
        private AppConfig appConfig;

        public GlyphPalette()
        {
            appConfig = AppConfig.Get();
            glyphs = new char[appConfig.glyphPaletteSize * appConfig.glyphPaletteSize];

            textureSize = appConfig.glyphPaletteSize * appConfig.canvasTileSize;
            texture = new Texture2D(Window.Instance.GraphicsDevice, textureSize, textureSize);

            var blankTextureData = new Color[textureSize * textureSize];
            for(var i = 0; i<blankTextureData.Length; i++)
                blankTextureData[i] = Color.TransparentBlack;
            texture.SetData(blankTextureData);

            var fontPath = Path.Combine(appConfig.resourcesDirectory, appConfig.fallbackFontFile);
            font = new TrueTypeFont(fontPath);
            fontScale = font.GetScaleForPixelHeight(appConfig.canvasTileSize - 1);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            return;
            foreach(int x in Enumerable.Range(0, appConfig.glyphPaletteSize))
            {
                PrimitiveDrawing.DrawLine(
                    spriteBatch,
                    new Vector2(x * appConfig.canvasTileSize, 0),
                    new Vector2(x * appConfig.canvasTileSize, appConfig.glyphPaletteSize*appConfig.canvasTileSize),
                    Color.LightGray
                );
            }

            foreach(int y in Enumerable.Range(0, appConfig.glyphPaletteSize))
            {
                PrimitiveDrawing.DrawLine(
                    spriteBatch,
                    new Vector2(0, y * appConfig.canvasTileSize),
                    new Vector2(appConfig.glyphPaletteSize*appConfig.canvasTileSize, y * appConfig.canvasTileSize),
                    Color.LightGray
                );
            }

            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
        }

        public void DrawGlpyhIndexAt(SpriteBatch spriteBatch, int glyphIndex, int drawX, int drawY, float rotation, Color colour)
        {
            var texturePosX = Math.Max(0, (glyphIndex % appConfig.glyphPaletteSize) * appConfig.canvasTileSize);
            var texturePosY = (glyphIndex / appConfig.glyphPaletteSize) * appConfig.canvasTileSize;
            var halfTileSize = appConfig.canvasTileSize / 2;
            var origin = new Vector2(halfTileSize, halfTileSize);
            spriteBatch.Draw(
                texture,
                destinationRectangle:new Rectangle(drawX + (int)origin.X, drawY + (int)origin.Y, appConfig.canvasTileSize, appConfig.canvasTileSize),
                sourceRectangle:new Rectangle(texturePosX, texturePosY, appConfig.canvasTileSize, appConfig.canvasTileSize),
                color:colour, rotation: rotation.ToRadians(), origin:origin
            );
        }

        public int GlyphIndex(char glyph)
        {
            if(glyph == ' ' | glyph == '')
                return -1;
            var index = Array.FindIndex(glyphs, g => g == glyph);
            if(index > -1)
                return index;

            lastInsertedIndex++;
            glyphs[lastInsertedIndex] = glyph;

            AddGlyphToTexture(glyph, lastInsertedIndex);

            return lastInsertedIndex;
        }

        private void AddGlyphToTexture(char glyph, int index)
        {
            int width, height, xOffset, yOffset;
            var glyphIndex = font.FindGlyphIndex(glyph);
            if(glyphIndex == 0)
            {
                Debug.Print($"Glyph not found in font {glyph.ToString()}");
                return;
            }
            byte[] rawData = font.GetCodepointBitmap(
                glyph, fontScale,fontScale,
                out width, out height, out xOffset, out yOffset
            );
            if(width == 0 | height == 0)
                return;
            int boxX0, boxY0, boxX1, boxY1;
            font.GetCodepointBitmapBox(
                glyph, fontScale, fontScale, out boxX0, out boxY0, out boxX1, out boxY1
            );
            var textureData = new Color[width * height];
            foreach(int i in Enumerable.Range(0, width*height))
                textureData[i] = new Color(rawData[i],rawData[i],rawData[i], rawData[i]);
            var quarterTileSize = appConfig.canvasTileSize / 4;
            var baseline = ((index / appConfig.glyphPaletteSize) * appConfig.canvasTileSize) + appConfig.canvasTileSize - quarterTileSize;
            var tilePosX = Math.Max(0, ((index % appConfig.glyphPaletteSize) * appConfig.canvasTileSize) + boxX0);
            var tilePosY = Math.Max(0, baseline + boxY0);
            if(width + tilePosX > textureSize)
                width = textureSize - tilePosX;
            if(height + tilePosY > textureSize)
                height = textureSize - tilePosY;
            texture.SetData(
                0, new Rectangle(tilePosX, tilePosY, width, height), textureData, 0, width*height
            );
        }
    }
}