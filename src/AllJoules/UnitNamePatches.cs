using Units = STRINGS.UI.UNITSUFFIXES;

namespace AllJoules
{
    internal class UnitNamePatches
    {
        public static void OnLoad()
        {
            Units.ELECTRICAL.WATT = Units.ELECTRICAL.JOULE + "/s";
            Units.ELECTRICAL.KILOWATT = Units.ELECTRICAL.KILOJOULE + "/s";

            Units.HEAT.DTU = Units.ELECTRICAL.JOULE;
            Units.HEAT.KDTU = Units.ELECTRICAL.KILOJOULE;
            Units.HEAT.DTU_S = Units.ELECTRICAL.JOULE + "/s";
            Units.HEAT.KDTU_S = Units.ELECTRICAL.KILOJOULE + "/s";
        }
    }
}
