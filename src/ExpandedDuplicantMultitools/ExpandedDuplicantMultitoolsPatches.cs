using System.Diagnostics;
using System.Reflection;
using ExpandedDuplicantMultitools.Equipment;
using ExpandedDuplicantMultitools.Skills;
using Harmony;

namespace ExpandedDuplicantMultitools
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[ExpandedMultitools] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );

            LocString.CreateLocStringKeys(typeof(MULTITOOLSSTRINGS.EQUIPMENT));
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public class Db_Initialize_Patches
    {
        public static void Postfix()
        {
            ExtraSkills.InitializeSkillsAndPerks();
            ExpandedAssignableSlots.InitializeSlots();
        }
    }

    [HarmonyPatch(typeof(GeneratedEquipment), "LoadGeneratedEquipment")]
    public class GeneratedEquipment_LoadGeneratedEquipment_Patches
    {
        public static void Postfix()
        {
            ExpandedAssignableSlots.LoadAllEquipment();
        }
    }
}