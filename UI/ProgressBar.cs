using System;

namespace Navy.UI
{
    public class ProgressBar : CanvasElement
    {
        public ProgressBar(Rectangle RawRect, Texture2D backgroundTexture, Texture2D fillTexture = null)
        {
            Texture = backgroundTexture;
            FillTexture = fillTexture ?? backgroundTexture;
            this.RawRect = RawRect;
        }

        public float Max { get; set; } = 1;

        public Texture2D FillTexture { get; set; }
        public float Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = Math.Clamp(value, 0, Max);
            }

        }

        private float value = 0;

        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect, null, BackgroundColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(FillTexture, new Rectangle(Rect.X, Rect.Y, (int)(Rect.Width * (Value / Max)), (int)(Rect.Height)), Color);
        }
    }
}
