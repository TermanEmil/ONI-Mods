using UnityEngine;

namespace EdibleDuplicants
{
    class ButcherTableConfig : IBuildingConfig
    {
        public const string Id = "ButcherTable";
        public static string DisplayName = "Butcher Table";
        public static string Description = "Butchers the corpses of dead duplicants." +
            "  Duplicants that perform \"work\" here will definitely not have fun, but sometimes, you need to eat.";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id, 3, 2, "clothingfactory_kanim", 100, 120, new[] { 100f }, TUNING.MATERIALS.ALL_METALS, 473.15f, BuildLocationRule.OnFloor, TUNING.DECOR.PENALTY.TIER2, TUNING.NOISE_POLLUTION.NOISY.TIER0);
            buildingDef.AudioCategory = "Metal";
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            var storage = go.AddOrGet<Storage>();
            storage.showInUI = false;
        }
    }
}
