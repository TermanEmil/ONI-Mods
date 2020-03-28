using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

namespace EquipmentExpanded.Equipment
{
    public abstract class MultitoolAttachmentConfig : IEquipmentConfig
    {
        public const string DefaultAnimIcon = "shirt_cold01_kanim";
        public const string DefaultEquipAnim = "body_shirt_cold01_kanim";

        public static EquipmentDef CreateAttachmentDef(string id = "asquared31415_" + nameof(MultitoolAttachmentConfig), string iconAnim = DefaultAnimIcon, string equipAnim = DefaultEquipAnim,
            List<AttributeModifier> modifiers = null, Action<Equippable> onEquip = null)
        {
            var equipmentDef = EquipmentTemplates.CreateEquipmentDef(
                id,
                ExpandedAssignableSlots.ToolAttachmentId,
                // TODO: why???
                SimHashes.Carbon,
                10f,
                iconAnim,
                // TODO: what does this do?
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
            return CreateAttachmentDef();
        }

        public virtual void DoPostConfigure(GameObject go)
        {
            go.AddOrGet<Equippable>();
            go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
        }
    }
}