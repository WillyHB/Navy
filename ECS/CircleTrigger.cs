using System;
using System.Collections.Generic;

namespace Navy.ECS
{
    public class CircleTrigger : Component, IUpdateableComponent
    {
        public float Radius { get; set; }
        public Vector2 Offset { get; set; }

        public event Action<GameObject> OnTriggerEnter;
        public event Action<GameObject> OnTriggerExit;

        public void Update(GameTime gameTime)
        {

            foreach (GameObject go in Level.CurrentLevel.GameObjects.ToArray())
            {
                if (!go.TryGetComponent<Collider>() || containedGameObjects.Contains(go))
                {
                    continue;
                }

                if (Intersects(go.GetComponent<Collider>().Bounds))
                {
                    containedGameObjects.Add(go);
                    OnTriggerEnter?.Invoke(go);
                    continue;
                }

                else
                {
                    OnTriggerExit?.Invoke(go);
                }
            }
        }

        public bool Intersects(Rectangle rectangle)
        {
            // the first thing we want to know is if any of the corners intersect
            var corners = new[]
            {
            new Point(rectangle.Top, rectangle.Left),
            new Point(rectangle.Top, rectangle.Right),
            new Point(rectangle.Bottom, rectangle.Right),
            new Point(rectangle.Bottom, rectangle.Left)
            };

            foreach (var corner in corners)
            {
                if (ContainsPoint(corner))
                    return true;
            }

            // next we want to know if the left, top, right or bottom edges overlap
            if (gameObject.Transform.Position.X - Offset.X - Radius > rectangle.Right || gameObject.Transform.Position.X - Offset.X + Radius < rectangle.Left)
                return false;

            if (gameObject.Transform.Position.Y - Offset.Y - Radius > rectangle.Bottom || gameObject.Transform.Position.Y - Offset.Y + Radius < rectangle.Top)
                return false;

            return true;
        }

        public bool ContainsPoint(Point point)
        {
            var vector2 = new Vector2(point.X - gameObject.Transform.Position.X - Offset.X, point.Y - gameObject.Transform.Position.Y - Offset.Y);
            return vector2.Length() <= Radius;
        }

        public bool ContainsGameObject(GameObject go)
        {
            return containedGameObjects.Contains(go);
        }

        private List<GameObject> containedGameObjects = new();
    }
}
