namespace RealisticValues
{
    public static class Util
    {
        public static class Energy
        {
            public static float GetTargetTemperature(
                SimHashes inElement,
                float inMass,
                float inTemp,
                SimHashes outElement,
                float outMass
            )
            {
                var joules = ElementLoader.FindElementByHash(inElement).specificHeatCapacity *
                             inMass *
                             (inTemp - Constants.CELSIUS2KELVIN);

                return joules / outMass / ElementLoader.FindElementByHash(outElement).specificHeatCapacity + Constants.CELSIUS2KELVIN;
            }
        }
    }
}
