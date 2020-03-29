using CaiLib.Utils;
using EquipmentExpanded.Buildings;
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
            CaiLib.Logger.Logger.LogInit();
            
            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Stations, MultitoolAttachmentFabricatorConfig.Id);
            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Stations, CustomSuitFabricatorConfig.Id);

            LocString.CreateLocStringKeys(typeof(MULTITOOLSSTRINGS.EQUIPMENT));
            LocString.CreateLocStringKeys(typeof(MULTITOOLSSTRINGS.BUILDING));
            LocString.CreateLocStringKeys(typeof(MULTITOOLSSTRINGS.BUILDINGS));

            STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Add(GameTags.Special);
            GameTags.IgnoredMaterialCategories = new TagSet();
            GameTags.MaterialCategories.Add(GameTags.Special);
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
            
            CrossModCompatibility.CheckAndRunAll();
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