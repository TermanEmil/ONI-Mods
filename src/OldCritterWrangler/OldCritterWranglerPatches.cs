using CaiLib.Utils;
using STRINGS;

namespace OldCritterWrangler
{
    public class OldCritterWranglerPatches
    {
        public static void OnLoad()
        {
            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Food, OldCritterWranglerConfig.Id);
            BuildingUtils.AddBuildingToTechnology(GameStrings.Technology.Food.Ranching, OldCritterWranglerConfig.Id);

            StringUtils.AddBuildingStrings(OldCritterWranglerConfig.Id, OldCritterWranglerConfig.DisplayName,
                OldCritterWranglerConfig.Description, OldCritterWranglerConfig.Effect);
            Strings.Add("STRINGS.UI.UNITSUFFIXES.CRITTERSOVERAGE", "Critters Over Age");
            Strings.Add("STRINGS.UI.SIDESCREENS.AGEDCRITTERWRANGLER.MURDERBOX.TITLE", "Age-Based Critter Wrangler");
        }
    }
}