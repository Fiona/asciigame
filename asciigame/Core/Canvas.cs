using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using asciigame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace asciigame.Core
{

    /*
     * Draws squares of sprites
     */
    class Canvas
    {
        private CanvasTile[,] canvasTiles;
        private List<(int x,int y)> dirtyTiles;
        private int screenWidth, screenHeight;
        private int canvasNumTilesVertical, canvasNumTilesHorizontal;
        private Vector2 canvasSize, canvasPos;
        private GlyphPalette glyphPalette;
        private AppConfig appConfig;

        public Canvas(int screenWidth, int screenHeight)
        {
            appConfig = AppConfig.Get();
            dirtyTiles = new List<(int,int)>();
            Resize(screenWidth, screenHeight);
            glyphPalette = new GlyphPalette();
            foreach(var i in Enumerable.Range(0x0021, 94))
                glyphPalette.GlyphIndex((char) i);
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
                    canvasTiles[x, y] = new CanvasTile {background = appConfig.canvasClearColour};
                    dirtyTiles.Add((x,y));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            glyphPalette.Draw(spriteBatch);
            foreach(var pos in dirtyTiles)
            {
                int x = pos.x;
                int y = pos.y;
                var tile = canvasTiles[x, y];
                var drawX = (int)canvasPos.X + x * appConfig.canvasTileSize;
                var drawY = (int)canvasPos.Y + y * appConfig.canvasTileSize;
                // Background
                PrimitiveDrawing.DrawRectangle(
                    spriteBatch,
                    new Vector2(drawX, drawY),
                    new Vector2(drawX + appConfig.canvasTileSize, drawY + appConfig.canvasTileSize),
                    tile.background
                );
                // Lower layer first
                if(tile.LowerLayer != null)
                    glyphPalette.DrawGlpyhIndexAt(
                        spriteBatch, glyphPalette.GlyphIndex(tile.LowerLayer.character),
                        drawX, drawY, tile.LowerLayer.rotation, tile.LowerLayer.colour
                    );
                // Middle layer second
                if(tile.MiddleLayer != null)
                    glyphPalette.DrawGlpyhIndexAt(
                        spriteBatch, glyphPalette.GlyphIndex(tile.MiddleLayer.character),
                        drawX, drawY, tile.MiddleLayer.rotation, tile.MiddleLayer.colour
                    );
                // Top layer last
                else if(tile.TopLayer != null)
                    glyphPalette.DrawGlpyhIndexAt(
                        spriteBatch, glyphPalette.GlyphIndex(tile.TopLayer.character),
                        drawX, drawY, tile.TopLayer.rotation, tile.TopLayer.colour
                    );
            }
            dirtyTiles.Clear();
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
                if(writeX >= canvasNumTilesHorizontal)
                {
                    if(!wrap)
                        return;
                    y++;
                    index = 1;
                    writeX = x;
                }

                if(x < 0 | x >= canvasNumTilesHorizontal )
                    return;
                if(y < 0 | y >= canvasNumTilesVertical)
                    return;
                WriteAt(chr, writeX, y, colour, clearTiles, clearColour, rotation);
            }
        }

        public void WriteAt(char chr, int x, int y, Color colour, bool clearTile = true, Color? clearColour = null,
            float rotation = 0)
        {
            if(!clearColour.HasValue)
                clearColour = appConfig.canvasClearColour;
            dirtyTiles.Add((x,y));
            if(clearTile)
                canvasTiles[x, y] = new CanvasTile {background = clearColour.Value, LowerLayer = null, MiddleLayer = null, TopLayer = null};
            if(chr == ' ')
                return;
            var tile = canvasTiles[x, y];
            // Try drawing bottom, middle and always draw on tap as a fallback
            if(tile.LowerLayer == null)
            {
                tile.LowerLayer = new CanvasTileLayer {character = chr, colour = colour, rotation = rotation};
                return;
            }

            if(tile.MiddleLayer == null)
            {
                tile.MiddleLayer = new CanvasTileLayer {character = chr, colour = colour, rotation = rotation};
                return;
            }

            tile.TopLayer = new CanvasTileLayer{character = chr, colour = colour, rotation = rotation};
            return;
        }
    }
}