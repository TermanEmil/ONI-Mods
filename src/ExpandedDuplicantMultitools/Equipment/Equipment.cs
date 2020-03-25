namespace ExpandedDuplicantMultitools.Equipment
{
    public class ExpandedAssignableSlots
    {
        public static AssignableSlot ToolAttachment;
        public const string ToolAttachmentId = "asquared31415_" + nameof(ToolAttachment);

        public static void InitializeSlots()
        {
            ToolAttachment = Db.Get().AssignableSlots
                .Add(new EquipmentSlot(ToolAttachmentId,
                    MULTITOOLSSTRINGS.EQUIPMENT.PREFABS.ASQUARED31415_DUPLICANTMULTITOOLATTACHMENTCONFIG.NAME));
        }

        public static void LoadAllEquipment()
        {
            EquipmentConfigManager.Instance.RegisterEquipment(new DuplicantMultitoolAttachmentConfig());
        }
    }
}