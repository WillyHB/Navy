namespace Navy.UI
{
    public class Image : CanvasElement
    {
        public Image() { }
        public Image(Rectangle? RawRect, Texture2D texture)
        {
            Texture = texture;
            this.RawRect = RawRect ?? new Rectangle(0, 0, texture.Bounds.Width, texture.Bounds.Height);
        }

        public float Rotation { get; set; } = 0;
        public Rectangle? TextureRect { get; set; } = null;

        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect, TextureRect, BackgroundColor, Rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
