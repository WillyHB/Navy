using System.Collections.Generic;
using System;

namespace Navy.UI
{
    public class Carousel : CanvasElement
    {
        public Carousel(Rectangle RawRect, Texture2D backgroundTex, SpriteFont Font = null)
        {
            Texture = backgroundTex;

            this.Font = Font;
            this.RawRect = RawRect;
        }

        private Button arrowRight, arrowLeft;


        protected override void OnInitialize()
        {
            arrowRight = new Button(new Rectangle(RawRect.X + (LocalOffset.X == RawRect.Width ? 0 : (LocalOffset.X == RawRect.Width / 2 ? (RawRect.Width - Rect.Height) / 2 : (RawRect.Width - Rect.Height))), RawRect.Y, RawRect.Height, RawRect.Height), Texture)
            {
                LocalOrigin = LocalOrigin,
                GlobalOrigin = GlobalOrigin,
                TextureFlipped = true,
                Parent = this,
            };

            arrowRight.AddInteraction(() => arrowRight.Scale, (v) => arrowRight.Scale = v, GetInteraction(Scale));
            arrowRight.AddInteraction(()=> arrowRight.Color, (v) => arrowRight.Color = v, GetInteraction(Color));
            arrowRight.AddInteraction(()=> arrowRight.BackgroundColor, (v) => arrowRight.BackgroundColor = v, GetInteraction(BackgroundColor));

            arrowLeft = new Button(new Rectangle(RawRect.X - (int)LocalOffset.X + (LocalOffset.X == RawRect.Width ? Rect.Height : (LocalOffset.X == RawRect.Width / 2 ? Rect.Height / 2 : 0)), RawRect.Y, RawRect.Height, RawRect.Height), Texture)
            {
                GlobalOrigin = GlobalOrigin,
                LocalOrigin = LocalOrigin,
                Parent = this,
            };

            arrowLeft.AddInteraction(() => arrowLeft.Scale, (v) => arrowLeft.Scale = v, GetInteraction(Scale));
            arrowLeft.AddInteraction(() => arrowLeft.Color, (v) => arrowLeft.Color = v, GetInteraction(Color));
            arrowLeft.AddInteraction(() => arrowLeft.BackgroundColor, (v) => arrowLeft.BackgroundColor = v, GetInteraction(BackgroundColor));
      

            arrowRight.Released += (sender, e) =>
            {
                if (Values.Count - 1 > CurrentIndex)
                {
                    SetIndex(++CurrentIndex);
                }
            };

            arrowLeft.Released += (sender, e) =>
            {
                if (CurrentIndex > 0)
                {
                    SetIndex(--CurrentIndex);
                }
            };
        }

        protected override void OnDestroy()
        {
            OnValueChanged = null;
        }

        public void AddValue(int index, string value)
        {
            Values.Insert(index, value);
        }

        public void AddValue(string value)
        {
            Values.Add(value);
        }

        public void RemoveValue(int index)
        {
            if (Values.Count - 1 < index)
            {
                Values.RemoveAt(index);
            }
        }



        public event EventHandler<int> OnValueChanged;
        private List<string> Values { get; set; } = new List<string>();

        public int CurrentIndex { get; private set; }
        public void SetIndex(int index, bool callEvent = true)
        {
            if (Values.Count > index)
            {
                CurrentIndex = index;
                CurrentValue = Values[index];
            }

            if (callEvent) OnValueChanged?.Invoke(this, CurrentIndex);
        }
        public string CurrentValue { get; private set; }
        public Vector2 TextPadding { get; set; } = Vector2.Zero;
        public float TextScale { get; set; } = 1;

        public string Text { get; set; }
        private Vector2 GetTextSize()
        {
            return Font.MeasureString(Text) * TextScale;
        }

        private SpriteFont Font { get => font ?? Globals.DefaultFont; set { font = value; } }

        private SpriteFont font;

        private Color backgroundColors = Color.White;
        private Vector2 scales = Vector2.One;

        public override Vector2 Scale
        {
            get => scales;

            set
            {
                scales = value;

                if (arrowRight != null)
                {
                    arrowLeft.Scale = value;
                    arrowRight.Scale = value;
                }
            }
        }


        public override Color BackgroundColor
        {
            get => backgroundColors;

            set
            {
                backgroundColors = value;

                if (arrowRight != null)
                {
                    arrowRight.Color = value;
                    arrowLeft.Color = value;
                }
            }
        }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            float scale = (GetTextSize().X > (RawRect.Width - TextPadding.X * 2 - Rect.Height * 2) ? (RawRect.Width - TextPadding.X * 2 - Rect.Height * 2) / GetTextSize().X : 1);

            spriteBatch.DrawString(Font, Text,
            new Vector2(GetScaledRect(Vector2.One).X + GetScaledRect(Vector2.One).Width / 2 * TextScale, GetScaledRect(Vector2.One).Y + Rect.Height / 2 + LocalOffset.Y / 2), Color, 0, new Vector2(GetTextSize().X / 2, GetTextSize().Y / 2), TextScale * scale, SpriteEffects.None, 0);

        }

        public override void Update(GameTime gameTime)
        {
            Text = Values[CurrentIndex] ?? "Default";
        }
    }
}
