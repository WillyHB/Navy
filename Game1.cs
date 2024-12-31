﻿global using System.Diagnostics;

global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Microsoft.Xna.Framework.Audio;
global using Microsoft.Xna.Framework.Content;
global using Microsoft.Xna.Framework.Input;
global using Microsoft.Xna.Framework.Media;

using System;

namespace Navy
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Effect effect;
        public Level DefaultLevel { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Window.AllowUserResizing = true;

            Window.ClientSizeChanged += (sender, args) =>
            {
                ResolutionHandler.SetResolution(Window.ClientBounds.Width, Window.ClientBounds.Height);
            };

        }

        protected override void Initialize()
        {
            Globals.GraphicsManager = graphics;

            Globals.Window = Window;

            Globals.FramerateManager = new FramerateManager(SetFps);

            Window.TextInput += InputManager.Keyboard.TextInputHandler;

            Globals.WorldRenderTarget = new RenderTarget2D(GraphicsDevice, (int)ResolutionHandler.GetVirtualResolution().X, (int)ResolutionHandler.GetVirtualResolution().Y);
            Globals.LightRenderTarget = new RenderTarget2D(GraphicsDevice, (int)ResolutionHandler.GetVirtualResolution().X, (int)ResolutionHandler.GetVirtualResolution().Y);
            Globals.UIRenderTarget = new RenderTarget2D(GraphicsDevice, (int)ResolutionHandler.GetVirtualResolution().X, (int)ResolutionHandler.GetVirtualResolution().Y);

            Globals.DefaultFont = Content.Load<SpriteFont>("Defaults/Roboto");

            Globals.EmptyTexture = new Texture2D(GraphicsDevice, 1, 1);
            Globals.EmptyTexture.SetData([new Color(255, 255, 255)]);

            Globals.XTexture = Base64.ConvertBase64ToTexture(GraphicsDevice, "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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgE3988//AWzgLdqT4avkAAAAAElFTkSuQmCC");
            Globals.CheckTexture = Base64.ConvertBase64ToTexture(GraphicsDevice, "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");
            Globals.Game = this;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            Navy.Audio.Audio.Init(Content);

            base.Initialize();

            Globals.FramerateManager.SetTargetFps(100);

        }


        private void SetFps(int fps)
        {

            if (fps <= 0)
            {
                IsFixedTimeStep = false;
                return;
            }

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / fps);
        }

        protected override void LoadContent()
        {
            effect = Content.Load<Effect>("Defaults/Effect");

            Level.Load(DefaultLevel);
        }

        protected override void Update(GameTime gameTime)
        {
            Level.LevelUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Level.Draw(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            effect.Parameters["lightMask"].SetValue(Globals.LightRenderTarget);

            effect.Parameters["ambientLight"].SetValue(Level.CurrentLevel.AmbientLight.ToVector4());
            effect.CurrentTechnique.Passes[0].Apply();
            //spriteBatch.Draw(Globals.WorldRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(Globals.WorldRenderTarget, new Rectangle(0, 0, (int)ResolutionHandler.GetClientResolution().X, (int)ResolutionHandler.GetClientResolution().Y), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            //spriteBatch.Draw(Globals.UIRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(Globals.UIRenderTarget, new Rectangle(0, 0, (int)ResolutionHandler.GetClientResolution().X, (int)ResolutionHandler.GetClientResolution().Y), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}