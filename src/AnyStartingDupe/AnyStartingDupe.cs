using System.Diagnostics;
using System.Reflection;
using Harmony;

namespace AnyStartingDupe
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log($"[AnyStartingDupe] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}");
        }
    }

    [HarmonyPatch(typeof(MinionStartingStats), MethodType.Constructor, typeof(bool), typeof(string))]
    public class AnyStartingDupe
    {
        public static void Prefix(ref bool is_starter_minion)
        {
            is_starter_minion = false;
        }
    }
}