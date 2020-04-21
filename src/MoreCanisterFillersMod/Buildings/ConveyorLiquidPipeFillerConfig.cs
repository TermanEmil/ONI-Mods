using TUNING;
using UnityEngine;

namespace MoreCanisterFillersMod.Buildings
{
    public class ConveyorLiquidPipeFillerConfig : IBuildingConfig
    {
        public const string Id   = "asquared31415." + nameof(ConveyorLiquidPipeFillerConfig);
        public const string Anim = "conveyorout_kanim";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                Id,
                1,
                2,
                Anim,
                30,
                30f,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
                MATERIALS.ALL_METALS,
                1600f,
                BuildLocationRule.Anywhere,
                BUILDINGS.DECOR.PENALTY.TIER1,
                NOISE_POLLUTION.NONE
            );

            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
            buildingDef.InputConduitType = ConduitType.Solid;
            buildingDef.UtilityInputOffset = CellOffset.none;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
            buildingDef.PermittedRotations = PermittedRotations.R360;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefabTag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            go.AddOrGet<SolidConduitConsumer>();
            var conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.alwaysDispense = true;
            conduitDispenser.elementFilter = null;
            BuildingTemplates.CreateDefaultStorage(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
        }

        public override void DoPostConfigureComplete(GameObject go) { Prioritizable.AddRef(go); }
    }
}
