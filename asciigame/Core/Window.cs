using System;
using System.IO;
using asciigame.Data;
using asciigame.Game;
using asciigame.UI;
using asciigame.UI.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;


namespace asciigame.Core
{
    class Window : Microsoft.Xna.Framework.Game
    {
        private static Window _instance;
        public static Window Instance => _instance;

        public SpriteFont debugFont;

        private Canvas canvas;
        private GraphicsDeviceManager graphics;
        private GameManager gameManager;
        private UIManager uiManager;
        private SpriteBatch spriteBatch;
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
            IsMouseVisible = true;
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            currentWindowSize = (Window.ClientBounds.Width, Window.ClientBounds.Height);
            canvas.Resize(currentWindowSize.width, currentWindowSize.height);
            uiManager.Resize((canvas.canvasNumTilesHorizontal, canvas.canvasNumTilesVertical));
            RecreateDrawBuffer();
        }

        protected override void Initialize()
        {
            base.Initialize();
            canvas = new Canvas(currentWindowSize.width, currentWindowSize.height);
            uiManager = new UIManager(new WidgetPanel(null));
            gameManager = new GameManager(uiManager);
            uiManager.Resize((canvas.canvasNumTilesHorizontal, canvas.canvasNumTilesVertical));
        }

        private RenderTarget2D drawBuffer;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            RecreateDrawBuffer();

            debugFont = Content.Load<SpriteFont>(Path.Combine("Resources", "DejaVuSansMono"));
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

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if(Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var fps = 1f / deltaTime;
            /*
            canvas.WriteAt(
                $"[ fps: {fps:F} ft: {gameTime.ElapsedGameTime.TotalMilliseconds:F}ms ]",
                0, 0, Color.White
            );
*/
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Redraw UI
            uiManager.Draw(canvas);

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