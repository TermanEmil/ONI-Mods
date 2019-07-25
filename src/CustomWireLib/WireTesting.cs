using Harmony;

namespace CustomWireLib
{
    class WireTesting
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class Wiretests
        {
            public static void Postfix()
            {
                CustomWireMaker.CreateWireWithRating(13.37f);
                CustomWireMaker.CreateWireWithRating(420f);
                CustomWireMaker.CreateWireWithRating(9999f);
                CustomWireValues.RegisterBuildings();
            }
        }
    }
}
