using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Navy.ECS;

namespace Navy
{
    public class Camera
    {
        public Camera()
        {

        }

        public Vector2 Position { get; set; }
        public float Zoom { get => zoom; set => zoom = value > 0.1f ? value : 0.1f; }
        private float zoom = 1;
        public float Rotation { get; set; }

        public void Move(float x, float y)
        {
            MoveX(x);
            MoveY(y);
        }

        public void Move(float amount)
        {
            MoveX(amount);
            MoveY(amount);
        }

        public void Move(Vector2 amount)
        {
            MoveX(amount.X);
            MoveY(amount.Y);
        }

        public void MoveX(float x)
        {
            Position = new Vector2(Position.X + x, Position.Y);
        }

        public void MoveY(float y)
        {
            Position = new Vector2(Position.X, Position.Y + y);
        }

        public void SetPositionSmooth(Transform transform, float smoothValue = 0.5f)
        {
            Position = new Vector2(MathHelper.Lerp(Position.X, transform.Position.X, smoothValue), MathHelper.Lerp(Position.Y, transform.Position.Y, smoothValue));
        }

        public void SetPositionSmooth(Transform transform, float smoothValue, Vector2 mapSize)
        {
            Position = 
                new Vector2(MathHelper.Lerp(Position.X, (transform.Position.X + ResolutionHandler.GetVirtualResolution().X/2 < mapSize.X * Globals.GameScale ? (transform.Position.X - ResolutionHandler.GetVirtualResolution().X / 2 > 0 ? transform.Position.X : 0 + ResolutionHandler.GetVirtualResolution().X/2) : mapSize.X * Globals.GameScale - ResolutionHandler.GetVirtualResolution().X/2), smoothValue),
                            MathHelper.Lerp(Position.Y, (transform.Position.Y + ResolutionHandler.GetVirtualResolution().Y / 2 < mapSize.Y * Globals.GameScale ? (transform.Position.Y - ResolutionHandler.GetVirtualResolution().Y / 2 > 0 ? transform.Position.Y : 0 + ResolutionHandler.GetVirtualResolution().Y / 2) : mapSize.Y * Globals.GameScale - ResolutionHandler.GetVirtualResolution().Y / 2), smoothValue));
        }

        public Matrix GetViewMatrix()
        {
            return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(ResolutionHandler.GetVirtualResolution().X / 2, ResolutionHandler.GetVirtualResolution().Y / 2, 0));
        }

        public Matrix GetInverseMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, GetViewMatrix());
        }

        // FIX THIS NO WORKING?!?!?!?!?!?!??!?!?!?!??!/
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, GetInverseMatrix());
        }


    }
}
