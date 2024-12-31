using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Navy.UI
{
    public class Button : CanvasElement
    {
        public Button(Rectangle RawRect, Texture2D backgroundTex, SpriteFont Font = null)
        {
            Texture = backgroundTex;
            this.Font = Font ?? Globals.DefaultFont;
            this.RawRect = RawRect;
        }

        public Vector2 TextPadding { get; set; } = Vector2.Zero;
        public float TextScale { get; set; } = 1;
        public TextStyleType TextStyle { get; set; }
        public string Text { get; set; }

        public Origin TextOrigin { get; set; } = Origin.Center;
        private SpriteFont Font { get => font ?? Globals.DefaultFont; set { font = value; } }

        private SpriteFont font;


        public bool TextureFlipped { get; set; }

        public Vector2 GetTextSize()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                switch (TextStyle)
                {
                    case TextStyleType.None: return Font.MeasureString(Text) * TextScale;

                    case TextStyleType.Wrap: return Font.MeasureString(TextWrap.WrapText(Font, Text, Rect.Width));

                    case TextStyleType.Fit: return Font.MeasureString(Text) * TextScale;
                }
            }

            return Vector2.Zero;
        }


        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect, null, BackgroundColor, 0, Vector2.Zero, TextureFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            if (!string.IsNullOrEmpty(Text))
            {
                if (TextStyle == TextStyleType.Wrap)
                {
                    Vector2 textOffset = Canvas.GetTextOriginOffset(Rect, TextOrigin, GetTextSize(), TextPadding, TextScale * (Scale.X > Scale.Y ? Scale.Y : Scale.X));

                    spriteBatch.DrawString(Font, TextWrap.WrapText(Font, Text, Rect.Width / TextScale - TextPadding.X),
                    new Vector2(Rect.X + textOffset.X, Rect.Y + textOffset.Y), Color, 0, new Vector2(GetTextSize().X / 2, GetTextSize().Y / 2), TextScale * (Scale.X > Scale.Y ? Scale.Y : Scale.X), SpriteEffects.None, 0);
                }

                else if (TextStyle == TextStyleType.None)
                {
                    Vector2 textOffset = Canvas.GetTextOriginOffset(Rect, TextOrigin, GetTextSize(), TextPadding, TextScale * (Scale.X > Scale.Y ? Scale.Y : Scale.X));

                    spriteBatch.DrawString(Font, Text,
                    new Vector2(Rect.X + textOffset.X, Rect.Y + textOffset.Y), Color, 0, new Vector2(GetTextSize().X / 2, GetTextSize().Y / 2), TextScale * (Scale.X > Scale.Y ? Scale.Y : Scale.X), SpriteEffects.None, 0);
                }

                else if (TextStyle == TextStyleType.Fit)
                {
                    float scale = 1;
                    if (GetTextSize().X * GetInteraction(Scale).Normal.X > Rect.Width - TextPadding.X * 2)
                    {
                        scale = (Rect.Width - TextPadding.X * 2) / GetTextSize().X;

                        if (GetTextSize().Y * scale > (Rect.Height - TextPadding.Y * 2))
                        {
                            scale = (Rect.Height - TextPadding.X * 2) / GetTextSize().Y;
                        }
                    }

                    else if (GetTextSize().Y  * GetInteraction(Scale).Normal.Y > Rect.Height - TextPadding.Y * 2)
                    {
                        scale = (Rect.Height - TextPadding.X * 2) / GetTextSize().Y;
                      

                        if (GetTextSize().X * scale > (Rect.Width - TextPadding.X * 2))
                        {
                            scale = (Rect.Width - TextPadding.X * 2) / GetTextSize().X;
                        }
                    }

                    Vector2 textOffset = Canvas.GetTextOriginOffset(Rect, TextOrigin, GetTextSize(), TextPadding, scale * TextScale * (Scale.X > Scale.Y ? Scale.Y : Scale.X));

                    spriteBatch.DrawString(Font, Text,
                    new Vector2(Rect.X + textOffset.X, Rect.Y + textOffset.Y),
                    Color, 0, new Vector2(GetTextSize().X / 2, GetTextSize().Y / 2), TextScale * scale, SpriteEffects.None, 0);
                }
            }

            //spriteBatch.Draw(Texture, InteractRect, null, new Color(0, 0, 255, 10), 0, Vector2.Zero, TextureFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void Update(GameTime gameTime)
        {
            InteractRect = GetScaledRect(GetInteraction(Scale).Normal);
        }
    }
}
