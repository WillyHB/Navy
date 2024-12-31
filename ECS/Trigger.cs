using System.Collections.Generic;
using System;

namespace Navy.ECS
{
    public class Trigger : Component, IUpdateableComponent, IDrawableComponent
    {
        public Vector2 Size { get; set; }
        public Vector2 Offset { get; set; }
        public bool Visible { get; set; } = false;
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)(gameObject.Transform.Position.X + Offset.X - Size.X / 2), (int)(gameObject.Transform.Position.Y + Offset.Y - Size.Y), (int)Size.X, (int)Size.Y);
            }

            set
            {
                Size = new Vector2(value.Width, value.Height);
                Offset = new Vector2(value.X, value.Y);
            }
        }

        public event EventHandler<GameObject> OnTriggerEnter;
        public event EventHandler<GameObject> OnTriggerExit;

        public void Update(GameTime gameTime)
        {

            foreach (GameObject go in Level.CurrentLevel.GameObjects.ToArray())
            {
                if (!go.TryGetComponent<Collider>() || containedGameObjects.Contains(go))
                {
                    continue;
                }

                if (Bounds.Contains(go.GetComponent<Collider>().Bounds))
                {
                    containedGameObjects.Add(go);
                    OnTriggerEnter?.Invoke(this, go);
                    continue;
                }

                else
                {
                    OnTriggerExit?.Invoke(this, go);
                }
            }
        }

        public bool ContainsGameObject(GameObject go)
        {
            return containedGameObjects.Contains(go);
        }

        private List<GameObject> containedGameObjects = new List<GameObject>();
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                Texture2D whiteRectangle = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                whiteRectangle.SetData(new[] { new Color(0, 0, 255, 100) });
                spriteBatch.Draw(whiteRectangle, new Vector2(Bounds.X, Bounds.Y), new Rectangle(0, 0, (int)(Bounds.Width), (int)(Bounds.Height)), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }
    }
}
