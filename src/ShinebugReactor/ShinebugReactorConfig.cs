using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;
using CREATURES = STRINGS.CREATURES;

namespace ShinebugReactor
{
    internal class ShinebugReactorConfig : IBuildingConfig
    {
        public const string Id = "ShinebugReactor";
        private const string Anim = "shinebug_reactor_kanim";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                Id,
                10,
                5,
                Anim,
                BUILDINGS.HITPOINTS.TIER4,
                BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
                new[] {500f, 1500f, 750f},
                new[] {SimHashes.Steel.ToString(), SimHashes.Iron.ToString(), SimHashes.Glass.ToString()},
                BUILDINGS.MELTING_POINT_KELVIN.TIER1,
                BuildLocationRule.OnFloor,
                BUILDINGS.DECOR.PENALTY.TIER1,
                NOISE_POLLUTION.NOISY.TIER0);

            buildingDef.AudioCategory = "Metal";
            buildingDef.PowerOutputOffset = CellOffset.none;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.GeneratorWattageRating = 1250f;
            buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
            buildingDef.UtilityInputOffset = new CellOffset(-4, 1);
            buildingDef.InputConduitType = ConduitType.Solid;

            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<Storage>();
            go.AddOrGet<SolidConduitConsumer>();
            go.AddOrGet<ShinebugReactor>();
            go.AddOrGetDef<PoweredActiveController.Def>();
            go.AddOrGet<ShowHideContentsButton>();
            go.AddOrGet<HatchAllEggsButton>();
        }
    }
}