using System.Diagnostics;
using System.Reflection;
using Harmony;

namespace UnblockableRockets
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[UnblockableRockets] Loading mod version {FileVersionInfo.GetVersionInfo( Assembly.GetExecutingAssembly().Location ).FileVersion}"
            );
        }
    }

    [HarmonyPatch( typeof( ConditionFlightPathIsClear ), "CanReachSpace" )]
    public class NoBlockPatches
    {
        public static void Postfix( ref bool __result, ref ConditionFlightPathIsClear __instance )
        {
            Traverse.Create( __instance ).Field( "obstructedTile" ).SetValue( default( int ) );
            __result = true;
        }
    }
}
