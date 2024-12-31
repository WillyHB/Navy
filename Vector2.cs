namespace Navy
{
    public abstract record Vector2<T>
    {
        public Vector2(T x, T y)
        {
            X = x;
            Y = y;
        }

        public Vector2() { }

        public T X = default;
        public T Y = default;
    }

    public record IntVector2 : Vector2<int>
    {
        public static IntVector2 Zero { get => new IntVector2(0, 0); }
        public  static IntVector2 One { get => new IntVector2(1, 1); }

        public IntVector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IntVector2(int value)
        {
            X = value;
            Y = value;
        }

        public Point ToPoint() => new Point(X, Y);
        public void ToXnaVector2() => new Vector2(X, Y);
    }
}
