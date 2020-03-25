﻿using System.Linq;
using CaiLib.Utils;
using Harmony;
using STRINGS;
using TUNING;
using UnityEngine;

namespace MoreCanisterFillersMod
{
    public class MoreCanisterFillersPatches
    {
        public static void OnLoad()
        {
            CaiLib.Logger.Logger.LogInit();

            // Piped Liquid Bottler
            Strings.Add(
                $"STRINGS.BUILDINGS.PREFABS.{PipedLiquidBottlerConfig.Id.ToUpperInvariant()}.NAME",
                UI.FormatAsLink( PipedLiquidBottlerConfig.DisplayName, PipedLiquidBottlerConfig.Id )
            );

            Strings.Add(
                $"STRINGS.BUILDINGS.PREFABS.{PipedLiquidBottlerConfig.Id.ToUpperInvariant()}.DESC",
                PipedLiquidBottlerConfig.Description
            );

            Strings.Add(
                $"STRINGS.BUILDINGS.PREFABS.{PipedLiquidBottlerConfig.Id.ToUpperInvariant()}.EFFECT",
                PipedLiquidBottlerConfig.Effect
            );

            // Conveyor Loaded Canister Emptier
            Strings.Add(
                $"STRINGS.BUILDINGS.PREFABS.{ConveyorLoadedCanisterEmptierConfig.Id.ToUpperInvariant()}.NAME",
                UI.FormatAsLink(
                    ConveyorLoadedCanisterEmptierConfig.DisplayName,
                    ConveyorLoadedCanisterEmptierConfig.Id
                )
            );

            Strings.Add(
                $"STRINGS.BUILDINGS.PREFABS.{ConveyorLoadedCanisterEmptierConfig.Id.ToUpperInvariant()}.DESC",
                ConveyorLoadedCanisterEmptierConfig.Description
            );

            Strings.Add(
                $"STRINGS.BUILDINGS.PREFABS.{ConveyorLoadedCanisterEmptierConfig.Id.ToUpperInvariant()}.EFFECT",
                ConveyorLoadedCanisterEmptierConfig.Effect
            );

            // Conveyor Loaded Canister Loader
            Strings.Add(
                $"STRINGS.BUILDINGS.PREFABS.{ConveyorCanisterLoaderConfig.Id.ToUpperInvariant()}.NAME",
                UI.FormatAsLink( ConveyorCanisterLoaderConfig.DisplayName, ConveyorCanisterLoaderConfig.Id )
            );

            Strings.Add(
                $"STRINGS.BUILDINGS.PREFABS.{ConveyorCanisterLoaderConfig.Id.ToUpperInvariant()}.DESC",
                ConveyorCanisterLoaderConfig.Description
            );

            Strings.Add(
                $"STRINGS.BUILDINGS.PREFABS.{ConveyorCanisterLoaderConfig.Id.ToUpperInvariant()}.EFFECT",
                ConveyorCanisterLoaderConfig.Effect
            );

            ModUtil.AddBuildingToPlanScreen( GameStrings.PlanMenuCategory.Plumbing, PipedLiquidBottlerConfig.Id );
            ModUtil.AddBuildingToPlanScreen(
                GameStrings.PlanMenuCategory.Shipping,
                ConveyorLoadedCanisterEmptierConfig.Id
            );

            ModUtil.AddBuildingToPlanScreen( GameStrings.PlanMenuCategory.Shipping, ConveyorCanisterLoaderConfig.Id );

            BuildingUtils.AddBuildingToTechnology(
                GameStrings.Technology.Liquids.Plumbing,
                PipedLiquidBottlerConfig.Id
            );

            BuildingUtils.AddBuildingToTechnology(
                GameStrings.Technology.SolidMaterial.SolidTransport,
                ConveyorCanisterLoaderConfig.Id
            );

            BuildingUtils.AddBuildingToTechnology(
                GameStrings.Technology.SolidMaterial.SolidTransport,
                ConveyorLoadedCanisterEmptierConfig.Id
            );
        }

        [HarmonyPatch( typeof( GasBottlerConfig ), nameof( GasBottlerConfig.ConfigureBuildingTemplate ) )]
        public class GasFillerPatches
        {
            public static void Postfix( GameObject go ) { go.AddOrGet<AutoDropInv>(); }
        }

        // Allows the Transfer Arm to pick up liquids and gasses
        [HarmonyPatch( typeof( SolidTransferArm ), "IsPickupableRelevantToMyInterests" )]
        public class TransferArmFix
        {
            private static bool _hasPatched;

            public static void Prefix( ref SolidTransferArm __instance )
            {
                if ( !_hasPatched )
                {
                    _hasPatched = true;
                    SolidTransferArm.tagBits = new TagBits(
                        STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat( STORAGEFILTERS.FOOD ).Concat( STORAGEFILTERS.GASES )
                                      .Concat( STORAGEFILTERS.LIQUIDS ).ToArray()
                    );
                }
            }
        }
    }
}
