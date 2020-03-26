using STRINGS;

namespace ExpandedDuplicantMultitools
{
    public static class MULTITOOLSSTRINGS
    {
        public static class EQUIPMENT
        {
            public static class SLOTS
            {
                public static class TOOLATTACHMENT
                {
                    public static readonly LocString NAME = "Multitool Attachment";
                }
            }
            
            public static class PREFABS
            {
                public static class ASQUARED31415_NEUTRONIUMMINERATTACHMENTCONFIG
                {
                    public static readonly LocString NAME        = "Neutronium Miner Attachment";
                    public static readonly LocString GENERICNAME = "Multitool Attachment";
                }
            }
        }

        public static class SKILLS
        {
            public static class NEUTRONIUM_DIGGING
            {
                // TODO: FormatAsLink?
                public static readonly LocString NAME = "Neutronium Digging";

                public static readonly LocString DESCRIPTION =
                    "Allows excavation of " + ELEMENTS.UNOBTANIUM.NAME + " (except at the bottom of the map)";
            }
        }

        public static class SKILLPERKS
        {
            public static class NEUTRONIUM_DIGGING
            {
                public static readonly LocString DESCRIPTION = ELEMENTS.UNOBTANIUM.NAME + " Mining";
            }
        }
    }
}
