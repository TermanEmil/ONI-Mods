using System.Collections.Generic;
using System.Linq;
using Harmony;
using STRINGS;
using TUNING;

namespace MoreCanisterFillersMod
{
    public class MoreCanisterFillers
    {
        // Allows the Transfer Arm to pick up liquids and gasses
        [HarmonyPatch(typeof(SolidTransferArm), MethodType.Constructor)]
        public class TransferArmFix
        {
            public static void Postfix(ref SolidTransferArm __instance)
            {
                SolidTransferArm.tagBits = new TagBits(STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Concat(STORAGEFILTERS.FOOD).Concat(STORAGEFILTERS.GASES).Concat(STORAGEFILTERS.LIQUIDS).ToArray<Tag>());
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class RegisterBuildings
        {
            public static void Prefix()
            {
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{PipedLiquidBottlerConfig.Id.ToUpperInvariant()}.NAME",
                    UI.FormatAsLink(PipedLiquidBottlerConfig.DisplayName, PipedLiquidBottlerConfig.Id));
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{PipedLiquidBottlerConfig.Id.ToUpperInvariant()}.DESC",
                    PipedLiquidBottlerConfig.Description);
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{PipedLiquidBottlerConfig.Id.ToUpperInvariant()}.EFFECT",
                    PipedLiquidBottlerConfig.Effect);

                Strings.Add(
                    $"STRINGS.BUILDINGS.PREFABS.{ConveyorLoadedCanisterEmptierConfig.Id.ToUpperInvariant()}.NAME",
                    UI.FormatAsLink(ConveyorLoadedCanisterEmptierConfig.DisplayName,
                        ConveyorLoadedCanisterEmptierConfig.Id));
                Strings.Add(
                    $"STRINGS.BUILDINGS.PREFABS.{ConveyorLoadedCanisterEmptierConfig.Id.ToUpperInvariant()}.DESC",
                    ConveyorLoadedCanisterEmptierConfig.Description);
                Strings.Add(
                    $"STRINGS.BUILDINGS.PREFABS.{ConveyorLoadedCanisterEmptierConfig.Id.ToUpperInvariant()}.EFFECT",
                    ConveyorLoadedCanisterEmptierConfig.Effect);

                Strings.Add(
                    $"STRINGS.BUILDINGS.PREFABS.{ConveyorCanisterLoaderConfig.Id.ToUpperInvariant()}.NAME",
                    UI.FormatAsLink(ConveyorCanisterLoaderConfig.DisplayName,
                        ConveyorCanisterLoaderConfig.Id));
                Strings.Add(
                    $"STRINGS.BUILDINGS.PREFABS.{ConveyorCanisterLoaderConfig.Id.ToUpperInvariant()}.DESC",
                    ConveyorCanisterLoaderConfig.Description);
                Strings.Add(
                    $"STRINGS.BUILDINGS.PREFABS.{ConveyorCanisterLoaderConfig.Id.ToUpperInvariant()}.EFFECT",
                    ConveyorCanisterLoaderConfig.Effect);

                ModUtil.AddBuildingToPlanScreen("Plumbing", PipedLiquidBottlerConfig.Id);
                ModUtil.AddBuildingToPlanScreen("Conveyance", ConveyorLoadedCanisterEmptierConfig.Id);
                ModUtil.AddBuildingToPlanScreen("Conveyance", ConveyorCanisterLoaderConfig.Id);
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                Database.Techs.TECH_GROUPING["LiquidPiping"] = new List<string>(Database.Techs.TECH_GROUPING["LiquidPiping"]) { PipedLiquidBottlerConfig.Id }.ToArray();
                Database.Techs.TECH_GROUPING["SolidTransport"] = new List<string>(Database.Techs.TECH_GROUPING["SolidTransport"]) { ConveyorCanisterLoaderConfig.Id }.ToArray();
                Database.Techs.TECH_GROUPING["SolidTransport"] = new List<string>(Database.Techs.TECH_GROUPING["SolidTransport"]) { ConveyorLoadedCanisterEmptierConfig.Id }.ToArray();
            }
        }
    }
}