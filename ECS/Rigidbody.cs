namespace Navy.ECS
{
    public class Rigidbody : Component, IUpdateableComponent
    {
        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public Vector2 Force { get; private set; } = Vector2.Zero;

        public float Friction { get; set; } = 10f;


        public void AddForce(Vector2 force)
        {
            Force += force;
        }

        public void AddForce(float x, float y)
        {
            Force += new Vector2(x, y);
        }


        public void SetForce(Vector2 force)
        {
            Force = force;
        }


        public void SetForce(float x, float y)
        {
            Force = new Vector2(x, y);
        }

        public void Update(GameTime gameTime)
        {
            gameObject.Transform.Move(Force * Friction * (float)gameTime.ElapsedGameTime.TotalSeconds);

            Force *= (1 - Friction * (float)gameTime.ElapsedGameTime.TotalSeconds);

            gameObject.Transform.Move(Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

    }
}
