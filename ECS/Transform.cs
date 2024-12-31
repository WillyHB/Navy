using System;

namespace Navy.ECS
{
    public class Transform : Component
    {
        public Vector2 Position { get => position + (gameObject.Parent is not null ? gameObject.Parent.Transform.Position : Vector2.Zero); set => position = value; }
        private Vector2 position = Vector2.Zero;

        public Vector2 Scale { get; set; } = Vector2.One;

        public float xRemainder, yRemainder;
        public void Move(float x, float y)
        {
            MoveX(x);
            MoveY(y);
        }
        public void Move(Vector2 amount)
        {
            MoveX(amount.X);
            MoveY(amount.Y);
        }
        public void MoveX(float x)
        {
            if (float.IsNaN(x)) return;

            xRemainder += x;
            int move = (int)Math.Round(xRemainder);

            if (move != 0)
            {
                xRemainder -= move;
                int sign = Math.Sign(x);

                while (move != 0)
                {
                    if (gameObject.TryGetComponent<Collider>())
                    {
                        if (gameObject.GetComponent<Collider>().CollideAt(new Vector2(sign, 0)))
                        {
                            goto sign;
                        }
                    }

                    Position = new Vector2(Position.X + sign, Position.Y);

                sign: move -= sign;
                }
            }
        }
        public void MoveY(float y)
        {
            if (float.IsNaN(y)) return;

            yRemainder += y;
            int move = (int)Math.Round(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(y);

                while (move != 0)
                {
                    if (gameObject.TryGetComponent<Collider>())
                    {
                        if (gameObject.GetComponent<Collider>().CollideAt(new Vector2(0, sign)))
                        {
                            goto sign;
                        }
                    }

                    Position = new Vector2(Position.X, Position.Y + sign);

                sign: move -= sign;
                }
            }
        }
    }
}
