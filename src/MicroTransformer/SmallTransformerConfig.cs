using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace MicroTransformer
{
    public class SmallTransformerConfig : IBuildingConfig
    {
        public static string Id = "asquared31415.MicroTransformer";
        public static string DisplayName = "Micro Transformer";

        public static readonly string Description =
            $"Connect {UI.FormatAsLink("Batteries", "BATTERY")} on the large side to act as a valve and prevent {UI.FormatAsLink("Wires", "WIRE")} from drawing more than 750 W and suffering overload damage.";

        public static readonly string Effect =
            $"Limits {UI.FormatAsLink("Power", "POWER")} flowing through the Transformer to 750 W.";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                id: Id,
                width: 2,
                height: 1,
                anim: "micro_transformer_kanim",
                hitpoints: BUILDINGS.HITPOINTS.TIER2,
                BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER0,
                construction_mass: BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
                construction_materials: MATERIALS.ALL_METALS,
                melting_point: BUILDINGS.MELTING_POINT_KELVIN.TIER1,
                build_location_rule: BuildLocationRule.Anywhere,
                decor: BUILDINGS.DECOR.PENALTY.TIER1,
                noise: NOISE_POLLUTION.NOISY.TIER0
            );

            buildingDef.RequiresPowerInput = true;
            buildingDef.UseWhitePowerOutputConnectorColour = true;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.PowerOutputOffset = new CellOffset(1, 0);
            buildingDef.ElectricalArrowOffset = new CellOffset(1, 0);
            buildingDef.ExhaustKilowattsWhenActive = 0.0f;
            buildingDef.SelfHeatKilowattsWhenActive = 1f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.Entombable = true;
            buildingDef.GeneratorWattageRating = 750f;
            buildingDef.GeneratorBaseCapacity = 750f;
            buildingDef.PermittedRotations = PermittedRotations.R360;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
            go.AddComponent<RequireInputs>();
            var def = go.GetComponent<Building>().Def;
            var battery = go.AddOrGet<Battery>();
            battery.powerSortOrder = 1000;
            battery.capacity = def.GeneratorWattageRating;
            battery.chargeWattage = def.GeneratorWattageRating;
            go.AddComponent<PowerTransformer>().powerDistributionOrder = 9;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Object.DestroyImmediate(go.GetComponent<EnergyConsumer>());
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}