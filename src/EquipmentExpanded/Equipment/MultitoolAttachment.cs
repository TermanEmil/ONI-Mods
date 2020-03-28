namespace EquipmentExpanded.Equipment
{
    public class MultitoolAttachment : KMonoBehaviour
    { 
        public AttachmentType toolType;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Debug.Log("Attachment added");
        }

        public enum AttachmentType
        {
            None,
            NeutroniumMiner,
            SuperSpeed,
        }
    }
}