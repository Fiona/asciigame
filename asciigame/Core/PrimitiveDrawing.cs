using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace asciigame.Core
{
    public class PrimitiveDrawing
    {
        private static Texture2D _whitePixelTexture;

        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if(_whitePixelTexture != null)
                return _whitePixelTexture;

            _whitePixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _whitePixelTexture.SetData(new[] { Color.White });
            spriteBatch.Disposing += (sender, args) =>
            {
                _whitePixelTexture?.Dispose();
                _whitePixelTexture = null;
            };
            return _whitePixelTexture;
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f, float layerDepth = 0)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness, layerDepth);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f, float layerDepth = 0)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, layerDepth);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float layerDepth = 0)
        {
            spriteBatch.Draw(
                GetTexture(spriteBatch),
                point1,
                new Rectangle(
                    (int) point1.X, (int) point1.Y, (int) point2.X - (int) point1.X, (int) point2.Y - (int) point1.Y
                ),
                color
            );
        }

    }
}