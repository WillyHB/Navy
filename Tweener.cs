using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace Navy
{
    public enum EasingType
    {
        easeLinear,
        easeInSine,
        easeOutSine,
        easeInOutSine,
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInQuart,
        easeOutQuart,
        easeInOutQuart,
        easeInQuint,
        easeOutQuint,
        easeInOutQuint,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo,
        easeInCirc,
        easeOutCirc,
        easeInOutCirc,
        easeInBack,
        easeOutBack,
        easeInOutBack,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce
    }

    public abstract class Tween
    {
        public bool started { get; internal set; }

        public float startTime { get; internal set; }
        public float time { get; internal set; }
        public float seconds { get; internal set; }
        public EasingType easingType { get; internal set; }

        public event EventHandler OnComplete, OnStart;

        public void Complete()
        {
            SetFinalValue();
            OnComplete?.Invoke(this, EventArgs.Empty);
            OnComplete = null;
        }

        public abstract void SetFinalValue();

        public void Start()
        {
            OnStart?.Invoke(this, EventArgs.Empty);
            OnStart = null;
        }
    }

    public class Tween<T> : Tween
    {
        public Func<T> getter;
        public Action<T> setter;
        public T from;
        public T to;

        public override void SetFinalValue()
        {
            setter.Invoke(to);
        }
    }


    public static class Tweener
    {
        private static Dictionary<EasingType, EaseFunction> easeFunctions = new()
        {
            { EasingType.easeLinear, (x) => x },
            { EasingType.easeInSine, EaseInSine },
            { EasingType.easeInOutSine, EaseInOutSine },
            { EasingType.easeOutSine, EaseOutSine },
            { EasingType.easeInQuad, EaseInQuad },
            { EasingType.easeOutQuad, EaseOutQuad },
            { EasingType.easeInOutQuad, EaseInOutQuad },
            { EasingType.easeInCubic, EaseInCubic },
            { EasingType.easeOutCubic, EaseOutCubic },
            { EasingType.easeInOutCubic, EaseInOutCubic },
            { EasingType.easeInQuart, EaseInQuart },
            { EasingType.easeOutQuart, EaseOutQuart },
            { EasingType.easeInOutQuart, EaseInOutQuart },
            { EasingType.easeInQuint, EaseInQuint },
            { EasingType.easeOutQuint, EaseOutQuint },
            { EasingType.easeInOutQuint, EaseInOutQuint },
            { EasingType.easeInExpo, EaseInExpo },
            { EasingType.easeOutExpo, EaseOutExpo },
            { EasingType.easeInOutExpo, EaseInOutExpo },
            { EasingType.easeInCirc, EaseInCirc },
            { EasingType.easeOutCirc, EaseOutCirc },
            { EasingType.easeInOutCirc, EaseInOutCirc },
            { EasingType.easeInBack, EaseInBack },
            { EasingType.easeOutBack, EaseOutBack },
            { EasingType.easeInOutBack, EaseInOutBack },
            { EasingType.easeInElastic, EaseInElastic },
            { EasingType.easeOutElastic, EaseOutElastic },
            { EasingType.easeInOutElastic, EaseInOutElastic },
            { EasingType.easeInBounce, EaseInBounce },
            { EasingType.easeOutBounce, EaseOutBounce },
            { EasingType.easeInOutBounce, EaseInOutBounce },
        };

        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < tweensRunning.Count; i++)
            {
                if ((tweensRunning[i].startTime -= (float)gameTime.ElapsedGameTime.TotalSeconds) > 0)
                {
                    continue;
                }

                if (!tweensRunning[i].started)
                {
                    tweensRunning[i].Start();
                    tweensRunning[i].started = true;
                }

                float percentage = tweensRunning[i].time / tweensRunning[i].seconds;
                
                float value = easeFunctions[tweensRunning[i].easingType].Invoke(float.IsNaN(percentage) ? 1 : percentage);

                // checks the type of the tweener, and then sets the needed values to the lerped value between the original and end value with the value from the easing type functions
                if (tweensRunning[i] is Tween<Color> col)
                {
                    col.setter.Invoke(new Color(MathUtils.Lerp(col.from.R, col.to.R, value) / 255, MathUtils.Lerp(col.from.G, col.to.G, value) / 255, MathUtils.Lerp(col.from.B, col.to.B, value) / 255, MathUtils.Lerp(col.from.A, col.to.A, value) / 255));
                }
                
                else if (tweensRunning[i] is Tween<Point> point)
                {
                    point.setter.Invoke(new Point((int)MathUtils.Lerp(point.from.X, point.to.X, value), (int)MathUtils.Lerp(point.from.Y, point.to.Y, value)));
                }

                else if (tweensRunning[i] is Tween<Rectangle> rect)
                {
                    rect.setter.Invoke(new Rectangle((int)MathUtils.Lerp(rect.from.X, rect.to.X, value), (int)MathUtils.Lerp(rect.from.Y, rect.to.Y, value), (int)MathUtils.Lerp(rect.from.Width, rect.to.Width, value), (int)MathUtils.Lerp(rect.from.Height, rect.to.Height, value)));
                }

                else if (tweensRunning[i] is Tween<Vector2> vec)
                {
                    vec.setter.Invoke(new Vector2(MathUtils.Lerp(vec.from.X, vec.to.X, value), MathUtils.Lerp(vec.from.Y, vec.to.Y, value)));
                }

                else if (tweensRunning[i] is Tween<decimal> dec)
                {
                    dec.setter.Invoke(MathUtils.Lerp(dec.from, dec.to, (decimal)value));
                }

                else if (tweensRunning[i] is Tween<double> d)
                {
                    d.setter.Invoke(MathUtils.Lerp(d.from, d.to, (double)value));
                }

                else if (tweensRunning[i] is Tween<int> j)
                {
                    j.setter.Invoke((int)MathUtils.Lerp(j.from, j.to, (double)value));
                }

                else if (tweensRunning[i] is Tween<float> f)
                {
                    f.setter.Invoke(MathUtils.Lerp(f.from, f.to, value));
                }

                if (tweensRunning[i].time >= tweensRunning[i].seconds)
                {
                    tweensRunning[i].Complete();
                    tweensRunning.RemoveAt(i);
                    continue;
                }

                tweensRunning[i].time = tweensRunning[i].time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }



        private delegate float EaseFunction(float x);

        public static Tween<T> TweenValue<T>(Tween<T> tween)
        {
            tweensRunning.Add(tween);
            return tween;
        }
        public static Tween<float> TweenValue(Func<float> getter, Action<float> setter, float from, float to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => AddTween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<int> TweenValue(Func<int> getter, Action<int> setter, int from, int to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => AddTween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<double> TweenValue(Func<double> getter, Action<double> setter, double from, double to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => AddTween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<decimal> TweenValue(Func<decimal> getter, Action<decimal> setter, decimal from, decimal to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => AddTween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<Vector2> TweenValue(Func<Vector2> getter, Action<Vector2> setter, Vector2 from, Vector2 to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => AddTween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<Rectangle> TweenValue(Func<Rectangle> getter, Action<Rectangle> setter, Rectangle from, Rectangle to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => AddTween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<Point> TweenValue(Func<Point> getter, Action<Point> setter, Point from, Point to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => AddTween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<Color> TweenValue(Func<Color> getter, Action<Color> setter, Color from, Color to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => AddTween(getter, setter, from, to, seconds, easingType, startTime);

        public static Tween<float> CreateTween(Func<float> getter, Action<float> setter, float from, float to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => CreateATween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<int> CreateTween(Func<int> getter, Action<int> setter, int from, int to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => CreateATween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<double> CreateTween(Func<double> getter, Action<double> setter, double from, double to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => CreateATween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<decimal> CreateTween(Func<decimal> getter, Action<decimal> setter, decimal from, decimal to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => CreateATween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<Vector2> CreateTween(Func<Vector2> getter, Action<Vector2> setter, Vector2 from, Vector2 to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => CreateATween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<Rectangle> CreateTween(Func<Rectangle> getter, Action<Rectangle> setter, Rectangle from, Rectangle to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => CreateATween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<Point> CreateTween(Func<Point> getter, Action<Point> setter, Point from, Point to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => CreateATween(getter, setter, from, to, seconds, easingType, startTime);
        public static Tween<Color> CreateTween(Func<Color> getter, Action<Color> setter, Color from, Color to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0) => CreateATween(getter, setter, from, to, seconds, easingType, startTime);
      
        private static Tween<T> AddTween<T>(Func<T> getter, Action<T> setter, T from, T to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0)
        {
            Tween<T> val = new()
            {
                startTime = startTime,
                getter = getter,
                setter = setter,
                from = from,
                to = to,
                seconds = seconds,
                easingType = easingType
            };

            for (int i = 0; i < tweensRunning.Count; i++)
            {
                if (tweensRunning[i] is Tween<T> tween)
                {
                    if (getter.Invoke().Equals(tween.getter.Invoke()))
                    {
       
                        tweensRunning[i] = val;
                        return val;
                    }
                }
            }

            tweensRunning.Add(val);

            return val;
        }

        private static Tween<T> CreateATween<T>(Func<T> getter, Action<T> setter, T from, T to, float seconds, EasingType easingType = EasingType.easeLinear, float startTime = 0)
        {
            Tween<T> val = new()
            {
                startTime = startTime,
                getter = getter,
                setter = setter,
                from = from,
                to = to,
                seconds = seconds,
                easingType = easingType
            };

            return val;
        }

        private static List<Tween> tweensRunning = new();

        #region easingFunctions
        private static float EaseInSine(float x)
        {
            return (float)(1.0f - Math.Cos((x * Math.PI) / 2.0f));
        }
        private static float EaseOutSine(float x)
        {
            return (float)Math.Sin((x * Math.PI) / 2);
        }
        private static float EaseInOutSine(float x)
        {
            return (float)(-(Math.Cos(Math.PI * x) - 1) / 2);
        }
        private static float EaseInQuad(float x)
        {
            return x * x;
        }
        private static float EaseOutQuad(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }
        private static float EaseInOutQuad(float x)
        {
            return x < 0.5 ? 2 * x * x : 1 - (float)Math.Pow(-2 * x + 2, 2) / 2;
        }
        private static float EaseInCubic(float x)
        {
            return x * x * x;
        }
        private static float EaseOutCubic(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 3);
        }
        private static float EaseInOutCubic(float x)
        {
            return x < 0.5 ? 4 * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 3) / 2;
        }
        private static float EaseInQuart(float x)
        {
            return x * x * x * x;
        }
        private static float EaseOutQuart(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 4);
        }
        private static float EaseInOutQuart(float x)
        {
            return x < 0.5 ? 8 * x * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 4) / 2;
        }
        private static float EaseInQuint(float x)
        {
            return x * x * x * x * x;
        }
        private static float EaseOutQuint(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 5);
        }
        private static float EaseInOutQuint(float x)
        {
            return x < 0.5 ? 16 * x * x * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 5) / 2;
        }
        private static float EaseInExpo(float x)
        {
            return x == 0 ? 0 : (float)Math.Pow(2, 10 * x - 10);
        }
        private static float EaseOutExpo(float x)
        {
            return x == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * x);
        }
        private static float EaseInOutExpo(float x)
        {
            return x == 0 ? 0 : x == 1 ? 1 : (x < 0.5 ? (float)Math.Pow(2, 20 * x - 10) / 2 : (2 - (float)Math.Pow(2, -20 * x + 10)) / 2);
        }
        private static float EaseInCirc(float x)
        {
            return 1 - (float)Math.Sqrt(1 - (float)Math.Pow(x, 2));
        }
        private static float EaseOutCirc(float x)
        {
            return (float)Math.Sqrt(1 - (float)Math.Pow(x - 1, 2));
        }
        private static float EaseInOutCirc(float x)
        {
            return x < 0.5 ? (1 - (float)Math.Sqrt(1 - (float)Math.Pow(2 * x, 2))) / 2 : ((float)Math.Sqrt(1 - (float)Math.Pow(-2 * x + 2, 2)) + 1) / 2;
        }
        private static float EaseInBack(float x)
        {
            return 2.70158f * x * x * x - 1.70158f * x * x;
        }
        private static float EaseOutBack(float x)
        {
            return 2.70158f * (float)Math.Pow(x - 1, 3) + 1.70158f * (float)Math.Pow(x - 1, 2);
        }
        private static float EaseInOutBack(float x)
        {
            return (float)(x < 0.5f ? ((float)Math.Pow(2 * x, 2) * ((1.70158f * 1.525 + 1) * 2 * x - 1.70158 * 1.525)) / 2 : ((float)Math.Pow(2 * x - 2, 2) * ((1.70158f * 1.525 + 1) * (x * 2 - 2) + 1.70158f * 1.525) + 2) / 2);
        }
        private static float EaseInElastic(float x)
        {
            return x == 0 ? 0 : x == 1 ? 1 : -(float)Math.Pow(2, 10 * x - 10) * (float)Math.Sin((x * 10 - 10.75) * (2 * (float)Math.PI) / 3);
        }
        private static float EaseOutElastic(float x)
        {
            return x == 0 ? 0 : (x == 1 ? 1 : (float)Math.Pow(2, -10 * x) * (float)Math.Sin((x * 10 - 0.75) * (2 * (float)Math.PI) / 3) + 1);
        }
        private static float EaseInOutElastic(float x)
        {
            float c5 = (2 * (float)Math.PI) / 4.5f;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : x < 0.5
              ? -((float)Math.Pow(2, 20 * x - 10) * (float)Math.Sin((20 * x - 11.125) * c5)) / 2
              : ((float)Math.Pow(2, -20 * x + 10) * (float)Math.Sin((20 * x - 11.125) * c5)) / 2 + 1;
            
        }
        private static float EaseInBounce(float x)
        {
            return 1 - EaseOutBounce(1 - x);
        }
        private static float EaseOutBounce(float x)
        {
            float n1 = 7.5625f;
            float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }

            else if (x < 2 / d1)
            {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }

            else if (x < 2.5 / d1)
            {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }

            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
        private static float EaseInOutBounce(float x)
        {
            return x < 0.5 ? (1 - EaseOutBounce(1 - 2 * x)) / 2 : (1 + EaseOutBounce(2 * x - 1)) / 2;
        }
        #endregion
    }
}
