using Harmony;

namespace UnblockableRockets
{
    [HarmonyPatch(typeof(ConditionFlightPathIsClear), "CanReachSpace")]
    public class NoBlockPatches
    {
        public static void Postfix(ref bool __result, ref ConditionFlightPathIsClear __instance)
        {
            Traverse.Create(__instance).Field("obstructedTile").SetValue(default(int));
            __result = true;
        }
    }
}