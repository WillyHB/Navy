namespace Navy
{
    public static class MathUtils
    {
        #region lerp
        public static float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public static double Lerp(double firstFloat, double secondFloat, double by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public static decimal Lerp(decimal firstFloat, decimal secondFloat, decimal by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }
        #endregion
    }
}
