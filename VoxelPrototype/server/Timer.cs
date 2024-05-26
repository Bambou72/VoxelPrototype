namespace VoxelPrototype.server
{
    public class Timer
    {
        private double lastLoopTime;
        private float timeCount;
        private int tps;
        private int tpsCount;
        public void Init()
        {
            lastLoopTime = GetTime();
        }
        public float GetDelta()
        {
            double time = GetTime();
            float delta = (float)(time - lastLoopTime);
            lastLoopTime = time;
            timeCount += delta;
            return delta;
        }
        public double GetTime()
        {
            return (double)DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
        }
        public void UpdateUPS()
        {
            tpsCount++;
        }
        public void Update()
        {
            if (timeCount > 1f)
            {
                tps = tpsCount;
                tpsCount = 0;
                timeCount -= 1;
            }
        }

        public int GetTPS()
        {
            return tps > 0 ? tps : tpsCount;
        }
        public double GetLastLoopTime()
        {
            return lastLoopTime;
        }
    }
}
