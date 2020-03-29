using EquipmentExpanded.Equipment.HeadEquipment;
using EquipmentExpanded.Equipment.MultitoolAttachment;
using static EquipmentExpanded.MULTITOOLSSTRINGS.EQUIPMENT;

namespace EquipmentExpanded.Equipment
{
    public class ExpandedAssignableSlots
    {
        public static AssignableSlot ToolAttachment;
        public const  string         ToolAttachmentId = "asquared31415_" + nameof( ToolAttachment );
        public static AssignableSlot HeadAttachment;
        public const string HeadAttachmentId = "asquared31415_" + nameof(HeadAttachment);

        public static AssignableSlot CustomSuit;
        public const string CustomSuitId = "asquared31415_" + nameof(CustomSuit);

        public static void InitializeSlots()
        {
            var assignableSlots = Db.Get().AssignableSlots;
            ToolAttachment = assignableSlots.Add(
                new EquipmentSlot(
                    ToolAttachmentId,
                    SLOTS.TOOLATTACHMENT.NAME
                )
            );

            HeadAttachment = assignableSlots.Add(
                new EquipmentSlot(
                    HeadAttachmentId,
                    SLOTS.HEADATTCHMENT.NAME
                )
            );

            CustomSuit = assignableSlots.Add(
                new EquipmentSlot(
                    CustomSuitId,
                    SLOTS.CUSTOMSUIT.NAME
                    )
                );
        }

        public static void LoadAllEquipment()
        {
            EquipmentConfigManager.Instance.RegisterEquipment( new NeutroniumMinerAttachmentConfig() );
            EquipmentConfigManager.Instance.RegisterEquipment( new SuperSpeedMinerAttachmentConfig() );
            EquipmentConfigManager.Instance.RegisterEquipment( new OxygenRebreatherConfig() );
        }
    }
}
