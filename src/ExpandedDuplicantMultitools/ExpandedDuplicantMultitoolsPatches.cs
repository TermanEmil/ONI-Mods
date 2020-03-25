using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Database;
using Harmony;

namespace ExpandedDuplicantMultitools
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[ExpandedMultitools] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}");
            
            LocString.CreateLocStringKeys( typeof( MULTITOOLSSTRINGS.EQUIPMENT ) );
            Debug.Log("After create keys");
        }
    }

    [HarmonyPatch(typeof(Strings), "Add")]
    public class tests
    {
        public static void Postfix(params string[] value)
        {
            foreach (var s in value)
            {
                Debug.Log(s);
            }
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public class Db_Initialize_Patches
    {
        public static void Postfix()
        {
            ExtraSkills.InitializeSkillsAndPerks();
            Equipment.ExpandedAssignableSlots.InitializeSlots();
        }
    }

    [HarmonyPatch(typeof(GeneratedEquipment), "LoadGeneratedEquipment")]
    public class GeneratedEquipment_LoadGeneratedEquipment_Patches
    {
        public static void Postfix()
        {
            Equipment.ExpandedAssignableSlots.LoadAllEquipment();
        }
    }
    
    public static class ExtraSkills
    {
        private static Skill NeutroniumDigging;
        public static SkillPerk NeutroniumDiggingPerk;
        
        public const string NeutroniumDiggingSkillId = "asquared31415_" + nameof(NeutroniumDigging);
        public const string NeutroniumDiggingPerkId = "asquared31415_" + nameof(NeutroniumDiggingPerk);

        public static void InitializeSkillsAndPerks()
        {
            var skillPerks = Db.Get().SkillPerks;
            var skills = Db.Get().Skills;
            NeutroniumDiggingPerk = skillPerks.Add(new SimpleSkillPerk(NeutroniumDiggingPerkId,
                MULTITOOLSSTRINGS.SKILLPERKS.NEUTRONIUM_DIGGING.DESCRIPTION));

            NeutroniumDigging = skills.Add(new ConditionalSkill(
                NeutroniumDiggingSkillId,
                MULTITOOLSSTRINGS.SKILLS.NEUTRONIUM_DIGGING.NAME,
                MULTITOOLSSTRINGS.SKILLS.NEUTRONIUM_DIGGING.DESCRIPTION,
                3,
                "hat_role_mining3",
                "skillbadge_role_mining3",
                Db.Get().SkillGroups.Mining.Id,
                (resume, skill) => { return resume.CurrentHat == "hat_role_mining3"; },
                (resume, skillId) => { return resume.CurrentHat == "hat_role_mining3"; }
            ));
            NeutroniumDigging.priorSkills = new List<string>()
            {
                skills.Mining3.Id
            };
            NeutroniumDigging.perks = new List<SkillPerk>()
            {
                NeutroniumDiggingPerk
            };
        }
    }
}