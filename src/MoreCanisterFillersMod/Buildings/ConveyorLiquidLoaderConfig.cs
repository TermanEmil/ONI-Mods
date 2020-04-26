using TUNING;
using UnityEngine;

namespace MoreCanisterFillersMod.Buildings
{
    public class ConveyorLiquidLoaderConfig : IBuildingConfig
    {
        public const string Id   = "asquared31415." + nameof(ConveyorLiquidLoaderConfig);
        public const string Anim = "conveyorin_kanim";

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
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = CellOffset.none;
            buildingDef.OutputConduitType = ConduitType.Solid;
            buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(0, 1);
            buildingDef.EnergyConsumptionWhenActive = 60f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.06f;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefabTag)
        {
            GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            var conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.consumptionRate = 10f;
            conduitConsumer.capacityKG = 20f;
            conduitConsumer.forceAlwaysSatisfied = true;
            var conduitDispenser = go.AddOrGet<SolidConduitDispenser>();
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
