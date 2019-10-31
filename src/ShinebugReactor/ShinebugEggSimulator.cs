using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STRINGS;

namespace ShinebugReactor
{
    class ShinebugEggSimulator
    {
        public float TimeToHatch;
        public float GrownLifeTime;
        public float LuxToGive;

        public ShinebugEggSimulator(float timeToHatch = 0, float grownLifeTime = 0, float lux = 0)
        {
            if (grownLifeTime <= 0)
            {
                Debug.LogWarning("[Shinebug Reactor] Shinebug egg simulator was provided a zero or negative max timeToHatch.");
            }

            TimeToHatch = timeToHatch;
            GrownLifeTime = grownLifeTime;
            LuxToGive = lux;
        }

        public ShinebugEggSimulator(int timeToHatch = 0, int grownLifeTime = 0, float lux = 0.0f) :
            this(timeToHatch * 600f, grownLifeTime * 600f, lux)
        {
        }

        public bool Simulate(float dt)
        {
            TimeToHatch -= dt;
            if (!(TimeToHatch < 0)) return false;

            //Debug.Log($"Hatching {this}");
            return true;
        }

        public override string ToString()
        {
            return $"(FakeShinebugEgg) {TimeToHatch}s left, will give {LuxToGive} Lux for {GrownLifeTime}s";
        }
    }
}
