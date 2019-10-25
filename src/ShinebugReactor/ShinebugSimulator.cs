namespace ShinebugReactor
{
    internal class ShinebugSimulator
    {
        public float Age;
        public bool IsDying;
        public float Lux;
        public float MaxAge;

        public ShinebugSimulator(float age = 0, float maxAge = 0, float lux = 0)
        {
            if (maxAge <= 0)
            {
                Debug.LogWarning("[Shinebug Reactor] Shinebug simulator was provided a zero or negative max age.");
                IsDying = true;
            }
            else
            {
                IsDying = false;
            }

            Age = age;
            MaxAge = maxAge;
            Lux = lux;
        }

        public ShinebugSimulator(int age = 0, int maxAge = 0, float lux = 0.0f) :
            this(age * 600000f, maxAge * 600000f, lux)
        {
        }

        public bool Simulate(float dt)
        {
            Age += dt;
            if (!(Age >= MaxAge)) return true;

            IsDying = true;
            Lux = 0;
            return false;
        }
    }
}