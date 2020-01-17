using System;
using UnityEngine;

namespace ShinebugReactor
{
    public class ShinebugEggSimulator
    {
        public float GrownLifeTime;
        public float LuxToGive;
        public float TimeToHatch;
        public string Name;
        public ShinebugSimulator Shinebug;
        public GameObject Item;

        public ShinebugEggSimulator(string name = "", float timeToHatch = 0, float grownLifeTime = 0, float lux = 0, ShinebugSimulator shinebug = null)
        {
            if (grownLifeTime <= 0)
                Debug.LogWarning(
                    "[Shinebug Reactor] Shinebug egg simulator was provided a zero or negative max timeToHatch.");

            Name = name;
            TimeToHatch = timeToHatch;
            GrownLifeTime = grownLifeTime;
            LuxToGive = lux;
            Shinebug = shinebug;
        }

        public ShinebugEggSimulator(string name = "", float timeToHatch = 0, float grownLifeTime = 0, float lux = 0, GameObject item = null)
        {
            if (grownLifeTime <= 0)
                Debug.LogWarning(
                    "[Shinebug Reactor] Shinebug egg simulator was provided a zero or negative max timeToHatch.");

            Name = name;
            TimeToHatch = timeToHatch;
            GrownLifeTime = grownLifeTime;
            LuxToGive = lux;
            Item = item;
        }

        /// <summary>
        /// Simulates time passing for this shinebug egg.
        /// </summary>
        /// <param name="dt">The time that has passed, in seconds</param>
        /// <returns>Whether the egg should hatch.</returns>
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