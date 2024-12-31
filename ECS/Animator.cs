using System.Collections.Generic;
using System;

namespace Navy.ECS
{
    public class Animator : Component, IUpdateableComponent
    {
        public Animator Play(Animation animation, bool isLooping)
        {
            CurrentAnimation = animation;
            CurrentAnimation.CurrentFrameIndex = 0;
            CurrentAnimation.updateValue = 0;
            CurrentAnimation.isLooping = isLooping;
            CurrentAnimation.Play();

            return this;
        }

        public void Stop()
        {
            gameObject.GetComponent<SpriteRenderer>().TextureRect = gameObject.GetComponent<SpriteRenderer>().Rect;
            CurrentAnimation.Stop();
        }

        public void Pause()
        {
            CurrentAnimation.Pause();
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentAnimation != null)
            {
                CurrentAnimation.Update(gameTime);
            }

            if (gameObject.TryGetComponent<SpriteRenderer>())
            {
                gameObject.GetComponent<SpriteRenderer>().TextureRect = CurrentAnimation == null ? gameObject.GetComponent<SpriteRenderer>().Texture.Bounds : CurrentAnimation.CurrentFrame;
            }

        }

        public Animation CurrentAnimation { get; private set; }
    }

    public class Animation
    {
        public Animation(float speed)
        {
            Speed = speed;
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void Stop()
        {
            updateValue = 0;
            isPlaying = false;
            currentFrameIndex = 0;
        }

        public void Pause()
        {
            isPlaying = false;
        }

        internal bool isLooping = false;
        internal bool isPlaying;
        internal float updateValue;

        public void Update(GameTime gameTime)
        {
            if (isPlaying)
            {
                updateValue += (float)gameTime.ElapsedGameTime.TotalSeconds * Speed;

                if (updateValue > 100)
                {
                    updateValue = 0;

                    CurrentFrameIndex++;

                    if (CurrentFrameIndex > frames.Count - 1)
                    {
                        if (isLooping)
                        {
                            CurrentFrameIndex = 0;
                            FrameEvent?.Invoke(this, CurrentFrameIndex);

                            return;
                        }

                        else
                        {
                            Stop();

                            /*
                             * 
                             * Final frame?
                             * */
                        }
                    }

                    CurrentFrame = frames[CurrentFrameIndex];


                    FrameEvent?.Invoke(this, CurrentFrameIndex);

                    return;

                }
            }
        }

        public float Speed { get; set; }

        private int currentFrameIndex = 0;

        public int CurrentFrameIndex
        {
            get
            {
                return currentFrameIndex;
            }
            set
            {
                if (value < frames.Count)
                {
                    CurrentFrame = frames[value];
                }

                currentFrameIndex = value;
            }

        }

        public Rectangle CurrentFrame { get; set; } = Rectangle.Empty;

        public Animation AddFrame(Rectangle rectangle)
        {
            frames.Add(rectangle);
            return this;
        }

        public Animation AddFrame(int yIndex, int xIndex, Vector2 size)
        {
            AddFrame(new Rectangle((int)(xIndex * size.X), (int)(yIndex * size.Y), (int)size.X, (int)size.Y));
            return this;
        }



        public void RemoveFrame(Rectangle rectangle)
        {
            frames.Remove(rectangle);
        }

        public Rectangle GetFrame(int index)
        {
            return frames[index];
        }

        public Animation AddStripFrames(int yIndex, int spriteNum, Vector2 size)
        {
            for (int i = 0; i < spriteNum; i++)
            {
                frames.Add(new Rectangle(i * (int)size.X, yIndex * (int)size.Y, (int)size.X, (int)size.Y));
            }

            return this;
        }

        private readonly List<Rectangle> frames = new List<Rectangle>();

        public event EventHandler<int> FrameEvent;
    }
}
