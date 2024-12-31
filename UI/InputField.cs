using System.Collections.Generic;
using System;

namespace Navy.UI
{
    public class InputField : CanvasElement
    {
        public InputField(Rectangle rect, Texture2D backgroundTex, Texture2D cursorTexture = null, SpriteFont font = null)
        {
            this.font = font ?? Globals.DefaultFont;
            Texture = backgroundTex;
            this.RawRect = rect;

            Color = Color.White;
            BackgroundColor = Color.Black;
        }

        protected override void OnInitialize()
        {
            CursorTexture ??= Globals.EmptyTexture;
            MaskRect = Rect;

            InputManager.Keyboard.TextInput += TextInputHandler;
            InputManager.Mouse.ButtonPressed += MousePressed;
            InputManager.Keyboard.KeyPressed += KeyPressed;

            Deselected += (s, e) =>
            {
                cursorOffset = 0;
                selectionPosition = -1;
                scrolled = 0;
            };

            Pressed += (s, button) =>
            {
                if (button == MouseButton.Left)
                {
                    float mouseX = InputManager.Mouse.MouseScreenPosition.X;

                    for (int i = Text.Length; i >= 0; i--)
                    {
                        if (font.MeasureString(Text[..i]).X * textScale - font.MeasureString(Text[..scrolled]).X * textScale + Rect.X < mouseX)
                        {
                            cursorOffset = Text.Length - i;
                            break;
                        }

                    }
                }
            };
        }

        protected override void OnDestroy()
        {
            InputManager.Mouse.ButtonPressed -= MousePressed;
            InputManager.Keyboard.KeyPressed -= KeyPressed;
            InputManager.Keyboard.TextInput -= TextInputHandler;
        }

        public string Filter { init { foreach (char c in value) filter.Add(c); } }
        private readonly List<char> filter = new();
        public Texture2D CursorTexture { get; set; }
        public string Text { get; set; } = "";
        private string previousText;

        private float animationTime = 0;
        public int MaxLength { get; set; }
        public int CursorWidth { get; set; } = 3;
        public int CursorFlashingSpeed { get; set; } = 120;
        public bool NumericOnly { get; set; }
        private SpriteFont font;
        private int cursorOffset;
        private int scrolled;
        public bool IsSelected
        {
            get => isSelected;

            set
            {
                if (isSelected != value)
                {
                    if (value)
                    {
                        Selected?.Invoke(this, EventArgs.Empty);
                    }

                    else
                    {
                        Deselected?.Invoke(this, EventArgs.Empty);
                    }
                }
                isSelected = value;
            }
        }
        private bool isSelected;

        private int selectionPosition = -1;
        public Point TextPadding { get; set; } = Point.Zero;
        private float textScale;

        public event EventHandler Deselected;
        public event EventHandler Selected;
        public event EventHandler Modified;

        public bool TextureFlipped { get; set; }

        public void AddChar(char c) => TextInputHandler(this, new TextInputEventArgs(c));

        private void KeyPressed(object sender, Keys key)
        {

            if (key == Keys.Left || key == Keys.Right)
            {
                if (InputManager.Keyboard.IsKeyDown(Keys.LeftShift))
                {
                    if (selectionPosition == -1)
                    {
                        selectionPosition = (key == Keys.Left ? (Text.Length - cursorOffset - (cursorOffset > 0 ? 1 : 0)) : (Text.Length - cursorOffset + (cursorOffset < Text.Length ? 1 : 0)));
                    }

                    else
                    {
                        selectionPosition += (key == Keys.Left ? (selectionPosition > 0 ? -1 : 0) : (selectionPosition < Text.Length ? 1 : 0));
                    }
                }

                else
                {
                    selectionPosition = -1;

                    cursorOffset += (key == Keys.Left ? (cursorOffset < Text.Length ? 1 : 0) : (cursorOffset > 0 ? -1 : 0));
                }
            }

            else if (key == Keys.Escape || key == Keys.Enter) IsSelected = false;

            if (InputManager.Keyboard.IsKeyDown(Keys.LeftControl))
            {
                if (key == Keys.C)
                {
                    if (selectionPosition >= 0)
                    {
                        System.Windows.Forms.Clipboard.SetText(Text[Math.Min(selectionPosition, Text.Length - cursorOffset)..Math.Max(selectionPosition, Text.Length - cursorOffset)]);
                    }
                }

                else if (key == Keys.V)
                {
                    string clipboardTxt = System.Windows.Forms.Clipboard.GetText();

                    foreach (char c in clipboardTxt)
                    {
                        if (filter.Contains(c))
                        {
                            return;
                        }
                    }

                    if (Text.Length + clipboardTxt.Length > MaxLength)
                    {
                        return;
                    }

                    int cursorPos = Text.Length - cursorOffset;

                    if (selectionPosition >= 0)
                    {
                        string start = Text[..Math.Min(selectionPosition, cursorPos)];
                        string end = Text[(Math.Max(selectionPosition, cursorPos))..];
                        Text = start + clipboardTxt + end;

                        if (cursorPos < selectionPosition)
                        {
                            cursorOffset -= Math.Abs(selectionPosition - cursorPos);
                        }
                        selectionPosition = -1;
                    }

                    else if (Text.Length < MaxLength || MaxLength <= 0)
                    {
                        Text = Text.Insert(cursorPos, clipboardTxt);
                    }
                }

                else if (key == Keys.X)
                {
                    if (selectionPosition >= 0)
                    {
                        System.Windows.Forms.Clipboard.SetText(Text[Math.Min(selectionPosition, Text.Length - cursorOffset)..Math.Max(selectionPosition, Text.Length - cursorOffset)]);

                        int cursorPos = Text.Length - cursorOffset;

                        string start = Text[..Math.Min(selectionPosition, cursorPos)];
                        string end = Text[(Math.Max(selectionPosition, cursorPos))..];
                        Text = start + end;

                        if (cursorPos < selectionPosition)
                        {
                            cursorOffset -= Math.Abs(selectionPosition - cursorPos);
                        }
                        selectionPosition = -1;
                    }
                }
            }
        }

        private void MousePressed(object sender, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                IsSelected = Rect.Contains(InputManager.Mouse.MouseScreenPosition);

                selectionPosition = -1;
            }
        }

        private void TextInputHandler(object sender, TextInputEventArgs args)
        {
            if (!IsSelected)
            {
                return;
            }

            char c = args.Character;

            if (filter.Contains(c))
            {
                return;
            }

            if (NumericOnly && (char.GetNumericValue(c) < 0 || char.GetNumericValue(c) > 9))
            {
                if (c != '\b')
                {
                    return;
                }
            }

            if (c == '\b' || args.Key == Keys.Delete)
            {
                if (Text.Length - cursorOffset - 1 >= 0 || (args.Key == Keys.Delete && Text.Length > 0))
                {
                    if (selectionPosition >= 0)
                    {
                        int cursorPos = Text.Length - cursorOffset;

                        string start = Text[..Math.Min(selectionPosition, cursorPos)];
                        string end = Text[(Math.Max(selectionPosition, cursorPos))..];
                        Text = start + end;

                        if (cursorPos < selectionPosition)
                        {
                            cursorOffset -= Math.Abs(selectionPosition - cursorPos);
                        }
                        selectionPosition = -1;
                    }

                    else
                    {
                        if (args.Key == Keys.Delete && cursorOffset > 0) cursorOffset--;
                        Text = Text.Remove(Text.Length - cursorOffset - 1, 1);
                    }
                }
            }

            /*
             FIX CAN'T CHANGE TAB WHILE WRITING IN INPUT FIELD
             INPUT FIELD TEXT PADDING!
             CAN'T SEE ERASER INPUT FIELD
            */
            else
            {
                if (font.Characters.Contains(c))
                {
                    int cursorPos = Text.Length - cursorOffset;

                    if (selectionPosition >= 0)
                    {
                        string start = Text[..Math.Min(selectionPosition, cursorPos)];
                        string end = Text[(Math.Max(selectionPosition, cursorPos))..];
                        Text = start + c + end;

                        if (cursorPos < selectionPosition)
                        {
                            cursorOffset -= Math.Abs(selectionPosition - cursorPos);
                        }
                        selectionPosition = -1;
                    }

                    else if (Text.Length < MaxLength || MaxLength <= 0)
                    {
                        Text = Text.Insert(cursorPos, c.ToString());
                    }
                }
            }
        }

        private bool IsFlashingCursorVisible()
        {
            int time = (int)animationTime % 120;

            return time > 0 && time <= 60;
        }

        public override void Update(GameTime gameTime)
        {
            if (previousText != Text)
            {
                previousText = Text;
                Modified?.Invoke(this, EventArgs.Empty);
            }
            textScale = (Rect.Height - TextPadding.Y) / font.MeasureString(Text).Y;

            animationTime += (CursorFlashingSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            InteractRect = GetScaledRect(GetInteraction(Scale).Normal);
            
            if (InputManager.Mouse.IsButtonDown(MouseButton.Left))
            {
                if (IsSelected)
                {
                    if (selectionPosition == -1)
                    {
                        selectionPosition = Text.Length - cursorOffset;
                    }
                    float mouseX = InputManager.Mouse.MouseScreenPosition.X;

                    for (int i = Text.Length; i >= 0; i--)
                    {
                        if (font.MeasureString(Text[..i]).X * textScale - font.MeasureString(Text[..scrolled]).X * textScale + Rect.X + TextPadding.X / 2 < mouseX)
                        {
                            int selectedOffset = Text.Length - i;

                            if (MathF.Abs(selectedOffset - cursorOffset) > 0)
                            {
                                cursorOffset = Text.Length - i;
                            }
                            break;
                        }
                    }
                }       
            }
        }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            int cursorPos = Text.Length - cursorOffset;

            spriteBatch.Draw(Texture, Rect, null, BackgroundColor, 0, Vector2.Zero, TextureFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            if (selectionPosition >= 0)
            {

                spriteBatch.Draw(
                    Globals.EmptyTexture,
                    new Rectangle(
                        (int)(Rect.X + font.MeasureString(Text[..Math.Min(selectionPosition, cursorPos)]).X * textScale - font.MeasureString(Text[..scrolled]).X * textScale), Rect.Y,
                        (int)(font.MeasureString(Text[Math.Min(selectionPosition, cursorPos)..Math.Max(selectionPosition, cursorPos)]).X * textScale), Rect.Height),
                    new Color(20, 40, 255, 100));
            }

            while ((font.MeasureString(Text[..^(cursorOffset)]).X * textScale) - font.MeasureString(Text[..scrolled]).X * textScale > Rect.Width - TextPadding.X)
            {
                scrolled++;
            }

            if (cursorPos < scrolled)
            {
                scrolled = cursorPos;
            }

            if (!string.IsNullOrEmpty(Text))
            {
                spriteBatch.DrawString(font, Text, new Vector2(Rect.X - font.MeasureString(Text[..scrolled]).X * textScale + TextPadding.X / 2, Rect.Y + TextPadding.Y / 2), Color, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

            }


            if (IsSelected && IsFlashingCursorVisible())
            {
                
                float cursorX = font.MeasureString(Text[..^(cursorOffset)]).X * textScale + 1;

                spriteBatch.Draw(CursorTexture, new Rectangle((int)(cursorX + Rect.X + TextPadding.X / 2 - font.MeasureString(Text[..scrolled]).X * textScale), (int)(Rect.Y), CursorWidth, Rect.Height), Color);
            }
        }

    }
}
