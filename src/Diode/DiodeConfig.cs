using TUNING;
using UnityEngine;

namespace Diode
{
    internal class DiodeConfig : IBuildingConfig
    {
        public const string Id = "Diode";
        public const string DisplayName = "Diode";
        public const string Description = "Diodes separate circuits and transfer power in one direction";
        public const string Effect = "TODO";
        private readonly string _anim = "diode_kanim";

        public override BuildingDef CreateBuildingDef()
        {
            var hitpoints = 30;
            var constructionTime = 10f;
            var constructionMaterials = MATERIALS.REFINED_METALS;
            var meltingPoint = 1600f;
            var buildLocationRule = BuildLocationRule.Anywhere;
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id, 2, 1, _anim, hitpoints, constructionTime,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, constructionMaterials, meltingPoint, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER1);
            buildingDef.Floodable = false;
            buildingDef.RequiresPowerInput = true;
            buildingDef.GeneratorWattageRating = float.PositiveInfinity;
            buildingDef.PowerInputOffset = CellOffset.none;
            buildingDef.PowerOutputOffset = new CellOffset(1, 0);
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PermittedRotations = PermittedRotations.R360;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefabTag)
        {
            var battery = go.AddOrGet<Battery>();
            battery.powerSortOrder = 1000;
            battery.capacity = float.PositiveInfinity;
            battery.chargeWattage = float.PositiveInfinity;
            go.AddOrGet<PowerTransformer>().powerDistributionOrder = 9;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}