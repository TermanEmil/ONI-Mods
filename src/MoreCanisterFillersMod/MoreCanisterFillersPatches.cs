using System.Linq;
using CaiLib.Utils;
using Harmony;
using MoreCanisterFillersMod.Buildings;
using MoreCanisterFillersMod.Components;
using TUNING;
using UnityEngine;
using static CaiLib.Utils.GameStrings;

namespace MoreCanisterFillersMod
{
    public static class Hooks
    {
        public static void OnLoad()
        {
            CaiLib.Logger.Logger.LogInit();

            ModUtil.AddBuildingToPlanScreen(PlanMenuCategory.Plumbing, PipedLiquidBottlerConfig.Id);
            ModUtil.AddBuildingToPlanScreen(PlanMenuCategory.Shipping, ConveyorLoadedCanisterEmptierConfig.Id);

            ModUtil.AddBuildingToPlanScreen(PlanMenuCategory.Shipping, ConveyorGasPipeFillerConfig.Id);
            ModUtil.AddBuildingToPlanScreen(PlanMenuCategory.Shipping, ConveyorLiquidPipeFillerConfig.Id);
            ModUtil.AddBuildingToPlanScreen(PlanMenuCategory.Shipping, ConveyorGasLoaderConfig.Id);
            ModUtil.AddBuildingToPlanScreen(PlanMenuCategory.Shipping, ConveyorLiquidLoaderConfig.Id);

            BuildingUtils.AddBuildingToTechnology(Technology.Liquids.Plumbing, PipedLiquidBottlerConfig.Id);
            BuildingUtils.AddBuildingToTechnology(
                Technology.SolidMaterial.SolidTransport,
                ConveyorLoadedCanisterEmptierConfig.Id
            );

            BuildingUtils.AddBuildingToTechnology(
                Technology.SolidMaterial.SolidTransport,
                ConveyorGasPipeFillerConfig.Id
            );

            BuildingUtils.AddBuildingToTechnology(
                Technology.SolidMaterial.SolidTransport,
                ConveyorLiquidPipeFillerConfig.Id
            );

            BuildingUtils.AddBuildingToTechnology(Technology.SolidMaterial.SolidTransport, ConveyorGasLoaderConfig.Id);

            BuildingUtils.AddBuildingToTechnology(
                Technology.SolidMaterial.SolidTransport,
                ConveyorLiquidLoaderConfig.Id
            );

            LocString.CreateLocStringKeys(typeof(STRINGS.BUILDINGS));
        }
    }

    public static class MoreCanisterFillersPatches
    {
        [HarmonyPatch(typeof(GasBottlerConfig), nameof(GasBottlerConfig.ConfigureBuildingTemplate))]
        public class GasFillerPatches
        {
            public static void Postfix(GameObject go) { go.AddOrGet<AutoDropInv>(); }
        }

        // Allows the conveyor loader to load canisters as well
        [HarmonyPatch(typeof(SolidConduitInboxConfig), nameof(SolidConduitInboxConfig.DoPostConfigureComplete))]
        public static class SolidConduitInboxConfig_DoPostConfigureComplete_Patches
        {
            public static void Postfix(GameObject go)
            {
                var storage = go.AddOrGet<Storage>();
                storage.storageFilters.AddRange(STORAGEFILTERS.LIQUIDS);
                storage.storageFilters.AddRange(STORAGEFILTERS.GASES);
            }
        }

        // Allows the Transfer Arm to pick up liquids and gasses
        [HarmonyPatch(typeof(SolidTransferArm), "IsPickupableRelevantToMyInterests")]
        public static class TransferArmFix
        {
            private static bool _hasPatched;

            public static void Prefix()
            {
                if(!_hasPatched)
                {
                    _hasPatched = true;
                    SolidTransferArm.tagBits = new TagBits(
                        STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat(STORAGEFILTERS.FOOD).Concat(STORAGEFILTERS.GASES)
                                      .Concat(STORAGEFILTERS.LIQUIDS).ToArray()
                    );
                }
            }
        }
    }
}
