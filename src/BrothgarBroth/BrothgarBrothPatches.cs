using BrothgarBroth.Entities;
using Harmony;

namespace BrothgarBroth
{
    public static class BrothgarBrothPatches
    {
        [HarmonyPatch( typeof( Edible ), "OnStopWork" )]
        public static class Edible_OnStopWork_Patches
        {
            public static void Prefix( Edible __instance, Worker worker )
            {
                if ( __instance.GetComponent<BrothgarBroth>() != null )
                {
                    // spawn phos based on how much of it is eaten
                    var element = ElementLoader.FindElementByHash( SimHashes.Phosphorus );
                    element.substance.SpawnResource(
                        Grid.CellToPosCCC( Grid.PosToCell( worker.transform.position ), Grid.SceneLayer.Ore ),
                        __instance.unitsConsumed * BrothConfig.PhosKg,
                        __instance.GetComponent<PrimaryElement>().Temperature,
                        byte.MaxValue,
                        0
                    );
                }
            }
        }
    }
}
