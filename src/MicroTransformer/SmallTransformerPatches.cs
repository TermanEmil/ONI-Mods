using CaiLib.Utils;

namespace MicroTransformer
{
    public class SmallTransformerPatches
    {
        public static void OnLoad()
        {
            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Power, SmallTransformerConfig.Id);
            BuildingUtils.AddBuildingToTechnology(GameStrings.Technology.Food.Ranching, SmallTransformerConfig.Id);
            StringUtils.AddBuildingStrings(SmallTransformerConfig.Id, SmallTransformerConfig.DisplayName,
                SmallTransformerConfig.Description, SmallTransformerConfig.Effect);
        }
    }
}