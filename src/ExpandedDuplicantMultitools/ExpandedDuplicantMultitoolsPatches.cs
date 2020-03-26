using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Database;
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
            Debug.Log("After create keys");
        }
    }

    //[HarmonyPatch(typeof(Strings), "Add")]
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