using Steamworks;
using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace SpotHeater
{
    public class SpotHeaterConfig : IBuildingConfig
    {
        public const string Id = "asquared31415.SpotHeater";
        public const string DisplayName = "Spot Heater";
        public const string Description = "A spot heater is useful for keeping things just a tad bit warmer.";
        public static readonly string Effect = "Radiates a small amount of." + UI.FormatAsLink("Heat", "HEAT");

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id, 1, 1, "farmtilerotating_kanim",
                BUILDINGS.HITPOINTS.TIER2, BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, BUILDINGS.MELTING_POINT_KELVIN.TIER4,
                BuildLocationRule.Tile, BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER1);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 60;
            buildingDef.SelfHeatKilowattsWhenActive = 1.5f;
            buildingDef.PowerInputOffset = CellOffset.none;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.OverheatTemperature = BUILDINGS.OVERHEAT_TEMPERATURES.HIGH_2;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefabTag)
        {
            go.AddOrGet<SpaceHeater>().targetTemperature = 323.15f;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<PoweredActiveController.Def>();
        }
    }
}