using Harmony;

namespace Infinispeed
{
    [HarmonyPatch(typeof(Workable), nameof(Workable.GetEfficiencyMultiplier))]
    internal class Infinispeed
    {
        public static void Postfix(ref Workable __instance, ref float __result)
        {
            if (__instance is Edible) return;
            __result = float.PositiveInfinity;
        }
    }
}