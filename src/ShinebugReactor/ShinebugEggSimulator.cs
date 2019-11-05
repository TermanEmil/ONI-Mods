namespace ShinebugReactor
{
    internal class ShinebugEggSimulator
    {
        public float GrownLifeTime;
        public float LuxToGive;
        private float _timeToHatch;

        public ShinebugEggSimulator(float timeToHatch = 0, float grownLifeTime = 0, float lux = 0)
        {
            if (grownLifeTime <= 0)
                Debug.LogWarning(
                    "[Shinebug Reactor] Shinebug egg simulator was provided a zero or negative max timeToHatch.");

            _timeToHatch = timeToHatch;
            GrownLifeTime = grownLifeTime;
            LuxToGive = lux;
        }

        public ShinebugEggSimulator(int timeToHatch = 0, int grownLifeTime = 0, float lux = 0.0f) :
            this(timeToHatch * 600f, grownLifeTime * 600f, lux)
        {
        }

        public bool Simulate(float dt)
        {
            _timeToHatch -= dt;
            return _timeToHatch < 0;

            //Debug.Log($"Hatching {this}");
        }

        public override string ToString()
        {
            return $"(FakeShinebugEgg) {_timeToHatch}s left, will give {LuxToGive} Lux for {GrownLifeTime}s";
        }
    }
}