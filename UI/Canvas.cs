using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Navy.ECS;

namespace Navy.UI
{
    public class Canvas : Component, IUpdateableComponent, IDrawableComponent
    {
        public Canvas() { }
        public Canvas(Rectangle rect, Texture2D texture = null)
        {
            Rect = rect;
            
            BackgroundTexture = texture ?? Globals.EmptyTexture;
        }
        public override void Initialize()
        {
            BaseResolution = ResolutionHandler.GetVirtualResolution();

            InputManager.Mouse.Scrolled += (sender, i) => { if (gameObject.Enabled) OnScroll(i); };
        }

        public override void Destroy()
        {
            rasterizerState.Dispose();

            InputManager.Mouse.Scrolled -= (sender, i) => { if (gameObject.Enabled) OnScroll(i); };

            while (elements.Count > 0)
            {
                elements[0].Destroy();
            }
        }

        public Vector2 BaseResolution { get; set; }
        

        public Rectangle Rect
        {
            get
            {
                return new Rectangle((int)((RenderInWorld ? gameObject.Transform.Position.X : 0) + rect.X - LocalOffset.X + GlobalOffset.X), 
                    (int)((RenderInWorld ? gameObject.Transform.Position.Y: 0) + rect.Y - LocalOffset.Y + GlobalOffset.Y),
                    (int)(ScaleWithScreen ? (ResolutionHandler.GetVirtualResolution().X - BaseResolution.X) + rect.Width : rect.Width), rect.Height);

            }

            set
            {
                rect = value;
            }
        }

        public void SetX(int x) => rect.X = x;
        public void SetY(int y) => rect.Y = y;
        public void MoveX(int x) => rect.X += x;
        public void MoveY(int y) => rect.Y += y;
        public void SetWidth(int width) => rect.Width = width;
        public void SetHeight(int height) => rect.Height = height;

        public RasterizerState rasterizerState = new() { ScissorTestEnable = true };

        private Rectangle rect;
        public Texture2D BackgroundTexture { get; set; }
        public Color BackgroundColor { get; set; } = Color.Transparent;

        public Origin Origin { set { GlobalOrigin = value; LocalOrigin = value; } }
        public Origin GlobalOrigin { get; set; } = Origin.Center;
        public Origin LocalOrigin { get; set; } = Origin.Center;

        public bool BlockMouseInput { get; set; } = true;
        public bool ScaleWithScreen { get; set; }

        public bool MaskOutsideCanvas { get; set; }
        public bool RenderInWorld { get; set; }


        public Vector2 GlobalOffset => GetOriginOffset(new Rectangle(0, 0, (int)ResolutionHandler.GetVirtualResolution().X, (int)ResolutionHandler.GetVirtualResolution().Y), GlobalOrigin);


        public Vector2 LocalOffset => GetOriginOffset(new Rectangle(RenderInWorld ? (int)gameObject.Transform.Position.X + rect.X : rect.X, 
                                                                    RenderInWorld ? (int)gameObject.Transform.Position.Y + rect.Y : rect.Y, 
            (int)(ScaleWithScreen ? (ResolutionHandler.GetVirtualResolution().X / BaseResolution.X) * rect.Width : rect.Width), rect.Height), LocalOrigin);


        public float MaskPadding { get; set; }

        public Vector2 Scrolled { get; set; } = Vector2.Zero;
        public Vector2 ScrollAmount { get; set; } = Vector2.Zero;

        private List<CanvasElement> elements = new();
        
        public static bool IsBlockingKeyboard()
        {
            foreach (Canvas canvas in GameObject.FindComponents<Canvas>())
            {
                foreach (InputField field in canvas.FindElementsOfType<InputField>())
                {
                    if (field.IsSelected)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public static bool IsBlockingKeyboard(out Canvas blockingCanvas)
        {
            foreach (Canvas canvas in GameObject.FindComponents<Canvas>())
            {
                foreach (InputField field in canvas.FindElementsOfType<InputField>())
                {
                    if (field.IsSelected)
                    {
                        blockingCanvas = canvas;
                        return true;
                    }
                }
            }

            blockingCanvas = null;
            return false;
        }

        public static bool IsBlockingMouse()
        {
            Canvas[] canvases = GameObject.FindComponents<Canvas>();

            for (int i = canvases.Length - 1; i >= 0; i--)
            {
                if (canvases[i].Enabled)
                {
                    if (canvases[i].Rect.Contains(InputManager.Mouse.MouseScreenPosition))
                    {
                        if (canvases[i].BlockMouseInput)
                        {
                            return true;
                        }

                        foreach (CanvasElement element in canvases[i].GetFullElementArray())
                        {
                            if (element.Rect.Contains(InputManager.Mouse.MouseScreenPosition))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        public static bool IsBlockingMouse(out Canvas blockingCanvas)
        {
            Canvas[] canvases = GameObject.FindComponents<Canvas>();

            for (int i = canvases.Length-1; i >= 0; i--)
            {
                if (canvases[i].Enabled)
                {
                    if (canvases[i].Rect.Contains(InputManager.Mouse.MouseScreenPosition))
                    {
                        if (canvases[i].BlockMouseInput)
                        {
                            blockingCanvas = canvases[i];
                            return true;
                        }

                        foreach (CanvasElement element in canvases[i].GetFullElementArray())
                        {
                            if (element.BlockMouseInput)
                            {
                                if (element.Rect.Contains(InputManager.Mouse.MouseScreenPosition))
                                {
                                    blockingCanvas = canvases[i];
                                    return true;
                                }
                            }
                        }
                    }
                }              
            }

            blockingCanvas = null;
            return false;
        }

        public bool IsMouseBlocked()
        {
            if (Rect.Contains(InputManager.Mouse.MouseScreenPosition))
            {
                Canvas[] canvases = GameObject.FindComponents<Canvas>();

                for (int i = canvases.Length - 1; i > GameObject.FindComponents<Canvas>().ToList().IndexOf(this); i--)
                {
                    if (canvases[i].Enabled)
                    {
                        if (canvases[i].Rect.Contains(InputManager.Mouse.MouseScreenPosition))
                        {
                            if (canvases[i].BlockMouseInput)
                            {
                                return true;
                            }
                        }
                    }                  
                }
            }

        
            return false;
        }
        public bool IsMouseBlocked(out Canvas canvas)
        {
            if (Rect.Contains(InputManager.Mouse.MouseScreenPosition))
            {
                Canvas[] canvases = GameObject.FindComponents<Canvas>();

                for (int i = canvases.Length - 1; i > GameObject.FindComponents<Canvas>().ToList().IndexOf(this); i--)
                {
                    if (canvases[i].Enabled)
                    {
                        if (canvases[i].Rect.Contains(InputManager.Mouse.MouseScreenPosition))
                        {
                            if (canvases[i].BlockMouseInput)
                            {
                                canvas = canvases[i];
                                return true;
                            }
                        }
                    }                   
                }
            }

            canvas = null;
            return false;
        }

        public CanvasElement[] GetFullElementArray()
        {
            List<CanvasElement> elementList = new();

            foreach (var e in elements)
            {
                elementList.Add(e);

                elementList.AddRange(e.GetDescendants());
            }

            return elementList.ToArray();
        }


        public void Update(GameTime gameTime)
        {
            CanvasElement[] allElements = GetFullElementArray();

            for (int i = 0; i < allElements.Length; i++)
            {
                if (!allElements[i].Enabled)
                {
                    continue;
                }

                allElements[i].Update(gameTime);

                if (allElements[i] is not null)
                {
                    if (allElements[i].Rect.Contains(InputManager.Mouse.MouseScreenPosition))
                    {
                        allElements[i].HasMouseEntered = true;
                    }

                    else
                    {
                        allElements[i].HasMouseEntered = false;
                    }
                }
            }
        }

        private void OnScroll(int amount)
        {
            foreach (Canvas canvas in GameObject.FindComponents<Canvas>())
            {
                if (canvas == this) continue;

                if (canvas.Enabled)
                {
                    if (canvas.BlockMouseInput)
                    {
                        if (canvas.Rect.Contains(InputManager.Mouse.MouseScreenPosition))
                        {
                            if (GameObject.FindComponents<Canvas>().ToList().IndexOf(canvas) > GameObject.FindComponents<Canvas>().ToList().IndexOf(this)) return;
                        }
                    }
                }              
            }

            if (ScrollAmount.Y > 0 && Rect.Contains(InputManager.Mouse.MouseScreenPosition))
            {
                Scrolled = new Vector2(Scrolled.X, Scrolled.Y - (amount / 5) > 0 ? (Scrolled.Y - (amount / 5) < ScrollAmount.Y ? Scrolled.Y - (amount / 5) : ScrollAmount.Y) : 0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: rasterizerState);


            if (BackgroundTexture != null)
            {
                spriteBatch.Draw(BackgroundTexture, new Rectangle((int)(Rect.X), Rect.Y, (int)(Rect.Width), (int)(Rect.Height)), null, BackgroundColor, 0, Vector2.Zero, SpriteEffects.None, 0);
            }

            if (MaskOutsideCanvas)
            {
                spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(Rect.X + (int)MaskPadding / 2, Rect.Y + (int)MaskPadding / 2, Rect.Width - (int)MaskPadding, Rect.Height - (int)MaskPadding);
            }


            spriteBatch.End();


            foreach (CanvasElement e in elements)
            {
                if (e.Enabled)
                {
                    e.DrawElement(spriteBatch);
                }

                spriteBatch.GraphicsDevice.ScissorRectangle = spriteBatch.GraphicsDevice.Viewport.Bounds;
            }

        }


        /// <summary>
        /// Adds an element to the Canvas
        /// </summary>
        public T AddElement<T>(T canvasElement) where T : CanvasElement
        {
            if (elements.Contains(canvasElement))
            {
                return canvasElement;
            }


            elements.Add(canvasElement);

            return canvasElement;
        }

        /// <summary>
        /// Removes an element from the Canvas
        /// </summary>
        public void RemoveElement(CanvasElement canvasElement)
        {
            if (!elements.Contains(canvasElement))
            {
                return;
            }

            elements.Remove(canvasElement);
        }

        /// <summary>
        /// Return the first found instance of an element of type <typeparamref name="T"/> within the canvas
        /// </summary>
        public T FindElementOfType<T>() where T : CanvasElement
        {
            // create an array of all the elments within the current canvas, for convenience sake
            CanvasElement[] allElements = GetFullElementArray();

            // loops through all, and returns the first instance of an element with type T
            for (int i = 0; i < allElements.Length; i++)
            {
                if (allElements[i] is T t)
                {
                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// Return all found instances of elements of type <typeparamref name="T"/> within the canvas
        /// </summary>
        public T[] FindElementsOfType<T>() where T : CanvasElement
        {
            // creates a list, so it can be returned later
            List<T> e = new();

            CanvasElement[] allElements = GetFullElementArray();

            // loops through all canvas elements, and adds any elements of type T to the previously created list
            for (int i = 0; i < allElements.Length; i++)
            {
                if (allElements[i] is T t)
                {
                    e.Add(t);    
                }
            }

            return e.ToArray();
        }

        /// <summary>
        /// Return the offset needed to be applied to a rectangle to conform to origin
        /// </summary>
        /// 
        public CanvasElement GetElementByTag(string tag)
        {
            var elements = GetFullElementArray();

            foreach (CanvasElement element in elements)
            {
                if (element.Tag == tag)
                {
                    return element;
                }
            }

            return null;
        }

        public static CanvasElement FindElementByTag(string tag)
        {
            Canvas[] canvases = GameObject.FindComponents<Canvas>();

            foreach (var canvas in canvases)
            {
                if (canvas.GetElementByTag(tag) is not null)
                {
                    return canvas.GetElementByTag(tag);
                }
            }

            return null;
        }
        public static CanvasElement[] FindElementsByTag(string tag)
        {
            Canvas[] canvases = GameObject.FindComponents<Canvas>();
            List<CanvasElement> elements = new();

            foreach (var canvas in canvases)
            {
                if (canvas.GetElementByTag(tag) is not null)
                {
                    elements.Add(canvas.GetElementByTag(tag));
                }
            }

            return elements.ToArray();
        }

        public static T FindElementByTag<T>(string tag) where T : CanvasElement
        {
            Canvas[] canvases = GameObject.FindComponents<Canvas>();

            foreach (var canvas in canvases)
            {
                if (canvas.GetElementByTag(tag) is not null && canvas.GetElementByTag(tag) is T t)
                {
                    return t;
                }
            }

            return null;
        }

        public static T[] FindElementsByTag<T>(string tag) where T : CanvasElement
        {
            Canvas[] canvases = GameObject.FindComponents<Canvas>();
            List<T> elements = new();

            foreach (var canvas in canvases)
            {
                if (canvas.GetElementByTag(tag) is not null && canvas.GetElementByTag(tag) is T t)
                {
                    elements.Add(t);
                }
            }

            return elements.ToArray();
        }


        public static Vector2 GetOriginOffset(Rectangle Rect, Origin origin) => origin switch
        {
            Origin.TopLeft => Vector2.Zero,
            Origin.TopRight => new Vector2(Rect.Width, 0),
            Origin.BottomRight => new Vector2(Rect.Width, Rect.Height),
            Origin.BottomLeft => new Vector2(0, Rect.Height),
            Origin.BottomMid => new Vector2(Rect.Width / 2, Rect.Height),
            Origin.TopMid => new Vector2(Rect.Width / 2, 0),
            Origin.Center => new Vector2(Rect.Width / 2, Rect.Height / 2),
            Origin.LeftMid => new Vector2(0, Rect.Height / 2),
            Origin.RightMid => new Vector2(Rect.Width, Rect.Height / 2),
            _ => Vector2.Zero,
        };

        /// <summary>
        /// Returns the offset needed for the specified text to conform to origin
        /// </summary>
        public static Vector2 GetTextOriginOffset(Rectangle rect, Origin origin, Vector2 textSize, Vector2 padding, float scale = 1) => origin switch
        {
            Origin.TopLeft => new Vector2(textSize.X / 2 * scale + padding.X, textSize.Y / 2 * scale + padding.Y),
            Origin.TopRight => new Vector2(rect.Width - textSize.X / 2 * scale - padding.X, textSize.Y / 2 * scale + padding.Y),
            Origin.BottomLeft => new Vector2(textSize.X / 2 * scale + padding.X, rect.Height - textSize.Y / 2 * scale - padding.Y),
            Origin.BottomRight => new Vector2(rect.Width - textSize.X / 2 * scale - padding.X, rect.Height - textSize.Y / 2 * scale - padding.Y),
            Origin.TopMid => new Vector2(rect.Width / 2, textSize.Y / 2 * scale + padding.Y),
            Origin.BottomMid => new Vector2(rect.Width / 2, rect.Height - textSize.Y / 2 * scale - padding.Y),
            Origin.LeftMid => new Vector2(textSize.X / 2 * scale + padding.X, rect.Height / 2),
            Origin.RightMid => new Vector2(rect.Width - textSize.X / 2 * scale - padding.X, rect.Height / 2),
            Origin.Center => new Vector2(rect.Width / 2, rect.Height / 2),
            _ => new Vector2(textSize.X / 2 * scale + padding.X, textSize.Y / 2 * scale + padding.Y),
        };

    }
}
