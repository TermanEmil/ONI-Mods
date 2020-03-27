using System.Diagnostics;
using System.Reflection;
using ExpandedEquipment.Equipment;
using ExpandedEquipment.Skills;
using Harmony;

namespace ExpandedEquipment
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[ExpandedEquipment] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );

            LocString.CreateLocStringKeys(typeof(MULTITOOLSSTRINGS.EQUIPMENT));
        }
    }

    [HarmonyPatch(typeof(KSelectable), "AddStatusItem")]
    public class test
    {
        public static void Prefix(object data)
        {
            Debug.Log(data);
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