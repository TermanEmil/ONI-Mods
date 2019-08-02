using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace MoreCanisterFillersMod
{
    internal class ConveyorLoadedCanisterEmptierConfig : IBuildingConfig
    {
        public const string Id = "ConveyorBottleEmptier";
        public const string DisplayName = "Conveyor Loaded Canister Emptier";
        public const string Description = "";

        public const ConduitType EmptierConduitType = ConduitType.Solid;

        public static readonly string Effect = "Unloads bottles from a " +
                                               UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") + " into the world.";

        public override BuildingDef CreateBuildingDef()
        {
            const string anim = "gas_emptying_station_kanim";
            const int hitpoints = 30;
            const float constructionTime = 10f;
            var constTier4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            var rawMinerals = MATERIALS.RAW_MINERALS;
            const float meltingPoint = 1600f;
            const BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
            var none = NOISE_POLLUTION.NONE;
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id, 1, 3, anim, hitpoints,
                constructionTime, constTier4, rawMinerals, meltingPoint, buildLocationRule,
                BUILDINGS.DECOR.PENALTY.TIER1, none);
            buildingDef.Floodable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.Overheatable = true;
            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
            buildingDef.InputConduitType = EmptierConduitType;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefabTag)
        {
            Prioritizable.AddRef(go);
            var storage = go.AddOrGet<Storage>();
            var tagList = new List<Tag>();
            tagList.AddRange(STORAGEFILTERS.GASES);
            tagList.AddRange(STORAGEFILTERS.LIQUIDS);
            storage.storageFilters = tagList;
            storage.showDescriptor = true;
            storage.capacityKg = 200f;
            storage.allowItemRemoval = false;

            var conduitConsumer = go.AddOrGet<SolidConduitConsumer>();
            conduitConsumer.storage = storage;
            conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = storage.capacityKg;

            //go.AddOrGet<TreeFilterable>();
            go.AddOrGet<UnfilteredBottleEmptier>().EmptyRate = 0.4f;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}