namespace Navy.ECS
{
    public class SpriteRenderer : Component, IDrawableComponent
    {
        public SpriteRenderer()
        {

        }

        public SpriteRenderer(Texture2D texture, Color color, Rectangle? textureRect, bool textureFlipped = false)
        {
            Texture = texture;
            Color = color;
            TextureRect = textureRect;
            TextureFlipped = textureFlipped;
        }

        public Texture2D Texture { get; set; }
        public Color Color { get; set; } = Color.White;
        public Rectangle? TextureRect { get; set; }
        public bool TextureFlipped { get; set; }

        public Vector2 Size
        {
            get => size ?? TextureRect.GetValueOrDefault().Size.ToVector2();
            set => size = value;
        }

        private Vector2? size;
        public Rectangle Rect
        {
            get
            {
               
                return new Rectangle((int)gameObject.Transform.Position.X - (TextureRect.HasValue ? TextureRect.Value.Width / 2 : Texture.Width / 2), (int)gameObject.Transform.Position.Y - (TextureRect.HasValue ? TextureRect.Value.Height / 2 : Texture.Height), TextureRect.HasValue ? TextureRect.Value.Width : Texture.Width, TextureRect.HasValue ? TextureRect.Value.Height : Texture.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float layerDepth = 1.0f / gameObject.Transform.Position.Y;
            if (!float.IsNormal(layerDepth)) layerDepth = 0;
            spriteBatch.Draw(Texture, new Rectangle(gameObject.Transform.Position.ToPoint(), new Point((int)Size.X * Globals.GameScale, (int)Size.Y * Globals.GameScale)), TextureRect, Color, 0, Vector2.Zero, TextureFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, layerDepth);
        }
    }
}
