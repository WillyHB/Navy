using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Navy.ECS
{
    public class LightSource : Component, IDrawableComponent
    {
        public void Draw(SpriteBatch spriteBatch)
        {
            
            spriteBatch.Draw(LightMask, new Rectangle((int)Position.X - (int)Radius / 2, (int)Position.Y - (int)Radius / 2, (int)Radius, (int)Radius), new Color(Color.R, Color.G, Color.B, (int)(Intensity * 255)));
        }

        public Texture2D LightMask { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; } = Color.White;
        public float Intensity { get; set; } = 1;
        public float Radius { get; set; } = 10;
    }
}
