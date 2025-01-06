using Navy.UI;
using Navy.ECS;
using System.Collections.Generic;
using System;

namespace Navy
{
    public abstract class Level
    {

        public Color AmbientLight { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Black;

        public ContentManager Content { get; set; }
        public Camera Camera { get; set; }
        public static Level CurrentLevel { get; private set; }
        internal List<GameObject> GameObjects { get; private set; } = [];


        protected abstract void Update(GameTime gameTime);
        protected abstract void PreDraw(SpriteBatch spriteBatch);
        protected abstract void PostDraw(SpriteBatch spriteBatch);

        public static event EventHandler<Level> Loaded;
        public event EventHandler Exiting;
        public Level()
        {
            Camera = new Camera();
            Content = new ContentManager(Globals.Game.Services, "Content");
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentLevel != null)
            {
                // RENDER WORLD GAMEOBJECTS
                Globals.GraphicsManager.GraphicsDevice.SetRenderTarget(Globals.WorldRenderTarget);
                Globals.GraphicsManager.GraphicsDevice.Clear(Color.Transparent);

                CurrentLevel.PreDraw(spriteBatch);

                DrawWorld(spriteBatch);

                CurrentLevel.PostDraw(spriteBatch);

                // RENDER LIGHTS
                Globals.GraphicsManager.GraphicsDevice.SetRenderTarget(Globals.LightRenderTarget);
                Globals.GraphicsManager.GraphicsDevice.Clear(Color.Transparent);

                DrawLights(spriteBatch);


                // RENDER UI
                spriteBatch.GraphicsDevice.SetRenderTarget(Globals.UIRenderTarget);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                DrawUI(spriteBatch);

                spriteBatch.GraphicsDevice.SetRenderTarget(null);
                spriteBatch.GraphicsDevice.Clear(Level.CurrentLevel.BackgroundColor);


            }
        }

        private static void DrawLights(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, transformMatrix: Level.CurrentLevel.Camera.GetViewMatrix());

            foreach (LightSource lightSource in GameObject.FindComponents<LightSource>())
            {
                if (lightSource.Enabled && lightSource.gameObject.Enabled)
                {
                    lightSource.Draw(spriteBatch);
                }
            }

            spriteBatch.End();
        }

        private static void DrawUI(SpriteBatch spriteBatch)
        {

            foreach (Canvas canvas in GameObject.FindComponents<Canvas>())
            {
                if (canvas.Enabled && canvas.gameObject.Enabled && !canvas.RenderInWorld)
                {
                    canvas.Draw(spriteBatch);
                }
            }
        }
        
        private static void DrawWorld(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: CurrentLevel.Camera.GetViewMatrix());

            for (int i = 0; i < CurrentLevel.GameObjects.Count; i++)
            {
                if (CurrentLevel.GameObjects[i].Enabled)
                {
                    CurrentLevel.GameObjects[i].Draw(spriteBatch);
                }
            }

            spriteBatch.End();

            foreach (Canvas canvas in GameObject.FindComponents<Canvas>())
            {
                if (canvas.Enabled && canvas.gameObject.Enabled && canvas.RenderInWorld)
                {
                    canvas.Draw(spriteBatch);
                }
            }
        }

        public static void LevelUpdate(GameTime gameTime)
        {
            InputManager.Update();

            if (CurrentLevel != null)
            {
                for (int i = 0; i < CurrentLevel.GameObjects.Count; i++)
                {
                    CurrentLevel.GameObjects[i].Update(gameTime);
                }

                Tweener.Update(gameTime);
                Audio.Audio.Update(gameTime);
                FieldWatcher.Update();
                CurrentLevel.Update(gameTime);
            }
        }
        public abstract void Initialize();

        public abstract void Exit();

        public static T Load<T>() where T : Level, new()
        {
            var level = (T)Activator.CreateInstance(typeof(T));
            return (T)Load(level);
        }


        public static Level Load(Level level)
        {
            if (CurrentLevel != null)
            {
                CurrentLevel.Exiting?.Invoke(null, EventArgs.Empty);
                CurrentLevel.Exiting = null;

                CurrentLevel.Exit();

                CurrentLevel.Content.Unload();

                while (CurrentLevel.GameObjects.Count > 0)
                {
                    if (!CurrentLevel.GameObjects[0].DestroyOnLoad)
                    {
                        level.Instantiate(CurrentLevel.GameObjects[0]);
                        CurrentLevel.GameObjects.RemoveAt(0);
                        continue;
                    }

                    CurrentLevel.GameObjects[0].Destroy();
                }
            }

            CurrentLevel = level;
            //LevelAwake?.Invoke(CurrentLevel);
            Loaded?.Invoke(null, level);
            CurrentLevel.Initialize();


            //LevelStart?.Invoke(CurrentLevel);

            return level;
        }


        public GameObject Instantiate(GameObject go)
        {
            GameObjects.Add(go);

            return go;

        }

    }
}
