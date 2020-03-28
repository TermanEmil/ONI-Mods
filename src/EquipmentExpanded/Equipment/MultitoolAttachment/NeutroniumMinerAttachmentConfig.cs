using UnityEngine;

namespace EquipmentExpanded.Equipment.MultitoolAttachment
{
    public class NeutroniumMinerAttachmentConfig : MultitoolAttachmentConfig
    {
        public const string Id = "asquared31415_" + nameof(NeutroniumMinerAttachmentConfig);
        public const string IconAnim = "shirt_cold01_kanim";
        public const string EquippedAnim = "body_shirt_cold01_kanim";

        public override EquipmentDef CreateEquipmentDef()
        {
            return CreateAttachmentDef(Id, IconAnim, EquippedAnim);
        }

        public override void DoPostConfigure(GameObject go)
        {
            base.DoPostConfigure(go);
            go.AddOrGet<MultitoolAttachment>().toolType = MultitoolAttachment.AttachmentType.NeutroniumMiner;
        }
    }
}