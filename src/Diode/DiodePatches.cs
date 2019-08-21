using System.Diagnostics;
using CaiLib.Utils;
using Harmony;
using static CaiLib.Utils.BuildingUtils;
using static CaiLib.Utils.StringUtils;

namespace Diode
{
    class DiodePatches
    {
        [HarmonyPatch(typeof(Debug), "Assert", typeof(bool))]
        internal static class Debug_Assert_Patch
        {
            internal static void Prefix(bool condition)
            {
                if (condition) return;
                var stack = new StackTrace();
                if (stack.FrameCount <= 2) return;
                var callName = stack.GetFrame(2).GetMethod().FullDescription();
                Debug.LogWarning(
                    $"[AssertFinder] The following method called an Assert that is about to fail:\n{callName}");
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                AddBuildingStrings(DiodeConfig.Id, DiodeConfig.DisplayName,
                    DiodeConfig.Description, DiodeConfig.Effect);
                AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Plumbing, DiodeConfig.Id);
            }
        }
    }
}
