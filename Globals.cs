namespace Navy;

public static class Globals
{
    public static int GameScale { get; set; } = 5;
    public static GameWindow Window { get; set; }
    public static FramerateManager FramerateManager { get; set; }

    public static GraphicsDeviceManager GraphicsManager { get; set; }
    public static Game Game { get; set; }
    public static RenderTarget2D WorldRenderTarget { get; set; }
    public static RenderTarget2D LightRenderTarget { get; set; }
    public static RenderTarget2D UIRenderTarget { get; set; }

    public static Texture2D EmptyTexture { get; set; }
    public static Texture2D XTexture { get; set; }
    public static Texture2D CheckTexture { get; set; }
    public static SpriteFont DefaultFont { get; set; }
}


