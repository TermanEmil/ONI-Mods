using STRINGS;

namespace ShinebugReactor
{
	public static class REACTORSTRINGS
	{
		public static class BUILDING
		{
			public static class STATUSITEMS
			{
				public static class SHINEBUGREACTORWATTAGE
				{
					public static LocString NAME = "Current Wattage: {Wattage}";

					public static LocString TOOLTIP = "This reactor is generating " +
					                                  STRINGS.UI.FormatAsPositiveRate("{Wattage}") +
					                                  " of " +
					                                  STRINGS.UI.PRE_KEYWORD +
					                                  "Power" +
					                                  STRINGS.UI.PST_KEYWORD;
				}
			}
		}

		public static class BUILDINGS
		{
			public static class PREFABS
			{
				public static class SHINEBUGREACTOR
				{
					public static LocString NAME = "Shinebug Reactor";

					public static LocString EFFECT =
						$"Receives {CREATURES.FAMILY_PLURAL.LIGHTBUGSPECIES} from a conveyor and " +
						$"generates {STRINGS.UI.PRE_KEYWORD} Power {STRINGS.UI.PST_KEYWORD} passively using their light.";

					public static LocString DESC =
						"When eggs enter the reactor, they are stored inside until they hatch. The " +
						"newly hatched shine bugs are then enslaved for your gain.";
				}
			}
		}

		public static class UI
		{
			public static class SHOWHIDECONTENTS
			{
				public static LocString SHOW = "Show Contents";
				public static LocString HIDE = "Hide Contents";

				public static LocString SHOW_TOOLTIP =
					$"Show the contents of the {BUILDINGS.PREFABS.SHINEBUGREACTOR.NAME}.\n<b><color=#FF0000>Warning! MAY LAG!</color></b>";

				public static LocString HIDE_TOOLTIP =
					$"Hide the contents of the {BUILDINGS.PREFABS.SHINEBUGREACTOR.NAME}.";
			}
		}
	}
}