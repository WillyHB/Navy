using System;

namespace Navy.UI
{
    public class Slider : CanvasElement
    {
        public Slider(Rectangle RawRect, Texture2D backgroundTexture, Texture2D fillTexture = null, Texture2D handleTexture = null)
        {
            Texture = backgroundTexture;
            FillTexture = fillTexture ?? backgroundTexture;
            HandleTexture = handleTexture ?? backgroundTexture;
            this.RawRect = RawRect;
        }

        protected override void OnDestroy()
        {
            OnValueChanged = null;
        }

        public Color FillColor { get; set; } = Color.Transparent;

        public Texture2D HandleTexture { get; set; }
        public Texture2D FillTexture { get; set; }

        public Vector2 HandleSize { get; set; }

        private float oldValue;
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

        public float Max { get; set; } = 1;

        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect, null, BackgroundColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(FillTexture, new Rectangle(Rect.X, Rect.Y, (int)(Rect.Width * (Value / Max)), Rect.Height), FillColor);

            spriteBatch.Draw(HandleTexture, new Rectangle((int)(Rect.X + (Rect.Width - HandleSize.X) * ((float)Value / (float)Max)), (int)(Rect.Y - HandleSize.Y * Scale.Y / 2 + Rect.Height / 2), (int)(HandleSize.X * Scale.X), (int)(HandleSize.Y * Scale.Y)), Color);
        }

        public override void Update(GameTime gameTime)
        {
            if (State ==   ElementState.Active)
            {
                Value = Math.Clamp((InputManager.Mouse.MouseScreenPosition.X - HandleSize.X / 2 - RawRect.X - (Parent == null ? ParentCanvas.Rect.X : Parent.Rect.X) + LocalOffset.X * Scale.X - GlobalOffset.X + ParentCanvas.Scrolled.X) / (Rect.Width - HandleSize.X), 0, 1) * Max;

                if (Value != oldValue)
                {
                    OnValueChanged?.Invoke(this, Value);
                }

                oldValue = Value;
            }

            InteractRect = new Rectangle((int)(GetScaledRect(Scale).X), (int)(Rect.Y - HandleSize.Y * Scale.Y / 2 + Rect.Height / 2), (int)(GetScaledRect(Scale).Width), (int)(HandleSize.Y));
        }

        public event EventHandler<float> OnValueChanged;
    }

}
