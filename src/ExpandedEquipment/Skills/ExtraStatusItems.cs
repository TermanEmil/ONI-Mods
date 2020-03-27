using static ExpandedEquipment.MULTITOOLSSTRINGS.BUILDING.STATUSITEMS.ASQUARED31415_COLONYLACKSNEUTRONIUMMINER;

namespace ExpandedEquipment.Skills
{
    public class ExtraStatusItems
    {
        public static StatusItem ColonyLacksNeutroniumMiner;

        public static void InitializeStatusItems()
        {
            var statusItems = Db.Get().BuildingStatusItems;
            ColonyLacksNeutroniumMiner = statusItems.Add(new StatusItem("asquared31415_" + nameof(ColonyLacksNeutroniumMiner), "BUILDING", "status_item_role_required", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID));
            ColonyLacksNeutroniumMiner.resolveStringCallback = (str, data) =>
            {
                str = NeutroniumPerk.AnyMinionHasSkill()
                    ? str.Replace("{Skill}\n", "")
                    : str.Replace("{Skill}", NEEDSSKILL);
                str = NeutroniumPerk.AnyMinionHasPerk()
                    ? str.Replace("{Equipment}", "")
                    : str.Replace("{Equipment}", NEEDSEQUIPMENT);
                return str.Replace("{Skill}", NEEDSSKILL).Replace("{Equipment}", NEEDSEQUIPMENT);
            };
        }
    }
}