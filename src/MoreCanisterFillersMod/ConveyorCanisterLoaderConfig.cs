using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace MoreCanisterFillersMod
{
    public class OldConveyorCanisterLoaderConfig : ConveyorCanisterLoaderConfig
    {
        private const string Id = "ConveyorBottleLoader";

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
            def.PrefabID = ConveyorCanisterLoaderConfig.Id;
        }
    }

    public class ConveyorCanisterLoaderConfig : IBuildingConfig
    {
        public const string Id = "asquared31415.ConveyorBottleLoader";
        public const string DisplayName = "Conveyor Canister Loader";
        public const string Description = "";

        private const ConduitType LoaderConduitType = ConduitType.Solid;

        public static readonly string Effect = "Loads bottles onto " +
                                               UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") +
                                               " for transport.\n\nOnly loads the resources of your choosing.";

        public override BuildingDef CreateBuildingDef()
        {
            const string anim = "conveyorin_kanim";
            const int hitpoints = 100;
            const float constructionTime = 60f;
            var constructionMass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
            var allMetals = MATERIALS.REFINED_METALS;
            const float meltingPoint = 1600f;
            const BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
            var none = NOISE_POLLUTION.NONE;
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id, 1, 2, anim, hitpoints, constructionTime,
                constructionMass, allMetals, meltingPoint, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, none);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 120f;
            buildingDef.SelfHeatKilowattsWhenActive = 2f;
            buildingDef.Floodable = false;
            buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.OutputConduitType = LoaderConduitType;
            buildingDef.PowerInputOffset = new CellOffset(0, 1);
            buildingDef.UtilityOutputOffset = CellOffset.none;
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(CellOffset.none);
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, Id);
            return buildingDef;
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<LogicOperationalController>();
            Prioritizable.AddRef(go);
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<EnergyConsumer>();
            go.AddOrGet<Automatable>();
            var tagList = new List<Tag>();
            tagList.AddRange(STORAGEFILTERS.GASES);
            tagList.AddRange(STORAGEFILTERS.LIQUIDS);
            var storage = go.AddOrGet<Storage>();
            storage.capacityKg = 1000f;
            storage.showInUI = true;
            storage.showDescriptor = true;
            storage.storageFilters = tagList;
            storage.allowItemRemoval = false;
            storage.onlyTransferFromLowerPriority = true;
            go.AddOrGet<TreeFilterable>();
            go.AddOrGet<SolidConduitInbox>();
            go.AddOrGet<SolidConduitDispenser>();
            go.AddOrGet<DropAllWorkable>();
        }
    }
}