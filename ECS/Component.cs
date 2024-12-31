using Navy.UI;

namespace Navy.ECS
{
    public interface IDrawableComponent
    {
        public void Draw(SpriteBatch spriteBatch);
    }

    public interface IUpdateableComponent
    {
        public void Update(GameTime gameTime);
    }

    public abstract class Component
    {
        public virtual void Destroy() { }

        public virtual void Initialize() { }

        public bool Enabled
        {
            get
            {
                return enabled && gameObject.Enabled;
            }

            set
            {
                enabled = value;

                if (this is Script script)
                {
                    if (value) script.OnEnable();
                    else script.OnDisable();
                }

                else if (this is Canvas canvas)
                {
                    foreach (var element in canvas.GetFullElementArray())
                    {
                        element.Enabled = value;
                    }
                }
            }
        }

        private bool enabled = true;

        public GameObject gameObject;
    }




    
}
