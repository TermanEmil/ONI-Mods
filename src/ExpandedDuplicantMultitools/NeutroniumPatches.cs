using System;
using Harmony;
using UnityEngine;

namespace ExpandedDuplicantMultitools
{
    public class NeutroniumPatches
    {
        [HarmonyPatch( typeof( Diggable ), "GetApproximateDigTime" )]
        public class Diggable_GetApproximateDigTime_Patches
        {
            public static void Postfix( int cell, ref float __result )
            {
                if ( __result >= float.MaxValue )
                {
                    var thisHardness = (float) Grid.Element[cell].hardness;
                    var hardnessMultiplier = thisHardness / ElementLoader.FindElementByHash( SimHashes.Ice ).hardness;
                    var num2 = 4f * (Mathf.Min( Grid.Mass[cell], 400f ) / 400f);
                    __result = num2 + hardnessMultiplier * num2;
                }
            }
        }

        [HarmonyPatch( typeof( Diggable ), "OnSolidChanged" )]
        public class Diggable_OnSolidChanged_Patches
        {
            public static void Postfix( Diggable __instance )
            {
                if ( __instance == null || __instance.gameObject == null )
                    return;

                var cell = Grid.PosToCell( __instance );
                if ( Grid.Element[cell].hardness == byte.MaxValue )
                {
                    var preconditions = __instance.chore.GetPreconditions();

                    var existingSkill = preconditions.Find( p => p.id == ChorePreconditions.instance.HasSkillPerk.id );
                    if ( existingSkill.id != default( Chore.PreconditionInstance ).id )
                    {
                        preconditions.Remove( existingSkill );
                        __instance.chore.AddPrecondition(
                            ChorePreconditions.instance.HasSkillPerk,
                            ExtraSkills.NeutroniumDiggingPerk
                        );
                    }

                    Console.WriteLine();
                    __instance.requiredSkillPerk = ExtraSkills.NeutroniumDiggingPerk.Id;
                    Traverse.Create( __instance ).Method( "UpdateStatusItem", new[] {typeof( object )} )
                            .GetValue( (object) null );
                }
            }
        }
    }
}
