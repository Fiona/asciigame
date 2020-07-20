using System;
using System.Drawing;
using asciigame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;


namespace asciigame.Core
{
    class Window : Game
    {
        private static Window _instance;
        public static Window Instance => _instance;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Canvas canvas;
        private AppConfig appConfig;
        private (int width, int height) currentWindowSize;

        public Window()
        {
            _instance = this;
            appConfig = AppConfig.Get();
            currentWindowSize = (appConfig.windowSize[0], appConfig.windowSize[1]);
            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = currentWindowSize.width,
                PreferredBackBufferHeight = currentWindowSize.height,
                SynchronizeWithVerticalRetrace = false
            };


            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            currentWindowSize = (Window.ClientBounds.Width, Window.ClientBounds.Height);
            canvas.Resize(currentWindowSize.width, currentWindowSize.height);
            RecreateDrawBuffer();
        }

        protected override void Initialize()
        {
            base.Initialize();
            canvas = new Canvas(currentWindowSize.width, currentWindowSize.height);
        }

        private RenderTarget2D drawBuffer;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            RecreateDrawBuffer();
        }

        private void RecreateDrawBuffer()
        {
            drawBuffer?.Dispose();
            var pp = graphics.GraphicsDevice.PresentationParameters;
            drawBuffer = new RenderTarget2D(
                graphics.GraphicsDevice, currentWindowSize.width, currentWindowSize.height,
                true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, pp.MultiSampleCount,
                RenderTargetUsage.PreserveContents
            );
        }

        protected override void UnloadContent()
        {
            drawBuffer?.Dispose();
        }

        private double num;

        private string lipsum =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla a feugiat neque. Suspendisse fringilla nec orci gravida dignissim. Nunc maximus mauris blandit est tincidunt venenatis. Proin pretium, metus ut malesuada convallis, mauris justo iaculis enim, dignissim ultrices elit quam nec turpis. Vestibulum lobortis, est aliquet consequat semper, felis tellus aliquam tortor, id efficitur sem lectus vel mauris. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. ";

        protected override void Update(GameTime gameTime)
        {
            if(Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var fps = 1000f / gameTime.ElapsedGameTime.TotalMilliseconds;
            canvas.WriteAt(
                $"[ fps: {fps:F} ft: {gameTime.ElapsedGameTime.TotalMilliseconds:F}ms ]",
                0, 0, Color.White
            );

            var frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            num += 1f * frameTime * 50f;
            canvas.WriteAt(
                num >= lipsum.Length ? lipsum : lipsum.Substring(0, (int)num),
                0, 3, Color.Lime
            );

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Draw canvas to off-screen buffer
            graphics.GraphicsDevice.SetRenderTarget(drawBuffer);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            canvas.Draw(spriteBatch);
            spriteBatch.End();

            // Draw screen buffer
            graphics.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            var screenRect = new Rectangle(0, 0, currentWindowSize.width, currentWindowSize.height);
            spriteBatch.Draw(drawBuffer, screenRect, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}