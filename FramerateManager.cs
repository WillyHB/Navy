using System;

namespace Navy
{
    public class FramerateManager
    {
        public FramerateManager(Action<int> onFrameRateSet)
        {
            OnFrameRateChanged += onFrameRateSet;
        }

        public int TargetFps { get; private set; } = 60;

        public event Action<int> OnFrameRateChanged;
        public void SetTargetFps(int fps)
        {
            TargetFps = fps;

            OnFrameRateChanged?.Invoke(fps);
        }
    }
}
