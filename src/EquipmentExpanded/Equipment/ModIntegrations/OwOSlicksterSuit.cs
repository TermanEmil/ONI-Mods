using System.Collections.Generic;
using Klei.AI;

namespace EquipmentExpanded.Equipment.ModIntegrations
{
    public class OwOSlicksterSuit : CustomSuitAttachment
    {
        public const string Id = "asquared31415_" + nameof(OwOSlicksterSuit);
        public const string Icon = "suit_jetpack_kanim";
        public const string Anim = "body_jetpack_kanim";
        
        public override EquipmentDef CreateEquipmentDef()
        {
            return CreateCustomSuit(Id, Icon, Anim, new List<AttributeModifier>
            {
                new AttributeModifier("QualityOfLifeExpectation", -7f, "TODO: OWO")
            });
        }
    }
}