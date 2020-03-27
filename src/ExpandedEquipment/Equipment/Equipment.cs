namespace ExpandedEquipment.Equipment
{
    public class ExpandedAssignableSlots
    {
        public static AssignableSlot ToolAttachment;
        public const  string         ToolAttachmentId = "asquared31415_" + nameof( ToolAttachment );

        public static void InitializeSlots()
        {
            ToolAttachment = Db.Get().AssignableSlots.Add(
                new EquipmentSlot(
                    ToolAttachmentId,
                    MULTITOOLSSTRINGS.EQUIPMENT.SLOTS.TOOLATTACHMENT.NAME
                )
            );
        }

        public static void LoadAllEquipment()
        {
            EquipmentConfigManager.Instance.RegisterEquipment( new NeutroniumMinerAttachmentConfig() );
        }
    }
}
