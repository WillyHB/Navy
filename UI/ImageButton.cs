namespace Navy.UI
{
    public class ImageButton : CanvasElement
    {
        public ImageButton(Rectangle RawRect, Texture2D backgroundTex, Texture2D buttonImageTex)
        {
            Texture = backgroundTex;
            ButtonImage = buttonImageTex;
            this.RawRect = RawRect;
        }
        public bool TextureFlipped { get; set; }

        public Texture2D ButtonImage { get; set; }

        public float ImageScale { get; set; } = 1;

        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect, null, BackgroundColor, 0, Vector2.Zero, TextureFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            spriteBatch.Draw(ButtonImage, new Rectangle((int)(Rect.X + Rect.Width / 2.0f), (int)(Rect.Y + Rect.Height / 2.0f), (int)(Rect.Width * ImageScale), (int)(Rect.Height * ImageScale)), null, Color, 0, new Vector2(ButtonImage.Bounds.Width / 2, ButtonImage.Bounds.Height / 2), SpriteEffects.None, 0);

            //spriteBatch.Draw(Texture, InteractRect, null, new Color(0, 0, 255, 10), 0, Vector2.Zero, TextureFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void Update(GameTime gameTime)
        {
            InteractRect = GetScaledRect(GetInteraction(Scale).Normal);
        }
    }
}
