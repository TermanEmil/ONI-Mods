using System.Collections.Generic;
using Klei.AI;
using UnityEngine;
using static EquipmentExpanded.MULTITOOLSSTRINGS.EQUIPMENT.PREFABS.ASQUARED31415_SUPERSPEEDMINERATTACHMENTCONFIG;

namespace EquipmentExpanded.Equipment.MultitoolAttachment
{
    public class SuperSpeedMinerAttachmentConfig : MultitoolAttachmentConfig
    {
        public const string Id = "asquared31415_" + nameof(SuperSpeedMinerAttachmentConfig);
        public const string IconAnim = "shirt_hot01_kanim";
        public const string EquippedAnim = "body_shirt_hot01_kanim";
        
        public override EquipmentDef CreateEquipmentDef()
        {
            return CreateAttachmentDef(Id, IconAnim, EquippedAnim, new List<AttributeModifier>
            {
                new AttributeModifier("Digging", 10.0f, DESC)
            });
        }

        public override void DoPostConfigure(GameObject go)
        {
            base.DoPostConfigure(go);
            go.AddOrGet<MultitoolAttachment>().toolType = MultitoolAttachment.AttachmentType.SuperSpeed;
        }
    }
}