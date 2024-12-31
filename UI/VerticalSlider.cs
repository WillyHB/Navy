using System;

namespace Navy.UI
{
    public class VerticalSlider : CanvasElement
    {
        public VerticalSlider(Rectangle RawRect, Texture2D backgroundTexture, Texture2D fillTexture = null, Texture2D handleTexture = null)
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
        public float Value
        {
            get
            {
                return value;
            }

            set
            {
                OnValueChanged?.Invoke(this, value);
                this.value = Math.Clamp(value, 0, Max);
            }

        }

        private float value = 0;

        public float Max { get; set; } = 1;

        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect, null, BackgroundColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(FillTexture, new Rectangle(Rect.X, Rect.Y, Rect.Width, (int)(Rect.Height * (Value / Max))), FillColor);

            spriteBatch.Draw(HandleTexture, new Rectangle((int)(Rect.X - HandleSize.X * Scale.X / 2 + Rect.Width / 2), (int)((Rect.Y + (Rect.Height - HandleSize.Y) * ((float)Value / (float)Max))), (int)(HandleSize.X * Scale.X), (int)(HandleSize.Y * Scale.Y)), Color);
        }

        public override void Update(GameTime gameTime)
        {
            if (State ==   ElementState.Active)
            {
                Value = Math.Clamp((InputManager.Mouse.MouseScreenPosition.Y - RawRect.Y - (Parent == null ? ParentCanvas.Rect.Y : Parent.Rect.Y) + LocalOffset.Y * Scale.Y - GlobalOffset.Y) / (Rect.Height - HandleSize.Y), 0, 1) * Max;
            }

            InteractRect = new Rectangle((int)(GetScaledRect(GetInteraction(Scale).Normal).X + ParentCanvas.Scrolled.X), (int)(GetScaledRect(GetInteraction(Scale).Normal).Y - HandleSize.Y / 2 + ParentCanvas.Scrolled.Y), (int)(GetScaledRect(GetInteraction(Scale).Normal).Width), (int)(GetScaledRect(GetInteraction(Scale).Normal).Height + HandleSize.Y));
        }

        public event EventHandler<float> OnValueChanged;
    }

}
