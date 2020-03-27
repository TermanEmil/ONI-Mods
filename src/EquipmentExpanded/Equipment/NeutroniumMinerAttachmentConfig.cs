using UnityEngine;

namespace EquipmentExpanded.Equipment
{
    public class NeutroniumMinerAttachmentConfig : MultitoolAttachmentConfig
    {
        public const string Id = "asquared31415_" + nameof(NeutroniumMinerAttachmentConfig);
        public new const string IconAnim = "shirt_cold01_kanim";
        public new const string Anim0 = "body_shirt_cold01_kanim";

        public override EquipmentDef CreateEquipmentDef()
        {
            return CreateAttachmentDef(Id, IconAnim, Anim0);
        }

        public override void DoPostConfigure(GameObject go)
        {
            base.DoPostConfigure(go);
            go.AddOrGet<MultitoolAttachment>().toolType = MultitoolAttachment.AttachmentType.NeutroniumMiner;
        }
    }
}