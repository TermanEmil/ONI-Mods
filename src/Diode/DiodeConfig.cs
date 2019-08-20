using System;
using TUNING;
using UnityEngine;

namespace Diode
{
    internal class DiodeConfig : IBuildingConfig
    {
        public const string Id = "Diode";
        private readonly string _anim = "valvegas_logic_kanim";

        public override BuildingDef CreateBuildingDef()
        {
            var hitpoints = 30;
            var constructionTime = 10f;
            var constructionMass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
            var refinedMetals = MATERIALS.REFINED_METALS;
            var meltingPoint = 1600f;
            var buildLocationRule = BuildLocationRule.Anywhere;
            var noise = NOISE_POLLUTION.NOISY.TIER1;
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id, 1, 2, _anim, hitpoints, constructionTime,
                constructionMass, refinedMetals, meltingPoint, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, noise);
            buildingDef.Floodable = false;
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = CellOffset.none;
            buildingDef.PowerOutputOffset = new CellOffset(1, 0);
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.PermittedRotations = PermittedRotations.R360;
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<Diode>();
        }
    }
}