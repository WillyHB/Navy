using System.Collections.Generic;
using System.Linq;
using System;

namespace Navy.UI
{
    public class CheckList : Listbox
    {
        public CheckList(Rectangle RawRect, float elementHeight, float elementPadding, Vector2 textPadding, Texture2D backgroundTex, Texture2D elementTex = null) : base(RawRect, elementHeight, elementPadding, textPadding, backgroundTex, elementTex)
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Checked = null;
        }

        public event EventHandler<int> Checked;
        public bool[] GetCheckValues()
        {
            List<bool> values = new();

            for (int i = 0; i < ElementCount; i++)
            {
                foreach (var element in elements[i].GetChildren())
                {
                    if (element is Checkbox checkbox)
                    {
                        values.Add(checkbox.Value);
                    }
                }
            }

            return values.ToArray();
        }

        public bool GetCheckValue(int i)
        {
            foreach (var element in elements[i].GetChildren())
            {
                if (element is Checkbox checkbox)
                {
                    return checkbox.Value;
                }
            }

            return false;
        }

        public int IndexOfCheck(Checkbox checkbox)
        {
            for (int i =0; i < ElementCount; i++)
            {
                foreach (var element in elements[i].GetChildren())
                {
                    if (element == checkbox)
                    {
                        return i;
                    }
                }
            }


            return -1;
        }

        public override Button AddElement(string text)
        {
            Button button = base.AddElement(text);

            Checkbox checkbox = new Checkbox(new Rectangle((int)(Rect.Width - Rect.Width * 0.1f - elementHeight), 0, (int)elementHeight, (int)elementHeight), Globals.EmptyTexture)
            {
                Parent = button,
                Color = TextColor,
                InactiveColor = BackgroundColor,
                BackgroundColor = ButtonBackgroundColor,
            };

            checkbox.ValueChanged += (sender, value) =>
            {
                OnModified(this, EventArgs.Empty);
                Checked?.Invoke(this, IndexOfCheck(checkbox));
            };

            button.AddInteraction(() => checkbox.BackgroundColor, (v) => checkbox.BackgroundColor = v, GetInteraction(ButtonBackgroundColor));

            return button;
        }

        public Button AddElement(string text, bool value)
        {
            Button button = base.AddElement(text);

            Checkbox checkbox = new Checkbox(new Rectangle((int)(Rect.Width - Rect.Width * 0.1f - elementHeight), 0, (int)elementHeight, (int)elementHeight), Globals.EmptyTexture)
            {
                Parent = button,
                Color = TextColor,
                InactiveColor = BackgroundColor,
                BackgroundColor = ButtonBackgroundColor,
                Value = value
            };

            checkbox.InteractRect = checkbox.Rect;

            checkbox.ValueChanged += (sender, value) =>
            {
                OnModified(this, EventArgs.Empty);
                Checked?.Invoke(this, IndexOfCheck(checkbox));
            };

            button.AddInteraction(() => checkbox.BackgroundColor, (v) => checkbox.BackgroundColor = v, GetInteraction(ButtonBackgroundColor));

            return button;
        }
    }
}
