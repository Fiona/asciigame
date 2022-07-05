using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using asciigame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace asciigame.Core
{

    /*
     * Draws squares of sprites
     */
    public class Canvas
    {
        public int canvasNumTilesVertical, canvasNumTilesHorizontal;

        private CanvasTile[,] canvasTiles;
        private List<(int x,int y)> dirtyTiles;
        private int screenWidth, screenHeight;
        private Vector2 canvasSize, canvasPos;
        private GlyphPalette glyphPalette;
        private AppConfig appConfig;

        private List<(float x, float y, string text)> debugText;

        public Canvas(int screenWidth, int screenHeight)
        {
            appConfig = AppConfig.Get();
            dirtyTiles = new List<(int,int)>();
            Resize(screenWidth, screenHeight);
            glyphPalette = new GlyphPalette();
            foreach(var i in Enumerable.Range(0x0021, 94))
                glyphPalette.GlyphIndex((char) i);

            debugText = new List<(float x, float y, string text)>();
        }

        public void Resize(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            canvasNumTilesVertical = screenHeight / appConfig.canvasTileSize;
            canvasNumTilesHorizontal = screenWidth / appConfig.canvasTileSize;
            canvasSize = new Vector2(
                canvasNumTilesHorizontal * appConfig.canvasTileSize,
                canvasNumTilesVertical * appConfig.canvasTileSize
            );
            canvasPos = new Vector2(
                Math.Max(0, (screenWidth-canvasSize.X) / 2),
                Math.Max(0, (screenHeight-canvasSize.Y) / 2)
            );

            dirtyTiles.Clear();
            canvasTiles = new CanvasTile[canvasNumTilesHorizontal, canvasNumTilesVertical];

            foreach(var x in Enumerable.Range(0, canvasNumTilesHorizontal))
            {
                foreach(var y in Enumerable.Range(0, canvasNumTilesVertical))
                {
                    canvasTiles[x, y] = new CanvasTile
                    {
                        background = appConfig.canvasClearColour,
                        layers = new List<CanvasTileLayer>()
                    };
                    dirtyTiles.Add((x,y));
                }
            }
        }

        public (int x, int y) GetScreenDrawPos(int tileX, int tileY)
        {
            var drawX = (int)canvasPos.X + tileX * appConfig.canvasTileSize;
            var drawY = (int)canvasPos.Y + tileY * appConfig.canvasTileSize;
            return (drawX, drawY);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            glyphPalette.Draw(spriteBatch);
            foreach(var pos in dirtyTiles)
            {
                int x = pos.x;
                int y = pos.y;
                var tile = canvasTiles[x, y];
                var (drawX, drawY) = GetScreenDrawPos(x, y);
                //var drawX = (int)canvasPos.X + x * appConfig.canvasTileSize;
                //var drawY = (int)canvasPos.Y + y * appConfig.canvasTileSize;
                // Background
                PrimitiveDrawing.DrawRectangle(
                    spriteBatch,
                    new Vector2(drawX, drawY),
                    new Vector2(drawX + appConfig.canvasTileSize, drawY + appConfig.canvasTileSize),
                    tile.background
                );
                foreach(var layer in tile.layers)
                {
                    glyphPalette.DrawGlpyhIndexAt(
                        spriteBatch, glyphPalette.GlyphIndex(layer.character),
                        drawX, drawY, layer.rotation, layer.colour
                    );
                }
            }
            dirtyTiles.Clear();
            foreach(var singleDebugText in debugText)
            {
                spriteBatch.DrawString(
                    Window.Instance.debugFont,
                    singleDebugText.text,
                    new Vector2(singleDebugText.x, singleDebugText.y),
                    Color.White
                );
            }
            debugText.Clear();
        }

        public void DrawDebugText(string text, float posX, float posY)
        {
            debugText.Add((posX, posY, text));
        }

        public void WriteAt(string text, int x, int y, Color colour, bool clearTiles = true, Color? clearColour = null,
            float rotation = 0, bool wrap = true)
        {
            if(!clearColour.HasValue)
                clearColour = appConfig.canvasClearColour;
            var index = 0;
            foreach(var chr in text)
            {
                var writeX = x + index;
                index++;
                if(writeX > canvasNumTilesHorizontal)
                {
                    if(!wrap)
                        return;
                    y++;
                    index = 0;
                    writeX = x;
                }

                if(x < 0 || x > canvasNumTilesHorizontal)
                    return;
                if(y < 0 || y > canvasNumTilesVertical)
                    return;
                WriteAt(chr, writeX, y, colour, clearTiles, clearColour, rotation);
            }
        }

        public void WriteAt(char chr, int x, int y, Color colour, bool clearTile = true, Color? clearColour = null,
            float rotation = 0)
        {
            if(!clearColour.HasValue)
                clearColour = appConfig.canvasClearColour;

            if(clearTile)
            {
                var newTile = new CanvasTile
                {
                    background = clearColour.Value,
                    layers = new List<CanvasTileLayer>()
                };
                if(chr != ' ')
                    newTile.layers.Add(new CanvasTileLayer {character = chr, colour = colour, rotation = rotation});

                if(Equals(newTile, canvasTiles[x,y]))
                    return;
                canvasTiles[x, y] = newTile;
                dirtyTiles.Add((x,y));
                return;
            }

            if(chr == ' ')
                return;
            dirtyTiles.Add((x,y));
            var tile = canvasTiles[x, y];
            tile.layers.Add(new CanvasTileLayer {character = chr, colour = colour, rotation = rotation});
        }
    }
}