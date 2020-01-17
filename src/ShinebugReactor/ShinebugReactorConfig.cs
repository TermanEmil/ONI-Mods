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
        public const string DisplayName = "Shinebug Reactor";

        public const string Description =
            "When eggs enter the reactor, they are stored inside unti they hatch.  The newly hatched shine bugs" +
            " are then enslaved for your gain.";

        public static readonly string Effect =
            $"Receives {CREATURES.FAMILY_PLURAL.LIGHTBUGSPECIES} from a conveyor and generates {UI.PRE_KEYWORD} Power + {UI.PST_KEYWORD} passively using their light";


        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                Id,
                10,
                5,
                "shinebug_reactor_kanim",
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
        }
    }
}