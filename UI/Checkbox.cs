using System;

namespace Navy.UI
{
    public class Checkbox : CanvasElement
    {
        public Checkbox(Rectangle RawRect, Texture2D backgroundTex, Texture2D checkTexture = null)
        {
            CheckTexture = checkTexture ?? backgroundTex;
            Texture = backgroundTex;
            this.RawRect = RawRect;

            Color = Color.White;
            BackgroundColor = Color.Black;
        }

        public Color InactiveColor { get; set; } = Color.Black;
        protected override void OnInitialize()
        {
            Released += Clicked;
        }

        protected override void OnDestroy()
        {
            ValueChanged = null;
        }

        public float CheckScale { get; set; } = 0.5f;
        public Texture2D CheckTexture { get; set; }

        private bool value = true;
        public bool Value
        {
            get => value;
            set
            {
                this.value = value;
                ValueChanged?.Invoke(this, value);
            }
        }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, GetScaledRect(Vector2.One), null, BackgroundColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(CheckTexture, new Rectangle((int)(GetScaledRect(Vector2.One).X + GetScaledRect(Vector2.One).Width / 2 - Rect.Width / 2 * CheckScale), (int)(GetScaledRect(Vector2.One).Y + GetScaledRect(Vector2.One).Height / 2 - Rect.Height / 2 * CheckScale), (int)(Rect.Width * CheckScale), (int)(Rect.Height * CheckScale))
                      , null, Value ? Color : InactiveColor, 0, Vector2.Zero, SpriteEffects.None, 0);


        }

        public event EventHandler<bool> ValueChanged;

        public override void Update(GameTime gameTime)
        {
            InteractRect = GetScaledRect(Vector2.One);
        }

        public void Clicked(object sender, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                Value = !Value;
            }
        }
    }
}
