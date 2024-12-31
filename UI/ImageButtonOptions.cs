using System.Collections.Generic;
using System;
using Navy;


namespace Navy.UI
{
    public class ImageButtonOptions<T> : CanvasElement
    {
        public ImageButtonOptions(Vector2 position, Vector2 buttonSize, Vector2 padding, Texture2D backgroundTex)
        {
            RawRect = new Rectangle(position.ToPoint(), Point.Zero);

            Texture = backgroundTex;

            this.padding = padding;
            this.buttonSize = buttonSize;
        }

        protected override void OnDestroy()
        {
            ValueChanged = null;
        }

        public T Value { get; private set; }

        public Color ImageColor { get; set; } = Color.White;


        private readonly Vector2 buttonSize;

        private readonly Vector2 padding;

        private ImageButton activeButton;

        public event EventHandler<T> ValueChanged;

        public void Select(T value)
        {
            if (buttons.ContainsValue(value))
            {

                ValueChanged?.Invoke(this, value);

                if (activeButton != null)
                {
                    activeButton.Color = ImageColor;
                }

                activeButton = buttons.KeyByValue(value);

                activeButton.Color = GetInteraction(ImageColor).Active;
                Value = value;

                return;
            }

            throw new NotImplementedException($"The value {value} does not exist within the ImageButtonOptions");

        }

        private Dictionary<ImageButton, T> buttons = new Dictionary<ImageButton, T>();
        public int ElementCount => buttons.Count;

        public void AddButton(T value, Texture2D buttonImage, float imageScale = 1, bool setValue = false)
        {
            RawRect = new Rectangle(RawRect.Location, new Vector2((buttons.Count + 1) * buttonSize.X + (buttons.Count + 2) * padding.X, buttonSize.Y + padding.Y * 2).ToPoint());

            ImageButton button = new ImageButton(new Rectangle((int)(buttons.Count * buttonSize.X + buttonSize.X / 2 + (buttons.Count + 1) * padding.X), 0, (int)buttonSize.X, (int)buttonSize.Y), Texture, buttonImage)
            {
                ImageScale = imageScale,
                GlobalOrigin = Origin.LeftMid,
                LocalOrigin = Origin.Center,
                Color = ImageColor,
                BackgroundColor = Color,
                Scale = Scale,
                Parent = this,
            };


            buttons.Add(button, value);


            T _value = value;
            button.Pressed += (object sender, MouseButton button) => { if (button == MouseButton.Left) Select(_value); };

            if (setValue) Select(_value);
        }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, GetScaledRect(Vector2.One), BackgroundColor);
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
