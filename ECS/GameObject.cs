using System.Collections.Generic;
using Navy.UI;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace Navy.ECS
{
    public class GameObject
    {
        public GameObject()
        {
            AddComponent<Transform>();
        }

        public GameObject(Vector2 position)
        {
            AddComponent<Transform>().Position = position;
        }

        public Transform Transform => GetComponent<Transform>();

        public GameObject Parent
        {
            get => parent;
            set
            {
                    
                parent?.children.Remove(this);

                parent = value;
                parent.children.Add(this);
                if (IsInstantiated)
                {
                    Level.CurrentLevel.GameObjects.RemoveAt(Level.CurrentLevel.GameObjects.IndexOf(this));
                    Level.CurrentLevel.GameObjects.Insert(Level.CurrentLevel.GameObjects.IndexOf(parent) + 1, this);
                }
            }
        }

        private readonly List<GameObject> children = [];
        private GameObject parent;

        public string Tag { get; set; } = "";

        public string Layer { get; set; } = "";
        public bool Enabled
        {
            get => IsInstantiated && enabled && (Parent?.Enabled ?? true);
            set => enabled = value;
        }

        private bool enabled = true;
        public bool DestroyOnLoad { get; set; } = true;

        public bool IsInstantiated { get; private set; } = false;

        public event EventHandler Destroyed;
        public event EventHandler Instantiated;


        public static GameObject FindTag(string tag)
        {
            foreach (GameObject go in Level.CurrentLevel.GameObjects)
            {
                if (go.Tag == tag) return go;
            }

            return null;
        }

        public static GameObject[] FindTags(string tag)
        {
            List<GameObject> gameObjects = [];

            foreach (GameObject go in Level.CurrentLevel.GameObjects)
            {
                if (go.Tag == tag)
                {
                    gameObjects.Add(go);
                }
            }

            return [..gameObjects];
        }

        /// <summary>
        /// Returns the first found component of type T in the current level
        /// </summary>   
        /// 
        public static T FindComponent<T>() where T : Component
        {
            foreach (GameObject go in Level.CurrentLevel.GameObjects)
            {
                foreach (Component component in go.components)
                {
                    if (component is T comp)
                    {
                        return comp;
                    }
                }
            }

            return default;
        }

        /// <summary>
        /// Returns all components of type T found in the current level
        /// </summary>
        /// 
        public static T[] FindComponents<T>() where T : Component
        {
            List<T> returnT = [];

            foreach (GameObject go in Level.CurrentLevel.GameObjects)
            {
                foreach (Component component in go.components)
                {

                    if (component is T comp)
                    {
                        returnT.Add(comp);
                    }
                }
            }

            return [.. returnT];
        }

        public void Update(GameTime gameTime)
        {
            foreach (Component component in components)
            {
                if (component is Script script)
                {
                    script.Update(gameTime);
                }

                if (component.Enabled)
                {
                    if (component is IUpdateableComponent comp)
                    {
                        comp.Update(gameTime);

                    }
                }
            }
        }
        public void Destroy()
        {
            while (components.Count > 0)
            {
                components[0].Destroy();
                components.RemoveAt(0);
            }

            IsInstantiated = false;
            Destroyed?.Invoke(this, EventArgs.Empty);
            Level.CurrentLevel.GameObjects.Remove(this);

            Destroyed = null;
            Instantiated = null;

        }

        public GameObject Instantiate()
        {
            if (!IsInstantiated)
            {
                Instantiated?.Invoke(this, EventArgs.Empty);
                IsInstantiated = true;

                foreach (Script script in GetComponents<Script>())
                {
                    script.Start();
                }

                Level.CurrentLevel.GameObjects.Add(this);
            }

            return this;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (Component component in components)
            {
                if (component.Enabled)
                {
                    if (component is IDrawableComponent comp && comp is not Canvas or LightSource)
                    {
                        comp.Draw(spriteBatch);
                    }
                }
            }
        }


        #region components


        private readonly List<Component> components = [];

        public T AddComponent<T>() where T : Component => AddComponent((T)Activator.CreateInstance(typeof(T)));

        public T AddComponent<T>(T instance) where T : Component
        {

            foreach (Component component in components)
            {
                if (component is T comp && comp is not Script)
                {
                    Debug.WriteLine($"Object Already Has A Component Of Type {comp.GetType()}");
                    return comp;
                }
            }

            instance.gameObject = this;

            if (instance is Script script)
            {
                if (IsInstantiated)
                {

                    script.Start();
                }
            }

            instance.Initialize();

            components.Add(instance);
            return instance;
        }

        public void RemoveComponent<T>() where T : Component
        {
            if (typeof(T) != typeof(Transform))
            {
                foreach (Component component in components)
                {
                    if (component is T)
                    {
                        components.Remove(component);
                        return;
                    }
                }
            }
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component is T t)
                {
                    return t;
                }
            }

            return null;
        }

        public T[] GetComponents<T>() where T : Component
        {
            List<T> ts = [];
            foreach (Component component in components)
            {
                if (component is T t)
                {
                    ts.Add(t);
                }
            }

            return [.. ts];
        }

        public bool TryGetComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component is T)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
