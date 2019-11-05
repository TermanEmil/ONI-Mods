using CaiLib.Utils;
using Database;
using Harmony;
using STRINGS;

namespace ShinebugReactor
{
    internal class ShinebugReactorPatches
    {
        public static void OnLoad()
        {
            StringUtils.AddBuildingStrings(ShinebugReactorConfig.Id, ShinebugReactorConfig.DisplayName,
                ShinebugReactorConfig.Description, ShinebugReactorConfig.Effect);
            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Power, ShinebugReactorConfig.Id);
            BuildingUtils.AddBuildingToTechnology(GameStrings.Technology.Power.RenewableEnergy, ShinebugReactorConfig.Id);

            Strings.Add("STRINGS.BUILDING.STATUSITEMS.SHINEBUGREACTORWATTAGE.NAME", "Current Wattage: {Wattage}");
            Strings.Add("STRINGS.BUILDING.STATUSITEMS.SHINEBUGREACTORWATTAGE.TOOLTIP",
                "This reactor is generating " + UI.FormatAsPositiveRate("{Wattage}") + " of " + UI.PRE_KEYWORD +
                "Power" + UI.PST_KEYWORD);
            //TODO: Strings
        }

        [HarmonyPatch(typeof(BuildingStatusItems), "CreateStatusItems")]
        public class Tooltips
        {
            public static void Postfix(BuildingStatusItems __instance)
            {
                var method = Traverse.Create(__instance).Method("CreateStatusItem", new[]
                {
                    typeof(string), typeof(string), typeof(string),
                    typeof(StatusItem.IconType), typeof(NotificationType), typeof(bool), typeof(HashedString),
                    typeof(bool), typeof(int)
                });
                ReactorStatusItem.ShinebugReactorWattageStatus = (StatusItem) method.GetValue("ShinebugReactorWattage",
                    "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false,
                    OverlayModes.Power.ID, true, 129022);

                ReactorStatusItem.ShinebugReactorWattageStatus.resolveStringCallback = (str, data) =>
                {
                    var reactor = (ShinebugReactor) data;
                    str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(reactor.CurrentWattage));
                    return str;
                };
            }
        }
    }
}