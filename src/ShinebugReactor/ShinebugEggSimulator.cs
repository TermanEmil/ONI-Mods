using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ShinebugReactor
{
    [Serializable]
    public class ShinebugEggSimulator
    {
        public string Name { get; set; }

        [FormerlySerializedAs("TimeToHatch")]
        public float Incubation { get; set; } = 0;

        [NonSerialized]
        public bool TimeToHatch => Incubation <= 0;

        public void Simulate(float deltaTime)
        {
            Incubation -= dt;
        }
    }
}