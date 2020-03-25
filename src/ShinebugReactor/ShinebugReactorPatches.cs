using System.Diagnostics;
using System.IO;
using System.Reflection;
using CaiLib.Config;
using CaiLib.Utils;
using Database;
using Harmony;
using STRINGS;

namespace ShinebugReactor
{
    public class ShinebugReactorPatches
    {
        public static ConfigManager<ReactorConfig> ConfigManager;

        public static void OnLoad()
        {
            CaiLib.Logger.Logger.LogInit();

            var assemblyPath = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) ?? "";
            var confFile = Path.Combine( assemblyPath, "Config.json" );
            var defaultConf = Path.Combine( assemblyPath, "Config_Default.json" );
            if ( !File.Exists( confFile ) )
            {
                Debug.Log(
                    "[ShinebugReactor] Could not find Config.json configuration file, creating from default configuration."
                );

                File.Copy( defaultConf, confFile );
            }

            ConfigManager = new ConfigManager<ReactorConfig>();
            ConfigManager.ReadConfig();

            StringUtils.AddBuildingStrings(
                ShinebugReactorConfig.Id,
                ShinebugReactorConfig.DisplayName,
                ShinebugReactorConfig.Description,
                ShinebugReactorConfig.Effect
            );

            BuildingUtils.AddBuildingToPlanScreen( GameStrings.PlanMenuCategory.Power, ShinebugReactorConfig.Id );
            BuildingUtils.AddBuildingToTechnology(
                GameStrings.Technology.Power.RenewableEnergy,
                ShinebugReactorConfig.Id
            );

            Strings.Add( "STRINGS.BUILDING.STATUSITEMS.SHINEBUGREACTORWATTAGE.NAME", "Current Wattage: {Wattage}" );
            Strings.Add(
                "STRINGS.BUILDING.STATUSITEMS.SHINEBUGREACTORWATTAGE.TOOLTIP",
                "This reactor is generating " +
                UI.FormatAsPositiveRate( "{Wattage}" ) +
                " of " +
                UI.PRE_KEYWORD +
                "Power" +
                UI.PST_KEYWORD
            );
        }

        [HarmonyPatch( typeof( BuildingStatusItems ), "CreateStatusItems" )]
        public class Tooltips
        {
            public static void Postfix( BuildingStatusItems __instance )
            {
                var method = Traverse.Create( __instance ).Method(
                    "CreateStatusItem",
                    new[]
                    {
                        typeof( string ),
                        typeof( string ),
                        typeof( string ),
                        typeof( StatusItem.IconType ),
                        typeof( NotificationType ),
                        typeof( bool ),
                        typeof( HashedString ),
                        typeof( bool ),
                        typeof( int )
                    }
                );

                ReactorStatusItem.ShinebugReactorWattageStatus = (StatusItem) method.GetValue(
                    "ShinebugReactorWattage",
                    "BUILDING",
                    string.Empty,
                    StatusItem.IconType.Info,
                    NotificationType.Neutral,
                    false,
                    OverlayModes.Power.ID,
                    true,
                    129022
                );

                ReactorStatusItem.ShinebugReactorWattageStatus.resolveStringCallback = ( str, data ) =>
                {
                    var reactor = (ShinebugReactor) data;
                    str = str.Replace( "{Wattage}", GameUtil.GetFormattedWattage( reactor.CurrentWattage ) );
                    return str;
                };
            }
        }
    }
}
