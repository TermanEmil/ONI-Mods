using UnityEngine;

namespace ShinebugReactor
{
    public static class ShinebugPowerMath
    {
        public static float ComputeShinebugWattage(float luxSum)
        {
            const float watsPerLux = 0.00159f;
            const float maxWattage = 1250.0f;

            var potentialWattage = luxSum * watsPerLux;
            return Mathf.Clamp(potentialWattage, 0.0f, maxWattage);
        }
    }
}
