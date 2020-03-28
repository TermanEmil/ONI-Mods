using UnityEngine;

namespace EquipmentExpanded.Equipment
{
    public class NeutroniumMinerAttachmentConfig : MultitoolAttachmentConfig
    {
        public const string IconAnim = "shirt_cold01_kanim";
        public const string EquippedAnim = "body_shirt_cold01_kanim";

        public override EquipmentDef CreateEquipmentDef()
        {
            return CreateAttachmentDef("asquared31415_" + nameof(NeutroniumMinerAttachmentConfig), IconAnim, EquippedAnim);
        }

        public override void DoPostConfigure(GameObject go)
        {
            base.DoPostConfigure(go);
            go.AddOrGet<MultitoolAttachment>().toolType = MultitoolAttachment.AttachmentType.NeutroniumMiner;
        }
    }
}