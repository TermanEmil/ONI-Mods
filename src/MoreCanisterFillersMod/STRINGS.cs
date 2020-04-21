using STRINGS;

namespace MoreCanisterFillersMod
{
    public static class STRINGS
    {
        public static class BUILDINGS
        {
            public static class PREFABS
            {
                public static class ASQUARED31415
                {
                    public static class PIPEDLIQUIDBOTTLER
                    {
                        public static LocString NAME = "Bottle Filler";

                        public static LocString EFFECT = "Automatically stores piped " +
                                                         UI.FormatAsLink("Liquids", "ELEMENTS_LIQUID") +
                                                         " into canisters for manual transport.";

                        public static LocString DESC =
                            "Bottles allow Duplicants to manually deliver liquids from place to place.";
                    }

                    public static class CONVEYORBOTTLEEMPTIER
                    {
                        public static LocString NAME = "Conveyor Loaded Canister Emptier";

                        public static LocString EFFECT = "Unloads bottles from a " +
                                                         UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") +
                                                         " into the world.";

                        public static LocString DESC = "";
                    }

                    public static class CONVEYORLIQUIDPIPEFILLERCONFIG
                    {
                        public static LocString NAME = "Liquid Pipe Filler";

                        public static LocString EFFECT = "Unloads bottles from a " +
                                                         UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") +
                                                         " into a liquid pipe.";

                        public static LocString DESC = "";
                    }

                    public static class CONVEYORLIQUIDLOADERCONFIG
                    {
                        public static LocString NAME = "Liquid Conveyor Loader";

                        public static LocString EFFECT = "Loads liquids from a pipe onto a " +
                                                         UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT");

                        public static LocString DESC = "";
                    }

                    public static class CONVEYORGASPIPEFILLERCONFIG
                    {
                        public static LocString NAME = "Gas Pipe Filler";

                        public static LocString EFFECT = "Unloads canisters from a " +
                                                         UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT") +
                                                         " into a gas pipe.";

                        public static LocString DESC = "";
                    }

                    public static class CONVEYORGASLOADERCONFIG
                    {
                        public static LocString NAME = "Gas Conveyor Loader";

                        public static LocString EFFECT = "Loads gasses from a pipe onto a " +
                                                         UI.FormatAsLink("Conveyor Rail", "SOLIDCONDUIT");

                        public static LocString DESC = "";
                    }

                    public static class CONVEYORBOTTLELOADER
                    {
                        public static LocString NAME = "Conveyor Canister Loader (deprecated)";

                        public static LocString EFFECT =
                            "This building is deprecated. Please use the Conveyor Loader from the base game.";

                        public static LocString DESC = "";
                    }
                }
            }
        }
    }
}
