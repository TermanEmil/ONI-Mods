using CaiLib.Utils;

namespace SpotHeater
{
    public class SpotHeaterPatches
    {
        public static void OnLoad()
        {
            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, SpotHeaterConfig.Id);
            BuildingUtils.AddBuildingToTechnology(GameStrings.Technology.Gases.TemperatureModulation,
                SpotHeaterConfig.Id);
            StringUtils.AddBuildingStrings(SpotHeaterConfig.Id, SpotHeaterConfig.DisplayName,
                SpotHeaterConfig.Description, SpotHeaterConfig.Effect);
        }
    }
}