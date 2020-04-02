using CaiLib.Utils;

namespace ClothingLocker
{
    public class ClothingLockerPatches
    {
        public static void OnLoad()
        {
            BuildingUtils.AddBuildingToPlanScreen( GameStrings.PlanMenuCategory.Base, ClothingLockerConfig.Id );
            StringUtils.AddBuildingStrings(
                ClothingLockerConfig.Id,
                ClothingLockerConfig.Name,
                ClothingLockerConfig.Desc,
                ClothingLockerConfig.Effect
            );
        }
    }
}
