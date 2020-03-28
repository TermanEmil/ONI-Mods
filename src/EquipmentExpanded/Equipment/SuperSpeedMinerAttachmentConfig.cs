using System.Collections.Generic;
using Klei.AI;
using UnityEngine;
using static EquipmentExpanded.MULTITOOLSSTRINGS.EQUIPMENT.PREFABS.ASQUARED31415_SUPERSPEEDMINERATTACHMENTCONFIG;

namespace EquipmentExpanded.Equipment
{
    public class SuperSpeedMinerAttachmentConfig : MultitoolAttachmentConfig
    {
        public const string IconAnim = "shirt_hot01_kanim";
        public const string EquippedAnim = "body_shirt_hot01_kanim";
        
        public override EquipmentDef CreateEquipmentDef()
        {
            return CreateAttachmentDef("asquared31415_" + nameof(SuperSpeedMinerAttachmentConfig), IconAnim, EquippedAnim, new List<AttributeModifier>
            {
                new AttributeModifier("Digging", 10.0f, DESCRIPTION)
            });
        }

        public override void DoPostConfigure(GameObject go)
        {
            base.DoPostConfigure(go);
            go.AddOrGet<MultitoolAttachment>().toolType = MultitoolAttachment.AttachmentType.SuperSpeed;
        }
    }
}