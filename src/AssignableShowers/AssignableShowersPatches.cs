using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace AssignableShowers
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[AssignableShowers] Loading mod version {FileVersionInfo.GetVersionInfo( Assembly.GetExecutingAssembly().Location ).FileVersion}"
            );
        }
    }

    internal class AssignableShowersPatches
    {
        public static AssignableSlot ShowerOwnable = new OwnableSlot( "Shower", "Shower" );

        [HarmonyPatch( typeof( MinionAssignablesProxy ), nameof( MinionAssignablesProxy.ConfigureAssignableSlots ) )]
        public class MinionAssignablesProxy_ConfigureAssignableSlots_Patch
        {
            public static void Postfix( MinionAssignablesProxy __instance, bool ___slotsConfigured )
            {
                if ( ___slotsConfigured )
                    return;

                var ownables = __instance.GetComponent<Ownables>();
                var ownableSlotInstance = new OwnableSlotInstance( ownables, (OwnableSlot) ShowerOwnable );
                ownables.Add( ownableSlotInstance );

                __instance.ownables = new List<Ownables> {__instance.gameObject.AddOrGet<Ownables>()};
            }
        }

        [HarmonyPatch( typeof( Assignable ), "get_slot" )]
        public class Assignable_get_slot_Patch
        {
            public static void Prefix( Assignable __instance, ref AssignableSlot ____slot )
            {
                if ( ____slot == null && __instance.slotID == ShowerOwnable.Id )
                    ____slot = ShowerOwnable;
            }
        }

        [HarmonyPatch( typeof( ShowerConfig ), nameof( ShowerConfig.ConfigureBuildingTemplate ) )]
        public class ShowerConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix( GameObject go )
            {
                var ownable = go.AddOrGet<Ownable>();
                ownable.slotID = ShowerOwnable.Id;
                ownable.canBePublic = true;
            }
        }
    }
}
