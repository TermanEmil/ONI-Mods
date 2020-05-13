using System.IO;
using System.Reflection;
using CaiLib.Config;
using CaiLib.Utils;
using Database;
using Harmony;

namespace ShinebugReactor
{
	public class Hooks
	{
		private static ConfigManager<ReactorConfig> _configManager;

		public static void OnLoad()
		{
			CaiLib.Logger.Logger.LogInit();

			var assemblyPath =
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
			var confFile = Path.Combine(assemblyPath, "Config.json");
			if(!File.Exists(confFile))
			{
				Debug.LogWarning(
					"[ShinebugReactor] Could not find Config.json configuration file, creating from default configuration.");
				var defaultConf = Path.Combine(assemblyPath, "Config_Default.json");
				if(!File.Exists(defaultConf))
					Debug.LogWarning("[ShinebugReactor] Could not find default configuration file!");
				else
					File.Copy(defaultConf, confFile);
			}

			_configManager = new ConfigManager<ReactorConfig>();
			_configManager.ReadConfig();

			BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Power,
				ShinebugReactorConfig.Id);
			BuildingUtils.AddBuildingToTechnology(GameStrings.Technology.Power.RenewableEnergy,
				ShinebugReactorConfig.Id);
			
			foreach(var type in typeof(REACTORSTRINGS).GetNestedTypes())
			{
				LocString.CreateLocStringKeys(type);
			}
		}
	}
	
	[HarmonyPatch(typeof(BuildingStatusItems), "CreateStatusItems")]
	public class BuildingStatusItems_CreateStatusItems_Patches
	{
		public static void Postfix(BuildingStatusItems __instance)
		{
			ReactorStatusItem.ShinebugReactorWattageStatus = __instance.Add(new StatusItem("ShinebugReactorWattage",
				"BUILDING",
				string.Empty,
				StatusItem.IconType.Info,
				NotificationType.Neutral,
				false,
				OverlayModes.Power.ID));

			ReactorStatusItem.ShinebugReactorWattageStatus.resolveStringCallback =
				(str, data) =>
				{
					var reactor = data as ShinebugReactor;
					if(reactor != null)
						str = str.Replace("{Wattage}",
							GameUtil.GetFormattedWattage(reactor.CurrentWattage));
					return str;
				};
		}
	}
}