using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace MoreCanisterFillersMod
{
    public class OldPipedLiquidBottlerConfig : PipedLiquidBottlerConfig
    {
        private const string Id = "PipedLiquidBottler";

        public override BuildingDef CreateBuildingDef()
        {
            var def = base.CreateBuildingDef();
            def.PrefabID = Id;
            return def;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            base.DoPostConfigureComplete(go);
            var def = go.GetComponent<Building>().Def;
            def.PrefabID = PipedLiquidBottlerConfig.Id;
        }
    }

    public class PipedLiquidBottlerConfig : IBuildingConfig
    {
        public const string Id = "asquared31415.PipedLiquidBottler";
        public const string DisplayName = "Liquid Canister Filler";
        public const string Description = "Canisters allow Duplicants to manually deliver liquids from place to place.";
        private const ConduitType BottlerConduitType = ConduitType.Liquid;
        private const int Width = 3;
        private const int Height = 2;

        public static string Effect = "Automatically stores piped " + UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") +
                                      " into canisters for manual transport.";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id, Width, Height, "gas_bottler_kanim", 100, 120f,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor,
                BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0);
            buildingDef.InputConduitType = BottlerConduitType;
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
            conduitConsumer.conduitType = BottlerConduitType;
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

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}