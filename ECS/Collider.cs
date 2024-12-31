using System.Collections.Generic;
using System;

namespace Navy.ECS
{
    public class Collider : Component, IDrawableComponent
    {
        public Vector2 Size { get; set; }
        public Vector2 Offset { get; set; }
        public bool Visible { get; set; }

        public string LayerMask { get; set; }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)(gameObject.Transform.Position.X + Offset.X - Size.X / 2), (int)(gameObject.Transform.Position.Y + Offset.Y - Size.Y / 2), (int)Size.X, (int)Size.Y);
            }

            set
            {

                Size = new Vector2(value.Width, value.Height);
                Offset = new Vector2(value.X, value.Y);
            }
        }

        public GameObject[] GetCollidingGameObjects(Vector2 offset)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            foreach (GameObject go in Level.CurrentLevel.GameObjects)
            {
                if (go.Layer == LayerMask) continue;
                if (!go.TryGetComponent<Collider>()) continue;
                if (go == gameObject) continue;
                if (!go.GetComponent<Collider>().Enabled) continue;
                if (!go.Enabled) continue;

                Vector2 otherHalfSize = new Vector2(go.GetComponent<Collider>().Size.X / 2, go.GetComponent<Collider>().Size.Y / 2);
                Vector2 thisPosition = new Vector2(gameObject.Transform.Position.X + offset.X + Offset.X, gameObject.Transform.Position.Y + offset.Y + Offset.Y);
                Vector2 thisHalfSize = Size / 2;
                Vector2 otherPosition = new Vector2(go.Transform.Position.X + go.GetComponent<Collider>().Offset.X, go.Transform.Position.Y + go.GetComponent<Collider>().Offset.Y);

                float deltaX = otherPosition.X - thisPosition.X;
                float deltaY = otherPosition.Y - thisPosition.Y;

                float intersectX = Math.Abs(deltaX) - (otherHalfSize.X + thisHalfSize.X);
                float intersectY = Math.Abs(deltaY) - (otherHalfSize.Y + thisHalfSize.Y);

                if (intersectX < 0.0f && intersectY < 0.0f)
                {
                    gameObjects.Add(go);
                }
            }

            return gameObjects.ToArray();
        }
        public bool CollideAt(Vector2 offset)
        {
            if (!Enabled)
            {
                return false;
            }

            /*
            if (gameObject.Transform.Position.X + offset.X + Size.X / 2 + Offset.X >= Level.CurrentLevel.TilemapLevelData.width * Globals.GameScale ||
                gameObject.Transform.Position.X + offset.X - Size.X / 2 + Offset.X <= 0 ||
                gameObject.Transform.Position.Y + offset.Y + Size.Y / 2 + Offset.Y >= Level.CurrentLevel.TilemapLevelData.height * Globals.GameScale ||
                gameObject.Transform.Position.Y + offset.Y - Size.Y / 2 + Offset.Y <= 0)
            {
                return true;
            }

            
            float tx1 = ((gameObject.Transform.Position.X + offset.X - Size.X / 2 + Offset.X) / Globals.GameScale / TilemapManager.GetLayer(Level.CurrentLevel.TilemapLevelData, "Collision").gridCellWidth);
            float tx2 = ((gameObject.Transform.Position.X + offset.X + Size.X / 2 + Offset.X) / Globals.GameScale / TilemapManager.GetLayer(Level.CurrentLevel.TilemapLevelData, "Collision").gridCellWidth);
            float ty1 = ((gameObject.Transform.FootPosition.Y + offset.Y + Offset.Y) / Globals.GameScale / TilemapManager.GetLayer(Level.CurrentLevel.TilemapLevelData, "Collision").gridCellHeight);
            float ty2 = ((gameObject.Transform.FootPosition.Y + offset.Y - Size.Y + Offset.Y) / Globals.GameScale / TilemapManager.GetLayer(Level.CurrentLevel.TilemapLevelData, "Collision").gridCellHeight);


            if (TilemapManager.GetTile(Level.CurrentLevel.TilemapLevelData, "Collision", (int)tx1, (int)ty2) ||
                 TilemapManager.GetTile(Level.CurrentLevel.TilemapLevelData, "Collision", (int)tx2, (int)ty1) ||
                 TilemapManager.GetTile(Level.CurrentLevel.TilemapLevelData, "Collision", (int)tx2, (int)ty2) ||
                 TilemapManager.GetTile(Level.CurrentLevel.TilemapLevelData, "Collision", (int)tx1, (int)ty1))
            {
                return true;
            }
            */

            foreach (GameObject go in Level.CurrentLevel.GameObjects)
            {
                if (go.Layer == LayerMask) continue;
                if (!go.TryGetComponent<Collider>()) continue;
                if (go == gameObject) continue;
                if (!go.GetComponent<Collider>().Enabled) continue;
                if (!go.Enabled) continue;

                Vector2 otherHalfSize = new(go.GetComponent<Collider>().Size.X / 2, go.GetComponent<Collider>().Size.Y / 2);
                Vector2 thisPosition = new(gameObject.Transform.Position.X + offset.X + Offset.X, gameObject.Transform.Position.Y + offset.Y + Offset.Y);
                Vector2 thisHalfSize = Size / 2;
                Vector2 otherPosition = new(go.Transform.Position.X + go.GetComponent<Collider>().Offset.X, go.Transform.Position.Y + go.GetComponent<Collider>().Offset.Y);

                float deltaX = otherPosition.X - thisPosition.X;
                float deltaY = otherPosition.Y - thisPosition.Y;

                float intersectX = Math.Abs(deltaX) - (otherHalfSize.X + thisHalfSize.X);
                float intersectY = Math.Abs(deltaY) - (otherHalfSize.Y + thisHalfSize.Y);

                if (intersectX < 0.0f && intersectY < 0.0f)
                {
                    return true;
                }
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                Texture2D whiteRectangle = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                whiteRectangle.SetData(new[] { new Color(0, 0, 255, 100) });
                spriteBatch.Draw(whiteRectangle, new Vector2(gameObject.Transform.Position.X - Size.X / 2 + Offset.X, gameObject.Transform.Position.Y - Size.Y / 2 + Offset.Y), new Rectangle(0, 0, (int)(Size.X), (int)(Size.Y)), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }
    }
}
