using System.Diagnostics;
using System.Reflection;
using EquipmentExpanded.Equipment;
using EquipmentExpanded.Skills;
using Harmony;
using TUNING;

namespace EquipmentExpanded
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[EquipmentExpanded] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );

            LocString.CreateLocStringKeys(typeof(MULTITOOLSSTRINGS.EQUIPMENT));
            LocString.CreateLocStringKeys(typeof(MULTITOOLSSTRINGS.BUILDING));

            STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Add(GameTags.Special);
            GameTags.IgnoredMaterialCategories = new TagSet();
        }
    }

    [HarmonyPatch(typeof(Db), "Initialize")]
    public class Db_Initialize_Patches
    {
        public static void Postfix()
        {
            ExtraSkills.InitializeSkillsAndPerks();
            ExpandedAssignableSlots.InitializeSlots();
            ExtraStatusItems.InitializeStatusItems();
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