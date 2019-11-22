using System;

namespace ShinebugReactor
{
    public class ShinebugEggSimulator
    {
        public float GrownLifeTime;
        public float LuxToGive;
        public float TimeToHatch;
        public string Name;

        public ShinebugEggSimulator(string name = "", float timeToHatch = 0, float grownLifeTime = 0, float lux = 0)
        {
            if (grownLifeTime <= 0)
                Debug.LogWarning(
                    "[Shinebug Reactor] Shinebug egg simulator was provided a zero or negative max timeToHatch.");

            Name = name;
            TimeToHatch = timeToHatch;
            GrownLifeTime = grownLifeTime;
            LuxToGive = lux;
        }

        public ShinebugEggSimulator(string name = "", int timeToHatch = 0, int grownLifeTime = 0, float lux = 0.0f) :
            this(name, timeToHatch * 600f, grownLifeTime * 600f, lux)
        {
        }

        public bool Simulate(float dt)
        {
            TimeToHatch -= dt;
            return TimeToHatch < 0;
        }

        public override string ToString()
        {
            return $"(FakeShinebugEgg) {Name} {TimeToHatch}s left, will give {LuxToGive} Lux for {GrownLifeTime}s";
        }
    }
}