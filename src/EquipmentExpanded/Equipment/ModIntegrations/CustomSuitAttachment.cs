using System.Collections.Generic;
using EquipmentExpanded.Equipment.HeadEquipment;
using Klei.AI;
using UnityEngine;

namespace EquipmentExpanded.Equipment.ModIntegrations
{
    public class CustomSuitAttachment : IEquipmentConfig
    {
        public const string DefaultIcon = "suit_jetpack_kanim";
        public const string DefaultAnim = "body_jetpack_kanim";
        
        public EquipmentDef CreateCustomSuit(string id = "asquared31415_" + nameof(HeadEquipmentConfig),
            string iconAnim = DefaultIcon,
            string equipAnim = DefaultAnim,
            List<AttributeModifier> modifiers = null,
            System.Action<Equippable> onEquip = null)
        {
            var equipmentDef = EquipmentTemplates.CreateEquipmentDef(
                id,
                ExpandedAssignableSlots.CustomSuitId,
                SimHashes.Carbon,
                10f,
                iconAnim,
                null,
                equipAnim,
                4,
                modifiers ?? new List<AttributeModifier>()
            );
            equipmentDef.OnEquipCallBack = onEquip;

            return equipmentDef;
        }
        
        public virtual EquipmentDef CreateEquipmentDef()
        {
            return CreateCustomSuit();
        }

        public virtual void DoPostConfigure(GameObject go)
        {
            go.AddOrGet<Equippable>();
            go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
        }
    }
}