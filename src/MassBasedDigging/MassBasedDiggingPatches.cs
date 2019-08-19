using Harmony;

namespace MassBasedDigging
{
    [HarmonyPatch(typeof(Workable), nameof(Workable.GetEfficiencyMultiplier))]
    internal class MassBasedDiggingPatches
    {
        public static void Postfix(ref Workable __instance, ref float __result)
        {
            if (__instance is Diggable) __result *= 1200 / Grid.Mass[Grid.PosToCell(__instance)];
        }
    }
}