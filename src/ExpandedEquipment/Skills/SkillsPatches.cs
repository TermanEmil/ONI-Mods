using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Harmony;

namespace ExpandedEquipment.Skills
{
    public class SkillsPatches
    {
        [HarmonyPatch( typeof( MinionResume ), "CalculateTotalSkillPointsGained" )]
        public class InfSkillPoints
        {
            public static void Postfix( ref int __result ) { __result = int.MaxValue; }
        }

        public class ResourceSet_Skill_Get_Patches
        {
            private static readonly Dictionary<string, bool> HasShownWarning = new Dictionary<string, bool>();

            private static Skill MakeEmptySkill( string oldId )
            {
                return new Skill(
                    "EmptySkill",
                    "Nonexistent Skill",
                    $"This skill does not exist.\nIt had ID {oldId}",
                    0,
                    "",
                    "",
                    ""
                );
            }

            [HarmonyPatch( typeof( ResourceSet<Skill> ), "Get", typeof( string ) )]
            public class Get_string
            {
                public static bool Prefix( ResourceSet<Skill> __instance, ref Skill __result, string id )
                {
                    // If the skill exists, return it and exit
                    foreach (var skill in __instance.resources.Where(skill => skill.Id == id))
                    {
                        __result = skill;
                        return false;
                    }

                    // Otherwise, show the warning the first time
                    if ( !HasShownWarning.ContainsKey( id ) )
                    {
                        Debug.LogWarning( $"Unable to find skill {id}, returning Empty skill!" );
                        HasShownWarning[id] = true;
                    }

                    // Make the empty skill and leave
                    __result = MakeEmptySkill( id );
                    return false;
                }
            }

            [HarmonyPatch( typeof( ResourceSet<Skill> ), "Get", typeof( HashedString ) )]
            public class Get_HashedString
            {
                public static bool Prefix( ResourceSet<Skill> __instance, ref Skill __result, HashedString id )
                {
                    // If the skill exists, return it and exit
                    foreach (var skill in __instance.resources.Where(skill => new HashedString( skill.Id ) == id))
                    {
                        __result = skill;
                        return false;
                    }

                    // Otherwise, show the warning the first time
                    if ( !HasShownWarning.ContainsKey( id.ToString() ) )
                    {
                        Debug.LogWarning( $"Unable to find skill {id}, returning Empty skill!" );
                        HasShownWarning[id.ToString()] = true;
                    }

                    // Make the empty skill and leave
                    __result = MakeEmptySkill( id.ToString() );
                    return false;
                }
            }
        }

        public class GivesPerk_Patches
        {
            [HarmonyPatch( typeof( Skill ), "GivesPerk", typeof( SkillPerk ) )]
            public class GivesPerk_Perk
            {
                public static void Postfix( Skill __instance, ref bool __result )
                {
                    // Return false for all ConditionalSkills
                    if ( __instance is ConditionalSkill ) __result = false;
                }
            }

            [HarmonyPatch( typeof( Skill ), "GivesPerk", typeof( HashedString ) )]
            public class GivesPerk_HashedString
            {
                public static void Postfix( Skill __instance, ref bool __result )
                {
                    // Return false for all ConditionalSkills
                    if ( __instance is ConditionalSkill ) __result = false;
                }
            }
        }

        public class HasPerk_Patches
        {
            [HarmonyPatch( typeof( MinionResume ), "HasPerk", typeof( SkillPerk ) )]
            public class HasPerk_SkillPerk
            {
                public static void Postfix( MinionResume __instance, ref bool __result, SkillPerk perk )
                {
                    // If it already gives the perk, don't do anything
                    if (__result) return;
                    
                    // Otherwise, check every ConditionalSkill to see if it does give the perk
                    // Check every mastered skill
                    foreach ( var skillPair in __instance.MasteryBySkillID.Where( s => s.Value ) )
                    {
                        if ( Db.Get().Skills.Get( skillPair.Key ) is ConditionalSkill skill )
                        {
                            // If we find a ConditionalSkill, check it using the method
                            if (skill.GivesPerk( __instance, perk ))
                            {
                                __result = true;
                                return;
                            }
                        }
                    }
                }
            }

            [HarmonyPatch( typeof( MinionResume ), "HasPerk", typeof( HashedString ) )]
            public class HasPerk_HashedString
            {
                public static void Postfix( MinionResume __instance, ref bool __result, HashedString perkId )
                {
                    // If it already gives the perk, don't do anything
                    if (__result) return;
                    
                    // Otherwise, check every ConditionalSkill to see if it does give the perk
                    // Check every mastered skill
                    foreach ( var skillPair in __instance.MasteryBySkillID.Where( s => s.Value ) )
                    {
                        if ( Db.Get().Skills.Get( skillPair.Key ) is ConditionalSkill skill )
                        {
                            // If we find a ConditionalSkill, check it using the method
                            if (skill.GivesPerk( __instance, perkId ))
                            {
                                __result = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    
    public class StatusItemPatches
    {
        [HarmonyPatch(typeof(Workable), "UpdateStatusItem")]
        public class Workable_UpdateStatusItem_Patches
        {
            public static bool Prefix(Workable __instance, ref Guid ___workStatusItemHandle)
            {
                // If it's not a Diggable or it is not Neutronium, don't do anything special 
                if (__instance is Diggable diggable && diggable.GetTargetElement() == ElementLoader.FindElementByHash(SimHashes.Unobtanium))
                {
                    var kSelectable = diggable.GetComponent<KSelectable>();
                    if (kSelectable == null)
                        return true;
                    kSelectable.RemoveStatusItem(___workStatusItemHandle, true);
                    
                    if (diggable.worker == null)
                    {
                        if (string.IsNullOrEmpty(diggable.requiredSkillPerk))
                            return true;
                        if (!NeutroniumPerk.AnyMinionHasPerk())
                        {
                            // Add status item if nobody has perk
                            ___workStatusItemHandle = kSelectable.AddStatusItem(ExtraStatusItems.ColonyLacksNeutroniumMiner, diggable.requiredSkillPerk);
                            return false;
                        }

                        // Cancel the "Skill required dig" when there is no worker
                        return false;
                    }

                    // Cancel the "Skill required dig" when there is a worker
                    return false;
                }
                return true;
            }
        }
    }
}
