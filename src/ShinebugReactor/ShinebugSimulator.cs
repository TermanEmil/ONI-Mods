using System;

namespace ShinebugReactor
{
    [Serializable]
    public class ShinebugSimulator
    {
        public float Age;
        public float MaxAge;
        public string Name;

        public bool IsDead => Age > MaxAge;

        public void Simulate(float dt)
        {
            Age += dt;
        }
    }
}