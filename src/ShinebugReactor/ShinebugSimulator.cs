using UnityEngine;

namespace ShinebugReactor
{
    public class ShinebugSimulator
    {
        public float Age;
        public ShinebugEggSimulator Egg;
        public GameObject Item;
        public float Lux;
        public float MaxAge;
        public string Name;

        public ShinebugSimulator(string name = "", float age = 0, float maxAge = 0, float lux = 0,
            ShinebugEggSimulator egg = null, GameObject item = null)
        {
            if (maxAge <= 0)
                Debug.LogWarning("[Shinebug Reactor] Shinebug simulator was provided a zero or negative max age.");

            Name = name;
            Age = age;
            MaxAge = maxAge;
            Lux = lux;
            Egg = egg;
            Item = item;
        }

        /// <summary>
        ///     Simulates time passing for this shinebug.
        /// </summary>
        /// <param name="dt">The amount of time that has passed, in seconds.</param>
        /// <returns>Whether this shinebug should die and produce an egg.</returns>
        public bool Simulate(float dt)
        {
            Age += dt;
            if (Age <= MaxAge) return false;

            Lux = 0;
            return true;
        }

        public override string ToString()
        {
            return $"(FakeShinebug) {Name}: {Age}s/{MaxAge}s {Lux} Lux";
        }
    }
}