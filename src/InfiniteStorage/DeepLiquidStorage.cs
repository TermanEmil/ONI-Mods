using System;
using TUNING;
using UnityEngine;

namespace InfiniteStorage
{
    public class DeepLiquidStorage : IBuildingConfig
    {
        public const string Id = "asquared31415_InfiniteLiquidStorage";

        private const string Anim = "liquidreservoir_kanim";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id, 4, 3, Anim, 3, 60f,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.REFINED_METALS, 1_600f, BuildLocationRule.OnFloor,
                BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NONE);
            buildingDef.Floodable = false;
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.Overheatable = false;

            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, Id);

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            var storage = go.AddOrGet<Storage>();
            storage.capacityKg = float.PositiveInfinity;
            storage.showDescriptor = true;
            storage.allowItemRemoval = false;
            storage.allowSublimation = false;
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.showInUI = true;
            storage.SetDefaultStoredItemModifiers(GasReservoirConfig.ReservoirStoredItemModifiers);
            go.AddOrGet<UserNameable>();

            var conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.ignoreMinMassCheck = true;
            conduitConsumer.forceAlwaysSatisfied = true;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = storage.capacityKg;
            var conduitDispenser = go.AddOrGet<ConduitDispenser>();
            conduitDispenser.conduitType = ConduitType.Liquid;
            conduitDispenser.elementFilter = null;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits);
        }
    }
}