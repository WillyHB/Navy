namespace Navy
{
    public static class ResolutionHandler
    {
        public static readonly ushort[] Widths = new ushort[] { 1024, 1280, 1280, 1280, 1360, 1366, 1440, 1600, 1680, 1920, 1920, 2560, 2560, 2560, 3440, 3840 };
        public static readonly ushort[] Heights = new ushort[] { 768, 1024, 720,  800,  768,  768,  900,  900,  1050, 1080, 1200, 1080, 1600, 1440, 1440, 2160};

        public static void SetResolution(int index)
        {
            if (index < Widths.Length && index > 0)
            {
                SetResolution(Widths[index], Heights[index]);
            }
        }

        public static void SetResolution(int width, int height)
        {
            Globals.GraphicsManager.PreferredBackBufferWidth = width;
            Globals.GraphicsManager.PreferredBackBufferHeight = height;
            Globals.GraphicsManager.ApplyChanges();

            Globals.WorldRenderTarget.Dispose();
            Globals.LightRenderTarget.Dispose();
            Globals.UIRenderTarget.Dispose();

            Globals.WorldRenderTarget = new RenderTarget2D(Globals.GraphicsManager.GraphicsDevice, (int)GetVirtualResolution().X, (int)GetVirtualResolution().Y);
            Globals.LightRenderTarget = new RenderTarget2D(Globals.GraphicsManager.GraphicsDevice, (int)GetVirtualResolution().X, (int)GetVirtualResolution().Y);
            Globals.UIRenderTarget = new RenderTarget2D(Globals.GraphicsManager.GraphicsDevice, (int)GetVirtualResolution().X, (int)GetVirtualResolution().Y);
        }

        public static Vector2 GetClientResolution()
        {
            if (Globals.GraphicsManager.IsFullScreen)
            {
                return new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            }

            return new Vector2(Globals.GraphicsManager.PreferredBackBufferWidth, Globals.GraphicsManager.PreferredBackBufferHeight);
        }

        public static Vector2 GetVirtualClientRatio() => new Vector2(GetClientResolution().X / GetVirtualResolution().X, GetClientResolution().Y / GetVirtualResolution().Y);


        private static float renderScale = 1.0f;
        private const int renderScreenHeight = 1080;

        public static float AspectRatio
        {
            get => GetClientResolution().X / GetClientResolution().Y;
        }

        public static Vector2 GetVirtualResolution()
        {
            var scaledHeight = (float)renderScreenHeight / renderScale;
            return new Vector2(AspectRatio * scaledHeight, scaledHeight);
        }


        /*
         

        WHEN INSTANTIATING UI, THE RECT IS SET IN STONE. CHANGE SO THE UI CAN SCALE ALONGSIDE THE SCALING OF THE ASPECT RATIO / CLIENT.

      



        */
    }
}
