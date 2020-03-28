using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

namespace EquipmentExpanded.Equipment.HeadEquipment
{
    public class HeadEquipmentConfig : IEquipmentConfig
    {
        public const string DefaultAnim = "";
        public const string DefaultIcon = "";

        public EquipmentDef CreateHeadAttachment(string id = "asquared31415_" + nameof(HeadEquipmentConfig),
            string iconAnim = DefaultIcon,
            string equipAnim = DefaultAnim,
            List<AttributeModifier> modifiers = null,
            System.Action<Equippable> onEquip = null)
        {
            var equipmentDef = EquipmentTemplates.CreateEquipmentDef(
                id,
                ExpandedAssignableSlots.HeadAttachmentId,
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
            return CreateHeadAttachment();
        }

        public virtual void DoPostConfigure(GameObject go)
        {
            go.AddOrGet<Equippable>();
            go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
        }
    }
}