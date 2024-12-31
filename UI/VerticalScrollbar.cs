using System;

namespace Navy.UI
{
    public class VerticalScrollbar : CanvasElement
    {
        public VerticalScrollbar(Rectangle RawRect, Texture2D backgroundTexture, Texture2D fillTexture = null, Texture2D handleTexture = null)
        {
            Texture = backgroundTexture;
            HandleTexture = handleTexture ?? backgroundTexture;  
            this.RawRect = RawRect;
        }

        public Texture2D HandleTexture { get; set; }
        public Vector2 HandleSize { get; set; }

        private float value = 0;

        public float Max { get; set; } = 1;

        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(Rect.X + (int)ParentCanvas.Scrolled.X, Rect.Y + (int)ParentCanvas.Scrolled.Y, Rect.Width, Rect.Height), null, BackgroundColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(HandleTexture, new Rectangle((int)(Rect.X - HandleSize.X / 2 * Scale.X + RawRect.Width * Scale.X / 2 + ParentCanvas.Scrolled.X), (int)(Rect.Y + (Rect.Height - HandleSize.Y) * (value / Max) + ParentCanvas.Scrolled.Y), (int)(HandleSize.X * Scale.X), (int)(HandleSize.Y * Scale.Y)), Color);
        }

        public override void Update(GameTime gameTime)
        {
            if (ParentCanvas.Scrolled != new Vector2(0, (value / Max) * ParentCanvas.ScrollAmount.Y))
            {
                value = ParentCanvas.Scrolled.Y / ParentCanvas.ScrollAmount.Y;
            }

            else
            {
                if (State ==   ElementState.Active)
                {
                    value = Math.Clamp((InputManager.Mouse.MouseScreenPosition.Y - RawRect.Y - ParentCanvas.Rect.Y + LocalOffset.Y * Scale.Y - GlobalOffset.Y) / (Rect.Height - HandleSize.Y), 0, 1) * Max;
                    ParentCanvas.Scrolled = new Vector2(0, (value / Max) * ParentCanvas.ScrollAmount.Y);
                }
            }

            InteractRect = new Rectangle((int)(GetScaledRect(GetInteraction(Scale).Normal).X + ParentCanvas.Scrolled.X), (int)(GetScaledRect(GetInteraction(Scale).Normal).Y - HandleSize.Y / 2 + ParentCanvas.Scrolled.Y), (int)(GetScaledRect(GetInteraction(Scale).Normal).Width), (int)(GetScaledRect(GetInteraction(Scale).Normal).Height + HandleSize.Y));
        }
    }
}
