using System.Collections.Generic;
using System.Linq;
using System;

namespace Navy.UI
{

    public class FromTo : EventArgs
    {
        public override string ToString()
        {
            return $"From: {from}, To: {to}";
        }
        public int from;
        public int to;
    }

    public class Listbox : CanvasElement
    {
        public enum ListboxSortMode
        {
            Deferred,
            Alphabetical,
            Free
        }

        protected ListboxSortMode sortMode;
        public ListboxSortMode SortMode
        {
            get => sortMode;

            set
            {
                sortMode = value;

                switch (value)
                {
                    case ListboxSortMode.Alphabetical:
                        elements = elements.OrderBy(x => x.Text).ToList();

                        for (int i = 0; i < elements.Count; i++)
                        {
                            elements[i].Position = new Vector2(0, i * elementHeight + elementPadding * i);
                        }
                        break;
                }
            }
        }

        public int ElementCount => elements.Count;

        protected float elementHeight;
        protected float elementPadding;

        public bool AlwaysSelected { get; set; }

        protected Vector2 textPadding;

        protected Texture2D elementTexture;

        public Origin TextOrigin { get; set; } = Origin.LeftMid;

        public Button SelectedElement { get; private set; }

        public int SelectedIndex
        {
            get => elements.IndexOf(SelectedElement);
        }

        public event EventHandler Modified;
        public event EventHandler<int> ElementSelected;

        public event EventHandler<MouseButton> ElementPressed;
        public event EventHandler<MouseButton> ElementReleased;

        public event EventHandler<FromTo> ElementMoved;
        public event EventHandler<int> ElementRenamed;

        public event EventHandler<int> ElementRemoved;
        public event EventHandler ElementAdded;

        protected void OnModified(object sender, EventArgs args)=> Modified?.Invoke(sender, args);
        protected void OnElementPressed(object sender, MouseButton button) => ElementPressed?.Invoke(sender, button);
        protected void OnElementReleased(object sender, MouseButton button)=> ElementReleased?.Invoke(sender, button);

        protected VerticalSlider scrollbar;

        protected InputField inputField;

        public Vector2 HandleSize
        {
            get => scrollbar.HandleSize;
            set => scrollbar.HandleSize = value;
        }

        protected List<Button> elements = new();

        public bool ElementsRenameable { get; set; } = false;
        public bool LockElement { get; set; } = false;

        public Color ScrollbarBackgroundColor
        {
            get => scrollbar.BackgroundColor;
            set => scrollbar.BackgroundColor = value;
        }

        public Color ScrollbarColor
        {          
            get => scrollbar.Color;
            set => scrollbar.Color = value;
        }

        public Color ButtonBackgroundColor
        {
            get => buttonBackgroundColor;

            set
            {
                foreach (var b in elements)
                {
                    b.BackgroundColor = value;
                }

                buttonBackgroundColor = value;
            }
        }

        private Color buttonBackgroundColor = Color.White;

        public Vector2 ButtonScale
        {
            get => buttonScale;

            set
            {
                foreach (var b in elements)
                {
                    b.Scale = value;
                }

                buttonScale = value;
            }
        }

        public void AddColorInteraction(Color normal, Color hover, Color active, float smoothing)
        {
            elements.ForEach((button) => button.AddInteraction(() => button.Color, v => button.Color = v, normal, hover, active, smoothing));
            ElementAdded += (s, e) =>
            {
                elements[^1].AddInteraction(() => elements[^1].Color, v => elements[^1].Color = v, normal, hover, active, smoothing);
            };
        }

        public void AddBackgroundColorInteraction(Color normal, Color hover, Color active, float smoothing)
        {
            
            ElementAdded += (s, e) =>
            {
                Button button = elements[^1];
                button.AddInteraction(() => button.BackgroundColor, v => button.BackgroundColor = v, normal, hover, active, smoothing);
            };
        }

        public void AddScaleInteraction(Vector2 normal, Vector2 hover, Vector2 active, float smoothing)
        {
            elements.ForEach((button) => button.AddInteraction(() => button.Scale, v => button.Scale = v, normal, hover, active, smoothing));
            ElementAdded += (s, e) =>
            {
                elements[^1].AddInteraction(() => elements[^1].Scale, v => elements[^1].Scale = v, normal, hover, active, smoothing);
            };
        }

        protected Vector2 buttonScale = Vector2.One;        
        
        public Color TextColor
        {
            get => textColor;

            set
            {
                foreach (var b in elements)
                {
                    b.Color = value;
                }

                 textColor = value;
            }
        }

        protected Color textColor = Color.Black;

        public float ElementMoveSmoothness { get; set; } = 0.1f;
        public Listbox(Rectangle RawRect, float elementHeight, float elementPadding, Vector2 textPadding, Texture2D backgroundTex, Texture2D elementTex = null)
        {
            this.RawRect = RawRect;
            this.elementHeight = elementHeight;
            this.elementPadding = elementPadding;
            this.textPadding = textPadding;
            Texture = backgroundTex;
            elementTexture = elementTex ?? backgroundTex;
        }
        protected override void OnInitialize()
        {
            MaskRect = Rect;

            scrollbar = new VerticalSlider(new Rectangle(-(int)(Rect.Width * 0.025f), 0, (int)(Rect.Width * 0.05f), RawRect.Height), Globals.EmptyTexture)
            {
                Color = Color.Black,
                FillColor = Color.Transparent,

                HandleSize = new Vector2(Rect.Width * 0.05f, Rect.Width * 0.075f),
                GlobalOrigin = Origin.TopRight,
                LocalOrigin = Origin.TopRight,
                Parent = this,
            };

            scrollbar.AddInteraction(() => scrollbar.BackgroundColor, (v) => scrollbar.BackgroundColor = v, Color.WhiteSmoke, Color.WhiteSmoke, Color.WhiteSmoke, 0);

            InputManager.Mouse.Scrolled += (sender, value) =>
            {
                if (HasMouseEntered)
                {
                    scrollbar.Value -= value / 2500f;
                }
            };

            scrollbar.OnValueChanged += (sender, value) =>
            {
                if (elements.Count > 0)
                {
                    int min = elements[0].RawRect.Y;
                    int max = (int)(elements.Count * elementHeight + elementPadding * elements.Count) - RawRect.Height;

                    if (max > 0)
                    {
                        for (int i = 0; i < elements.Count; i++)
                        {
                            elements[i].Position = new Vector2(0, i * elementHeight + elementPadding * i - max * value);
                        }
                    }

                    else if (min < 0)
                    {
                        if (value < scrollbar.Value)
                        {
                            for (int i = 0; i < elements.Count; i++)
                            {
                                elements[i].Position = new Vector2(0, i * elementHeight + elementPadding * i - (elementPadding - min) * value);
                            }
                        }
                    }
                }

            };

            InputManager.Keyboard.KeyPressed += KeyPressed;

        }

        public void Select(int button)
        {
            if (SelectedElement != null)
            {
                //if (inputField != null) inputField.Select(false);

                if (LockElement)
                {
                    SelectedElement.UnlockCurrentState();
                }
            }

            if (button >= 0 && button < elements.Count)
            {
                SelectedElement = elements[button];

                elements[button].SwitchState(ElementState.Active);

                if (LockElement)
                {
                    elements[button].LockCurrentState();
                }

                ElementSelected?.Invoke(this, button);
            }

            else
            {
                SelectedElement = null;
            }
        }

        public void SetText(int i, string text)
        {
            elements[i].Text = text;
            ElementRenamed?.Invoke(this, i);
        }

            public string[] GetElements()
        {
            List<string> text = new();

            elements.ForEach((button) => text.Add(button.Text));

            return text.ToArray();
        }

        public string GetElement(int i) => elements[i].Text;


        protected void KeyPressed(object sender, Keys key)
        {
            if (key == Keys.F2)
            {
                if (ElementsRenameable)
                {
                    if (inputField == null && SelectedElement != null)
                    {
                        inputField = new InputField(new Rectangle(0, 0, SelectedElement.RawRect.Width, SelectedElement.RawRect.Height), Globals.EmptyTexture)
                        {
                            Text = SelectedElement.Text,
                            Parent = SelectedElement
                        };

                        inputField.BackgroundColor = GetInteraction(ButtonBackgroundColor).Normal;
                        inputField.Color = GetInteraction(TextColor).Normal;

                        //inputField.Select(true);

                        inputField.Deselected += (s, e) =>
                        {

                            SelectedElement.Text = inputField.Text;
                            inputField.Destroy();
                            inputField = null;
                            ElementRenamed?.Invoke(this, SelectedIndex);
                            Modified?.Invoke(this, EventArgs.Empty);
                        };
                    }
                }
               
            }

            else if (key == Keys.Delete)
            {
                if (SelectedElement != null)
                {
                    RemoveElement(SelectedElement);
                }
            }
        }

        public void RemoveElement(string text)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Text == text)
                {
                    RemoveElement(i);
                    return;
                }
            }
        }

        public void RemoveElement(Button element)
        {
            RemoveElement(elements.IndexOf(element));
        }

        public void RemoveElement(int index)
        {
            if (index < ElementCount)
            {
                elements[index].Destroy();
                elements.RemoveAt(index);             
            }

            for (int i = index; i < elements.Count; i++)
            {
                int j = i;
                Tweener.TweenValue(() => elements[j].Position, (v) => elements[j].Position = v, elements[j].Position, new Vector2(elements[j].Position.X, elements[j].Position.Y - elementHeight - elementPadding), ElementMoveSmoothness);
                //elements[i].Position = new Vector2(elements[i].Position.X, elements[i].Position.Y - elementHeight - elementPadding);
            }

            ElementRemoved?.Invoke(this, index);
            Modified?.Invoke(this, EventArgs.Empty);
        }

        public virtual Button AddElement(string text)
        {
            Button button = new Button(new Rectangle(0, (int)(elements.Count * elementHeight + elementPadding * elements.Count), (int)(Rect.Width - Rect.Width * 0.1f), (int)elementHeight), Globals.EmptyTexture)
            {
                Text = text,
                Texture = elementTexture,
                TextPadding = textPadding,
                TextStyle = TextStyleType.Fit,
                TextOrigin = TextOrigin,
                BlockMouseInput = false,
                ActivityStyle = ActivityStyle.ToggleRelease,
                Parent = this,
                Color = Color.Blue,
            };

            button.Pressed += (sender, e) =>
            {
                if (e == MouseButton.Left)
                {

                    Select(elements.IndexOf(button));
                }
            };

         
            button.Pressed += ElementPressed;
            button.Released += ElementReleased;


            elements.Add(button);

            if (sortMode == ListboxSortMode.Alphabetical)
            {
                elements = elements.OrderBy(x => x.Text).ToList();

                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].Position = new Vector2(0, i * elementHeight + elementPadding * i);
                }
            }

            ElementAdded?.Invoke(this, EventArgs.Empty);
            Modified?.Invoke(this, EventArgs.Empty);

            return button;
        }

        public Button[] AddElements(params string[] text)
        {
            List<Button> buttons = new();
            foreach (string str in text)
            {
                buttons.Add(AddElement(str));
            }

            return buttons.ToArray();
        }

        protected override void OnDestroy()
        {
            InputManager.Mouse.Scrolled -= (sender, value) =>
            {
                if (HasMouseEntered)
                {
                    scrollbar.Value -= value / 2500f;
                }
            };

            InputManager.Keyboard.KeyPressed -= KeyPressed;

            ElementPressed = null;
            ElementReleased = null;
            ElementSelected = null;
            Modified = null;
            ElementMoved = null;
            ElementRemoved = null;
            ElementAdded = null;
            ElementRenamed = null;


        }

        protected override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect, BackgroundColor);
        }

        protected float offset;
        protected bool moving;

        public override void Update(GameTime gameTime)
        {
            if (SortMode == ListboxSortMode.Free)
            {
                if (SelectedElement == null)
                {
                    return;
                }

                if (moving && !InputManager.Mouse.IsButtonDown(MouseButton.Left))
                {
                    moving = false;

                    float selecedElementPosY = SelectedElement.Position.Y + ((elements.Count * elementHeight + elementPadding * elements.Count) - RawRect.Height) * scrollbar.Value;

                    float y = selecedElementPosY % ((elementPadding + elementHeight));

                    float listLength = (elementHeight * (elements.Count - 1) + elementPadding * (elements.Count - 1));

                    float elementPos;

                    if (y / (elementPadding + elementHeight) < 0.5f)
                    {
                        elementPos = selecedElementPosY - y <= listLength ? (selecedElementPosY - y >= 0 ? selecedElementPosY - y : 0) : listLength;
                    }

                    else
                    {
                        elementPos = selecedElementPosY + ((elementPadding + elementHeight) - y) <= listLength ? selecedElementPosY + ((elementPadding + elementHeight) - y) : listLength;
                    }

                    int i = (int)((elementPos / listLength) * (elements.Count - 1));

                    i = i < 0 ? 0 : i;

                    elements[i].Position = new Vector2(0, elementHeight * SelectedIndex + elementPadding * SelectedIndex - ((elements.Count * elementHeight + elementPadding * elements.Count) - RawRect.Height) * scrollbar.Value);

                    SelectedElement.Position = new Vector2(0, elementPos - ((elements.Count * elementHeight + elementPadding * elements.Count) - RawRect.Height) * scrollbar.Value);

                    var temp = elements[i];

                    // To capture in lambda
                    int selectedIndex = SelectedIndex;

                    elements[SelectedIndex] = temp;
                    elements[i] = SelectedElement;


       
                    ElementMoved?.Invoke(this, new FromTo { from = selectedIndex, to = i});
                    Modified?.Invoke(this, EventArgs.Empty);
                }

                else
                {
                    if (!moving && SelectedElement.IsPressed)
                    {
                        moving = true;
                        offset = InputManager.Mouse.MouseScreenPosition.Y - Rect.Y - (elementPadding * SelectedIndex + elementHeight * SelectedIndex) + ((elements.Count * elementHeight + elementPadding * elements.Count) - RawRect.Height) * scrollbar.Value;
                    }

                    if (moving)
                    {
                        SelectedElement.Position = new Vector2(SelectedElement.Position.X, InputManager.Mouse.MouseScreenPosition.Y - Rect.Y - offset);
                    }
                }
            }
        }
    }
}
