namespace Navy.UI
{
    public class Tooltip : CanvasElement
    {
        public Tooltip(Vector2 size, Texture2D backgroundTex, SpriteFont Font = null)
        {
            Texture = backgroundTex;
            this.Font = Font ?? Globals.DefaultFont;
            this.RawRect = new Rectangle(0, 0, (int)size.X, (int)size.Y);
        }

        public Tooltip(Rectangle RawRect, Texture2D backgroundTex, SpriteFont Font = null)
        {
            Texture = backgroundTex;
            this.Font = Font;
            this.RawRect = RawRect;
        }

        public TooltipLocation TooltipLocation { get; set; } = TooltipLocation.CornerOfMouseAuto;

        public Vector2 TextPadding { get; set; } = Vector2.Zero;
        public float TextScale { get; set; } = 1;

        private string text;
        private TextStyleType textStyle;
        public CanvasElement Element { get; set; }

        public TextStyleType TextStyle
        {
            get { return textStyle; }

            set
            {
                textStyle = value;

                if (!string.IsNullOrEmpty(Text))
                {
                    switch (value)
                    {
                        case TextStyleType.None:
                            TextSize = Font.MeasureString(Text) * TextScale;
                            break;

                        case TextStyleType.Wrap:
                            TextSize = Font.MeasureString(Navy.TextWrap.WrapText(Font, Text, Rect.Width));
                            break;

                        case TextStyleType.Fit:
                            TextSize = Font.MeasureString(Text) * TextScale;
                            break;
                    }
                }
            }
        }
        public string Text
        {
            get { return text; }

            set
            {

                switch (textStyle)
                {
                    case TextStyleType.None:
                        TextSize = Font.MeasureString(value) * TextScale;
                        break;

                    case TextStyleType.Wrap:
                        TextSize = Font.MeasureString(Navy.TextWrap.WrapText(Font, value, Rect.Width));
                        break;

                    case TextStyleType.Fit:
                        TextSize = Font.MeasureString(value) * TextScale;
                        break;
                }

                text = value;
            }

        }

        private Vector2 TextSize { get; set; } = Vector2.One;

        public Origin TextOrigin { get; set; } = Origin.Center;
        private SpriteFont Font { get => font ?? Globals.DefaultFont; set { font = value; } }

        private SpriteFont font;

        protected override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle RawRect = Rectangle.Empty;

            if (Element != null)
            {
                if (Element.InteractRect.Contains(InputManager.Mouse.MouseScreenPosition))
                {
                    if (TooltipLocation == TooltipLocation.CornerOfMouseAuto)
                    {
                        RawRect = new Rectangle((int)(InputManager.Mouse.MouseScreenPosition.X), (int)(InputManager.Mouse.MouseScreenPosition.Y), (int)(Rect.Width), (int)(Rect.Height));
                    }

                    else if (TooltipLocation == TooltipLocation.Screen)
                    {
                        RawRect = Rect;
                    }
                }
            }

            spriteBatch.Draw(Texture, new Rectangle((int)(RawRect.X), (int)(RawRect.Y), (int)(RawRect.Width), (int)(RawRect.Height)), null, BackgroundColor, 0, Vector2.Zero, SpriteEffects.None, 0);

            if (!string.IsNullOrEmpty(Text))
            {
                if (TextStyle == TextStyleType.Wrap)
                {
                    Vector2 textOffset = Canvas.GetTextOriginOffset(RawRect, TextOrigin, TextSize, TextPadding, TextScale);

                    spriteBatch.DrawString(Font, TextWrap.WrapText(Font, Text, RawRect.Width / TextScale - TextPadding.X),
                    new Vector2(RawRect.X + textOffset.X * Scale.X, RawRect.Y + textOffset.Y * Scale.Y), Color, 0, new Vector2(TextSize.X / 2, TextSize.Y / 2), TextScale * (Scale.X > Scale.Y ? Scale.Y : Scale.X), SpriteEffects.None, 0);
                }

                else if (TextStyle == TextStyleType.None)
                {
                    Vector2 textOffset = Canvas.GetTextOriginOffset(RawRect, TextOrigin, TextSize, TextPadding, TextScale);

                    spriteBatch.DrawString(Font, Text,
                    new Vector2(RawRect.X + textOffset.X * Scale.X, RawRect.Y + textOffset.Y * Scale.Y), Color, 0, new Vector2(TextSize.X / 2, TextSize.Y / 2), TextScale * (Scale.X > Scale.Y ? Scale.Y : Scale.X), SpriteEffects.None, 0);
                }

                else if (TextStyle == TextStyleType.Fit)
                {
                    float scale = (TextSize.X > (RawRect.Width - TextPadding.X * 2) ? (RawRect.Width - TextPadding.X * 2) / TextSize.X : 1);
                    Vector2 textOffset = Canvas.GetTextOriginOffset(RawRect, TextOrigin, TextSize, TextPadding, scale * TextScale);

                    spriteBatch.DrawString(Font, Text,
                    new Vector2(RawRect.X + textOffset.X * Scale.X, RawRect.Y + textOffset.Y * Scale.Y),
                    Color, 0, new Vector2(TextSize.X / 2, TextSize.Y / 2), TextScale * (Scale.X > Scale.Y ? Scale.Y : Scale.X) * scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
