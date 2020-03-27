using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ExpandedEquipment.Skills;
using Harmony;
using UnityEngine;

namespace ExpandedEquipment
{
    public class NeutroniumPatches
    {
        [HarmonyPatch( typeof( Diggable ), "GetApproximateDigTime" )]
        public class Diggable_GetApproximateDigTime_Patches
        {
            public static void Postfix( int cell, ref float __result )
            {
                // Reimplement the method if it would return MaxValue or +Inf
                // This means it was a neutronium tile
                // TODO: Possibly increase the time multiplier?
                if ( __result >= float.MaxValue )
                {
                    var thisHardness = (float) Grid.Element[cell].hardness;
                    var hardnessMultiplier = thisHardness / ElementLoader.FindElementByHash( SimHashes.Ice ).hardness;
                    var num2 = 4f * (Mathf.Min( Grid.Mass[cell], 400f ) / 400f);
                    __result = num2 + hardnessMultiplier * num2;
                }
            }
        }

        [HarmonyPatch(typeof(Diggable), "UpdateColor")]
        public class Diggable_UpdateColor_Patches
        {
            private static readonly MethodInfo RequiresTool = AccessTools.Method(typeof(Diggable), "RequiresTool"); 
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();
                for (var i = 0; i < codes.Count; ++i)
                {
                    if (codes[i].operand as MethodInfo == RequiresTool)
                    {
                        // Create new label and put it on the instruction after next ret
                        var label = new Label();
                        codes[codes.FindIndex(i, ci => ci.opcode == OpCodes.Ret) + 1].labels.Add(label);
                        // Go 5 instructions back, and branch to that label
                        codes.Insert(i - 5, new CodeInstruction(OpCodes.Br, label));
                        return codes;
                    }
                }
                
                Debug.LogWarning("[Equipment Expanded] Unable to patch Diggable.UpdateColor to ignore neutronium checks");
                return codes;
            }
        }

        [HarmonyPatch( typeof( Diggable ), "OnSolidChanged" )]
        public class Diggable_OnSolidChanged_Patches
        {
            public static void Postfix( Diggable __instance )
            {
                if ( __instance == null || __instance.gameObject == null )
                    return;
                
                // Check if cell is at bottom of map
                var cell = Grid.PosToCell(__instance);
                var travInstance = Traverse.Create(__instance);
                if (cell < Grid.WidthInCells)
                {
                    // We are within the bottom row, cancel this
                    travInstance.Method("OnCancel").GetValue();
                }
                
                // Only Neutronium (or other unminable tiles if implemented?) have a hardness of 255
                if (Grid.Element[cell].hardness != byte.MaxValue) return;
                
                // Get the existing preconditions and find one that checks for a skill perk
                var preconditions = __instance.chore.GetPreconditions();
                var existingSkill = preconditions.Find( p => p.id == ChorePreconditions.instance.HasSkillPerk.id );
                
                // If the skill perk is checking for super hard digging (hardness >= 200)
                if ( existingSkill.data == Db.Get().SkillPerks.CanDigSupersuperhard )
                {
                    // Remove it and add our digging
                    preconditions.Remove( existingSkill );
                    __instance.chore.AddPrecondition(
                        ChorePreconditions.instance.HasSkillPerk,
                        ExtraSkills.NeutroniumDiggingPerk
                    );
                }
                
                // Update the status item with the new required skill
                __instance.requiredSkillPerk = ExtraSkills.NeutroniumDiggingPerk.Id;
                travInstance.Method( "UpdateStatusItem", new[] {typeof( object )} )
                    .GetValue( (object) null );
            }
        }
    }
}
