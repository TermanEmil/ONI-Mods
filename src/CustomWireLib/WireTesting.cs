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
                CustomBuildingMaker.CreateWireWithRating(13.37f);
                CustomBuildingMaker.CreateWireWithRating(420f);
                CustomBuildingMaker.CreateWireWithRating(9999f);
                CustomWireValues.RegisterBuildings();
            }
        }
    }
}
