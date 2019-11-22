using System;

namespace ShinebugReactor
{
    public class ShinebugSimulator
    {
        public float MaxAge;
        public float Age;
        public float Lux;
        public string Name;

        public ShinebugSimulator(string name = "", float age = 0, float maxAge = 0, float lux = 0)
        {
            if (maxAge <= 0)
                Debug.LogWarning("[Shinebug Reactor] Shinebug simulator was provided a zero or negative max age.");

            Name = name;
            Age = age;
            MaxAge = maxAge;
            Lux = lux;
        }

        public ShinebugSimulator(string name = "", int age = 0, int maxAge = 0, float lux = 0.0f) :
            this(name, age * 600f, maxAge * 600f, lux)
        {
        }

        public bool Simulate(float dt)
        {
            Age += dt;
            if (!(Age >= MaxAge)) return true;

            Lux = 0;
            return false;
        }

        public override string ToString()
        {
            return $"(FakeShinebug) {Name}: {Age}s/{MaxAge}s {Lux} Lux";
        }
    }
}