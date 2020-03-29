using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EquipmentExpanded.Skills;
using Harmony;
using UnityEngine;

namespace EquipmentExpanded
{
    
    [HarmonyPatch(typeof(MinionConfig), "SetupLaserEffects")]
    public class MinionConfig_SetupLaserEffects
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> orig)
        {
            var codes = orig.ToList();
            var idx = codes.Count - 1;
            codes.Insert(idx++, new CodeInstruction(OpCodes.Ldloc_2));
            codes.Insert(idx++, new CodeInstruction(OpCodes.Ldloc_0));
            codes.Insert(idx++, new CodeInstruction(OpCodes.Ldloc_1));
            var addAnim = typeof(MinionConfig_SetupLaserEffects).GetMethod("AddAnim");
            codes.Insert(idx, new CodeInstruction(OpCodes.Call, addAnim));

            return codes;
        }
        
        public static void AddAnim(KBatchedAnimController animController,
            GameObject gameObject,
            KBatchedAnimEventToggler toggler)
        {
            var laserEffect = new MinionConfig.LaserEffect
            {
                id = "NeutroniumDigEffect",
                animFile = "neutronium_miner_beam_fx_kanim",
                anim = "idle",
                context = (HashedString) "neutroniumdig"
            };

            var go = new GameObject(laserEffect.id);
            go.transform.parent = gameObject.transform;
            go.AddOrGet<KPrefabID>().PrefabTag = new Tag(laserEffect.id);
            var animTracker = go.AddOrGet<KBatchedAnimTracker>();
            animTracker.controller = animController;
            animTracker.symbol = new HashedString("snapTo_rgtHand");
            animTracker.offset = new Vector3(195f, -35f, 0.0f);
            animTracker.useTargetPoint = true;
            var kbatchedAnimController = go.AddOrGet<KBatchedAnimController>();
            kbatchedAnimController.AnimFiles = new[]
            {
                Assets.GetAnim(laserEffect.animFile)
            };
            var entry = new KBatchedAnimEventToggler.Entry
            {
                anim = laserEffect.anim,
                context = laserEffect.context,
                controller = kbatchedAnimController
            };
            toggler.entries.Add(entry);
            go.AddOrGet<LoopingSounds>();
        }
    }
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

            public static void Postfix(Diggable __instance)
            {
                if (Grid.Element[Grid.PosToCell(__instance.gameObject)].hardness == byte.MaxValue)
                {
                    var inst = Traverse.Create(__instance);
                    var childRenderer = (MeshRenderer) inst.Field("childRenderer").GetValue();
                    if (childRenderer == null) return;
                    childRenderer.material.color = Game.Instance.uiColours.Dig.validLocation;
                    inst.Field("multitoolContext").SetValue((HashedString) "neutroniumdig");
                    inst.Field("multitoolHitEffectTag").SetValue((Tag) "fx_dig_splash");
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
