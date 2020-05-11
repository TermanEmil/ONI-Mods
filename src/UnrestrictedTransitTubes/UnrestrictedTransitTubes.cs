using System.Diagnostics;
using System.Reflection;
using Harmony;

namespace UnrestrictedTransitTubes
{
    public class Hooks
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[UnrestrictedTransitTubes] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );
        }
    }

    [HarmonyPatch(typeof(UtilityNetworkTubesManager), nameof(UtilityNetworkTubesManager.CanAddConnection))]
    public class Patch
    {
        public static void Postfix(ref bool __result) { __result = true; }
    }
}
