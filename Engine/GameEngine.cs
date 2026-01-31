using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace GAlgoT2530.Engine
{
    public class GameEngine : Game
    {
        // MonoGame or C# related
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;
        public Random Random;

        // Custom Built Sub-engines
        public CollisionEngine CollisionEngine;
        public SceneManager SceneManager;

        private int _backbufferWidth;
        private int _backbufferHeight;

        public GameEngine(string windowTitle, int backbufferWidth, int backbufferHeight, bool fullScreen = false)
        {
            _backbufferWidth = backbufferWidth;
            _backbufferHeight = backbufferHeight;

            Window.Title = windowTitle;
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = fullScreen;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Initialize Sub-engines
            CollisionEngine = new CollisionEngine();
            SceneManager = new SceneManager();
            Random = new Random();

            // Set Game Engine and Scene Manager for Game Object
            GameScene.SetGame(this);
            GameScene.SetSceneManager(SceneManager);

            // Set Game Engine for Game Object
            GameObject.SetGame(this);

            // Initialize Scalable Game Time
            ScalableGameTime.TimeScale = 1f;
        } 

        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Change back buffer size
            Graphics.PreferredBackBufferWidth = _backbufferWidth;
            Graphics.PreferredBackBufferHeight = _backbufferHeight;
            Graphics.ApplyChanges();

            // TOIMPROVE: Preload() may be redundant in the future and may be removed.
            // Initialize Scene
            SceneManager.CurrentScene.Preload();
            SceneManager.CurrentScene.CreateScene();

            // Initialize and load assets for all game objects
            GameObjectCollection.LoadAndInitialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // Compute scaled time
            ScalableGameTime.Process(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TOIMPROVE: This may be redundant in the future and may be removed.
            SceneManager.CurrentScene.Update();

            GameObjectCollection.Update();

            CollisionEngine.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GameObjectCollection.Draw();
        }

        protected override void EndDraw()
        {
            base.EndDraw();
            GameObjectCollection.EndDraw();
        }

        public void AddScene(string name, GameScene scene, bool asCurrent = false)
        {
            try
            {
                SceneManager.AddScene(name, scene);
                if (asCurrent)
                {
                    SceneManager.ChangeScene(name);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}