using System.Collections.Generic;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Navy.UI
{
    #region Constants
    public enum ElementState
    {
        Normal,
        Active,
        Hover,
    };

    public enum Origin
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        TopMid,
        BottomMid,
        LeftMid,
        RightMid,
        Center
    }

    public enum TextStyleType
    {
        None,
        Wrap,
        Fit,
    }

    public abstract class Interaction
    {
        public float Smoothing { get; set; }
    }

    public class Interaction<T> : Interaction
    {
        public Interaction(Func<T> getter, Action<T> setter, T normal, T hover, T active, float smoothing)
        {
            Getter = getter;
            Setter = setter;
            Normal = normal;
            Hover = hover;
            Active = active;
            Smoothing = smoothing;
        }

        public Interaction(Func<T> getter, Action<T> setter, T value)
        {
            Getter = getter;
            Setter = setter;
            Normal = value;
            Hover = value;
            Active = value;
            Smoothing = 0;
        }

        public Interaction(Func<T> getter, Action<T> setter, Interaction<T> copyInteraction)
        {
            Getter = getter;
            Setter = setter;
            Normal = copyInteraction.Normal;
            Hover = copyInteraction.Hover;
            Active = copyInteraction.Active;
            Smoothing = copyInteraction.Smoothing;
        }

        public Func<T> Getter;
        public Action<T> Setter;

        public T Normal;
        public T Hover;
        public T Active;

    }
    public enum ActivityStyle
    {
        Default,
        TogglePress,
        ToggleOn,
        ToggleRelease
    }

    public enum TooltipLocation
    {
        Screen,
        CornerOfMouseAuto
    }
    #endregion


    public static class CanvasElementExtensions
    {
        //public static T ShallowDuplicate<T>(this T element) where T : CanvasElement => (T)element.ShallowClone();

        public static T Duplicate<T>(this T element) where T : CanvasElement => (T)element.DeepClone();

        public static T SetPosition<T>(this T element, Vector2 position) where T : CanvasElement
        {
            element.Position = position;
            return element;
        }

        public static T SetPosition<T>(this T element, float x, float y) where T : CanvasElement
        {
            element.Position = new Vector2(x,y);
            return element;
        }
    }

    public abstract class CanvasElement
    {
        public void Initialize()
        {
            if (Enabled)
            {
                if (Parent == null)
                {
                    Enabled = ParentCanvas.Enabled;
                }

                else
                {
                    Enabled = Parent.Enabled;
                }
            }

            InputManager.Mouse.ButtonPressed += (s, b) => { if (Enabled) OnPress(s, b); };
            InputManager.Mouse.ButtonReleased += (s, b) => { if (Enabled) OnRelease(s, b); };
            watcher = new FieldWatcher<bool>(() => HasMouseEntered);
            watcher.FieldChanged += (s, prevValue) =>
            {
                if (watcher.Field) Enter();
                else Exit();
            };

            OnInitialize();
        }


        public void Destroy()
        {
            interactions.Clear();
            watcher.Dispose();
            for (int i = 0; i < children.Count; i++)
            {
                children[i].Destroy();
            }

            InputManager.Mouse.ButtonPressed -= (s, b) => { if (Enabled) OnPress(s, b); };
            InputManager.Mouse.ButtonReleased -= (s, b) => { if (Enabled) OnRelease(s, b); };

            Pressed = null;
            Released = null;
            MouseEntered = null;
            MouseExited = null;
            StateSwitched = null;

            Parent?.RemoveChild(this);
            ParentCanvas?.RemoveElement(this);
            ParentCanvas = null;
            Parent = null;

            OnDestroy();

            Destroyed?.Invoke(this, EventArgs.Empty);
            Destroyed = null;

        }


        private FieldWatcher<bool> watcher;
        public virtual Color BackgroundColor { get; set; } = Color.White;
        public virtual Color Color { get; set; } = Color.Black;

        public virtual Vector2 Scale { get; set; } = Vector2.One;
        
        public bool IsMouseOver
        {
            get
            {
                if (ParentCanvas.IsMouseBlocked()) return false;
                if (!Rect.Contains(InputManager.Mouse.MouseScreenPosition)) return false;

                CanvasElement[] allElements = ParentCanvas.GetFullElementArray();

                for (int i = allElements.Length - 1; i > Array.IndexOf(allElements, this); i--)
                {
                    if (allElements[i].BlockMouseInput && allElements[i].Rect.Contains(InputManager.Mouse.MouseScreenPosition))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        public bool IsPressed
        {
            get => InputManager.Mouse.IsButtonDown(MouseButton.Left) && IsMouseOver;
        }
        public bool IsReleased
        {
            get => !IsPressed;
        }

        public Vector2 Position
        {
            get { return RawRect.Location.ToVector2(); }
            set { RawRect = new Rectangle((int)value.X, (int)value.Y, RawRect.Width, RawRect.Height); }
        }

        protected virtual void OnDestroy() { }
        protected virtual void OnInitialize() { }
        public virtual void Update(GameTime gameTime) { }

        public void DrawElement(SpriteBatch spriteBatch)
        {
            Rectangle prevScissor = spriteBatch.GraphicsDevice.ScissorRectangle;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState:ParentCanvas.rasterizerState);

            if (MaskRect != null)
            {
                spriteBatch.GraphicsDevice.ScissorRectangle = MaskRect.Value;
            }

            Draw(spriteBatch);

            spriteBatch.End();

            foreach (var child in children)
            {
                if (child.Enabled)
                {
                    child.DrawElement(spriteBatch);
                }
            }

                        spriteBatch.GraphicsDevice.ScissorRectangle = prevScissor;
        }

        protected abstract void Draw(SpriteBatch spriteBatch);

        public void RemoveFromCanvas() => ParentCanvas.RemoveElement(this);

        protected Vector2 LocalOffset
        {
            get => Canvas.GetOriginOffset(RawRect, LocalOrigin);
        }
        protected Vector2 GlobalOffset
        {
            get => Canvas.GetOriginOffset(Parent?.Rect ?? ParentCanvas.Rect, GlobalOrigin);
        }

        public string Tag { get; set; }
        public Rectangle? MaskRect { get; set; }
        public bool ScaleWithScreen { get; set; }
        public bool BlockMouseInput { get; set; } = true;

        public Rectangle RawRect { get; set; }
        public Rectangle Rect
        {
            get
            {
                if (ParentCanvas == null) return RawRect;

                return new((int)(RawRect.X - LocalOffset.X * Scale.X + GlobalOffset.X + (Parent?.Rect.X ?? ParentCanvas.Rect.X) - ParentCanvas.Scrolled.X), (int)(RawRect.Y - LocalOffset.Y * Scale.Y + GlobalOffset.Y + (Parent?.Rect.Y ?? ParentCanvas.Rect.Y) - ParentCanvas.Scrolled.Y),
               (int)((ScaleWithScreen ? ResolutionHandler.GetVirtualResolution().X - ParentCanvas.BaseResolution.X : 0) + (RawRect.Width * Scale.X)), (int)(RawRect.Height * Scale.Y));
            }
        }

        public Texture2D Texture { get; set; }

        public Origin Origin { set { GlobalOrigin = value; LocalOrigin = value; } }


        public Origin GlobalOrigin { get; set; } = Origin.TopLeft;
        public Origin LocalOrigin { get; set; } = Origin.TopLeft;

        private CanvasElement parent;
        private Canvas parentCanvas;
        public CanvasElement Parent
        { 
            get => parent;
            set
            {
                if (value != Parent)
                {
                    Parent?.children.Remove(this);
                    ParentCanvas?.RemoveElement(this);

                    parent = value;
                    ParentCanvas = parent?.ParentCanvas;

                    value?.children.Add(this);
                }
            } 
        }
        public Canvas ParentCanvas 
        {
            get
            {
                return parentCanvas;
            }

            set
            {
                if (value != parentCanvas)
                {
                    parentCanvas?.RemoveElement(this);

                    Parent?.children.Remove(this);

                    parentCanvas = value;

                    if (value != null)
                    {
                        value.AddElement(this);
                        Initialize();
                    }             
                }
            }
        }

        private readonly List<CanvasElement> children = new();

        public void RemoveChild(CanvasElement child)
        {
            children.Remove(child);
        }

        public CanvasElement[] GetChildren() => children.ToArray();
        public CanvasElement[] GetDescendants()
        {
            List<CanvasElement> childrenList = new();

            foreach (var child in children)
            {
                childrenList.Add(child);

                childrenList.AddRange(child.GetDescendants());
            }

            return [.. childrenList];

        }
        public CanvasElement GetChild(int i) => children[i] ?? null;

        public bool HasChild(CanvasElement child) => children.Contains(child);

        public CanvasElement GetChildOfType<T>()
        {
            foreach (var child in children)
            {
                if (child is T)
                {
                    return child;
                }
            }

            return null;
        }
        public CanvasElement[] GetChildrenOfType<T>()
        {
            List<CanvasElement> elements = [];

            foreach (var child in children)
            {
                if (child is T)
                {
                    elements.Add(child);
                }
            }

            return [.. elements];
        }

        public CanvasElement GetChildByTag(string tag)
        {
            foreach (var child in children)
            {
                if (child.Tag == tag)
                {
                    return child;
                }
            }

            return null;
        }
        public CanvasElement[] GetChildrenByTag(string tag)
        {
            List<CanvasElement> elements = [];
            foreach (var child in children)
            {
                if (child.Tag == tag)
                {
                    elements.Add(child);
                }
            }

            return [.. elements];
        }


        public bool Enabled { get => enabled && (Parent == null ? (ParentCanvas != null && ParentCanvas.Enabled) : Parent.Enabled); set { children.ForEach((child) => child.Enabled = value) ; enabled = value; } } 

        private bool enabled = true;



        public bool HasMouseEntered { get; set; }

        public void LockCurrentState()
        {
            currentStateLocked = true;
        }
        public void UnlockCurrentState()
        {
            currentStateLocked = false;
        }

        private bool currentStateLocked;


        private readonly List<Interaction> interactions = [];

        public Interaction<T> GetInteraction<T>(T value)
        {       
            foreach (Interaction i in interactions)
            {
                if (i is Interaction<T> interaction)
                {
                    if (EqualityComparer<T>.Default.Equals(interaction.Getter.Invoke(), value))
                    {
                        return interaction;
                    }
                }
            }
            
            return new Interaction<T>(()=>value, (v)=>value=v, value);
        }
        public Interaction<T> AddInteraction<T>(Func<T> getter, Action<T> setter, T normal, T hover, T active, float smoothing = 0)
        {            
            Interaction<T> interaction = new(getter, setter, normal, hover, active, smoothing);
            
            interactions.Add(interaction);

            setter.Invoke(normal);

            StateSwitched += (s, state) =>
            {
                setter.Invoke(state switch
                {
                    ElementState.Normal => normal,
                    ElementState.Hover => hover,
                    ElementState.Active => active,
                    _ => throw new NotImplementedException()
                });
            };
            
            return interaction;
        }
        public Interaction<T> AddInteraction<T>(Func<T> getter, Action<T> setter, Interaction<T> copyInteraction) => AddInteraction(getter, setter, copyInteraction.Normal, copyInteraction.Hover, copyInteraction.Active, copyInteraction.Smoothing);

        public virtual Rectangle InteractRect { get; set; }

        public virtual ElementState State { get; set; } = ElementState.Normal;
        public virtual ActivityStyle ActivityStyle { get; set; } = ActivityStyle.Default;

        public event EventHandler<MouseButton> Pressed;
        public event EventHandler<MouseButton> Released;
        public event EventHandler MouseEntered;
        public event EventHandler MouseExited;
        public event EventHandler<ElementState> StateSwitched;
        public event EventHandler Destroyed;


        public void SwitchState(ElementState state)
        {
            if (!currentStateLocked)
            {
                if (state != State)
                {
                    State = state;

                    StateSwitched?.Invoke(this, state);
                    /*
                    Tweener.TweenValue(() => Scale, (v) => { Scale = v; }, Scale, scalesTo, Scales.Smoothing);
                    Tweener.TweenValue(() => Color, (v) => { Color = v; }, Color, colorsTo, Colors.Smoothing);
                    Tweener.TweenValue(() => BackgroundColor, (v) => { BackgroundColor = v; }, BackgroundColor, backgroundColorsTo, BackgroundColors.Smoothing);
                    */
                }
            }
        }

        public void Enter()
        {
            if (State == ElementState.Normal)
            {
                SwitchState(ElementState.Hover);
            }

            MouseEntered?.Invoke(this, EventArgs.Empty);
        }

        public void Exit()
        {
            if (State != ElementState.Active)
            {
                SwitchState(ElementState.Normal);
            }

            MouseExited?.Invoke(this, EventArgs.Empty);
        }

        public void Press(MouseButton button) => Pressed?.Invoke(this, button);
        public void Release(MouseButton button) => Released?.Invoke(this, button);

        public void OnPress(object sender, MouseButton button)
        {
            if (ParentCanvas.IsMouseBlocked())
            {
                return;
            }

            if (InteractRect.Contains(InputManager.Mouse.MouseScreenPosition))
            {
                if (button == MouseButton.Left)
                {
                    if (State == ElementState.Active)
                    {
                        if (ActivityStyle == ActivityStyle.TogglePress)
                        {
                            SwitchState(ElementState.Hover);
                        }
                    }

                    else if (State == ElementState.Hover)
                    {
                        SwitchState(ElementState.Active);

                        if (ActivityStyle == ActivityStyle.ToggleRelease)
                        {
                            thisPress = true;
                        }
                    }
                }

                Pressed?.Invoke(this, button);
            }

            else if (State == ElementState.Active)
            {
                SwitchState(ElementState.Normal);
            }
        }

        private bool thisPress;

        public void OnRelease(object sender, MouseButton button)
        {   
            if (State == ElementState.Active)
            {
                if (ActivityStyle == ActivityStyle.Default || ActivityStyle == ActivityStyle.ToggleRelease)
                {
                    if (thisPress)
                    {
                        thisPress = false;
                        return;
                    }

                    if (InteractRect.Contains(InputManager.Mouse.MouseScreenPosition) && !ParentCanvas.IsMouseBlocked())
                    {
                        if (button == MouseButton.Left)
                        {
                            SwitchState(ElementState.Hover);
                        }

                        Released?.Invoke(this, button);
                    }

                    else
                    {
                        if (button == MouseButton.Left)
                        {
                            SwitchState(ElementState.Normal);
                        }

                    }
                }
            }       
        }

        public Rectangle GetScaledRect(Vector2 scale) => new Rectangle((int)(RawRect.X - LocalOffset.X * scale.X + GlobalOffset.X + (Parent == null ? ParentCanvas.Rect.X : Parent.Rect.X) - ParentCanvas.Scrolled.X), (int)(RawRect.Y - LocalOffset.Y * scale.Y + GlobalOffset.Y + (Parent == null ? ParentCanvas.Rect.Y : Parent.Rect.Y) - ParentCanvas.Scrolled.Y),
            (int)((ScaleWithScreen ? ResolutionHandler.GetVirtualResolution().X / ParentCanvas.BaseResolution.X : 1) * (RawRect.Width * scale.X)), (int)(RawRect.Height * scale.Y));
    } 


}