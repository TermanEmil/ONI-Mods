﻿using STRINGS;

namespace EquipmentExpanded
{
    public static class MULTITOOLSSTRINGS
    {
        public static class BUILDING
        {
            public static class STATUSITEMS
            {
                public static class ASQUARED31415_COLONYLACKSNEUTRONIUMMINER
                {
                    public static LocString NAME = "Colony Lacks " + SKILLS.NEUTRONIUM_DIGGING.NAME + " Skill";
                    public static LocString TOOLTIP = "{Skill}\n{Equipment}";
                    
                    public static LocString NEEDSSKILL =
                        "Open the " + UI.FormatAsManagementMenu("Skills Panel", "[L]") +
                        " and teach a Duplicant the " + SKILLS.NEUTRONIUM_DIGGING.NAME + " Skill to use this.";
                    public static LocString NEEDSEQUIPMENT = "A Duplicant will need to have a " + 
                                                             EQUIPMENT.PREFABS.ASQUARED31415_NEUTRONIUMMINERATTACHMENTCONFIG.NAME +
                                                             " equipped.";
                }
            }
        }
        
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