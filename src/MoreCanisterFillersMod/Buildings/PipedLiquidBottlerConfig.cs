using MoreCanisterFillersMod.Components;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace MoreCanisterFillersMod.Buildings
{
    public class PipedLiquidBottlerConfig : IBuildingConfig
    {
        public const string Id          = "asquared31415.PipedLiquidBottler";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                Id,
                3,
                2,
                "gas_bottler_kanim",
                100,
                120f,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER4,
                MATERIALS.ALL_METALS,
                800f,
                BuildLocationRule.OnFloor,
                BUILDINGS.DECOR.PENALTY.TIER1,
                NOISE_POLLUTION.NOISY.TIER0
            );

            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefabTag)
        {
            var defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
            defaultStorage.showDescriptor = true;
            defaultStorage.storageFilters = STORAGEFILTERS.LIQUIDS;
            defaultStorage.capacityKg = 1000f;
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGet<AutoDropInv>();

            var conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.storage = defaultStorage;
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = defaultStorage.capacityKg;
            conduitConsumer.keepZeroMassObject = false;

            // Logic component, contains SMI
            var liquidBottler = go.AddOrGet<PipedLiquidBottler>();
            liquidBottler.Storage = defaultStorage;
            liquidBottler.workTime = 9f;
        }

        public override void DoPostConfigureComplete(GameObject go) { }
    }
}
