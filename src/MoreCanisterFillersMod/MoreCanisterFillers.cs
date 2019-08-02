using Harmony;
using STRINGS;

namespace MoreCanisterFillersMod
{
    public class MoreCanisterFillers
    {
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
    }
}