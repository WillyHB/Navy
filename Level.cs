using Navy.UI;
using Navy.ECS;
using System.Collections.Generic;
using System;

namespace Navy
{
    public abstract class Level
    {
        public Level()
        {
            Camera = new Camera();
            Content = new ContentManager(Globals.Game.Services, "Content");
        }

        public Color AmbientLight { get; set; } = Color.White;
        public Color BackgroundColor { get; set; } = Color.Black;

        public ContentManager Content { get; set; }

        protected abstract void Update(GameTime gameTime);
        protected abstract void PreDraw(SpriteBatch spriteBatch);
        protected abstract void PostDraw(SpriteBatch spriteBatch);

        public static event EventHandler<Level> Loaded;
        public event EventHandler Exiting;

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentLevel != null)
            {
                Globals.GraphicsManager.GraphicsDevice.SetRenderTarget(Globals.WorldRenderTarget);
                Globals.GraphicsManager.GraphicsDevice.Clear(Color.Transparent);

                CurrentLevel.PreDraw(spriteBatch);

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

                CurrentLevel.PostDraw(spriteBatch);

                Globals.GraphicsManager.GraphicsDevice.SetRenderTarget(Globals.LightRenderTarget);
                Globals.GraphicsManager.GraphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, transformMatrix: Level.CurrentLevel.Camera.GetViewMatrix());

                foreach (LightSource lightSource in GameObject.FindComponents<LightSource>())
                {
                    if (lightSource.Enabled && lightSource.gameObject.Enabled)
                    {
                        lightSource.Draw(spriteBatch);
                    }
                }

                spriteBatch.End();

                spriteBatch.GraphicsDevice.SetRenderTarget(Globals.UIRenderTarget);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);




                foreach (Canvas canvas in GameObject.FindComponents<Canvas>())
                {
                    if (canvas.Enabled && canvas.gameObject.Enabled && !canvas.RenderInWorld)
                    {
                        canvas.Draw(spriteBatch);
                    }
                }

                spriteBatch.GraphicsDevice.SetRenderTarget(null);
                spriteBatch.GraphicsDevice.Clear(Level.CurrentLevel.BackgroundColor);
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

        public Camera Camera { get; set; }

        public static Level CurrentLevel { get; private set; }

        public void Instantiate(GameObject go)
        {
            GameObjects.Add(go);

        }

        internal List<GameObject> GameObjects { get; private set; } = [];
    }
}
