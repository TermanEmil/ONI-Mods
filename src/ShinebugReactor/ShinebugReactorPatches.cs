using System.Diagnostics;
using CaiLib.Utils;
using Harmony;

namespace ShinebugReactor
{
    internal class ShinebugReactorPatches
    {
        public static void OnLoad()
        {
            StringUtils.AddBuildingStrings(ShinebugReactorConfig.Id, ShinebugReactorConfig.DisplayName,
                ShinebugReactorConfig.Description, ShinebugReactorConfig.Effect);
            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Power, ShinebugReactorConfig.Id);
        }

        [HarmonyPatch(typeof(Debug), "Assert", typeof(bool))]
        internal static class Debug_Assert_Patch
        {
            internal static void Prefix(bool condition)
            {
                if (!condition)
                {
                    var stack = new StackTrace();
                    if (stack.FrameCount > 2)
                    {
                        var callName = stack.GetFrame(2).GetMethod().FullDescription();
                        Debug.LogWarning(
                            $"[AssertFinder] The following method called an Assert that is about to fail:\n{callName}");
                    }
                }
            }
        }
    }
}