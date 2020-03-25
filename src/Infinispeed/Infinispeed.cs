using System.Diagnostics;
using System.Reflection;
using Harmony;

namespace Infinispeed
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[Infinispeed] Loading mod version {FileVersionInfo.GetVersionInfo( Assembly.GetExecutingAssembly().Location ).FileVersion}"
            );
        }
    }

    [HarmonyPatch( typeof( Workable ), nameof( Workable.GetEfficiencyMultiplier ) )]
    internal class Infinispeed
    {
        public static void Postfix( ref Workable __instance, ref float __result )
        {
            if ( __instance is Edible )
                return;

            __result = float.PositiveInfinity;
        }
    }
}
