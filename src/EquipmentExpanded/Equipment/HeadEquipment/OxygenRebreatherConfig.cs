using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

namespace EquipmentExpanded.Equipment.HeadEquipment
{
    public class OxygenRebreatherConfig : HeadEquipmentConfig
    {
        public const string Id = "asquared31415_" + nameof(OxygenRebreatherConfig);
        public const string Icon = "shirt_decor01_kanim";
        public const string Anim = "body_shirt_decor01_kanim";
        
        public override EquipmentDef CreateEquipmentDef()
        {
            return CreateHeadAttachment(Id, Icon, Anim, new List<AttributeModifier>
            {
                new AttributeModifier("AirConsumptionRate", -0.1f, "TODO: THING", true)
            });
        }

        public override void DoPostConfigure(GameObject go)
        {
            base.DoPostConfigure(go);
        }
    }
}