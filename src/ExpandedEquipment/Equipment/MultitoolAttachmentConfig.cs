using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

namespace ExpandedEquipment.Equipment
{
    public abstract class MultitoolAttachmentConfig : IEquipmentConfig
    {
        public const string IconAnim = "shirt_cold01_kanim";
        public const string Anim0 = "body_shirt_cold01_kanim";

        public static EquipmentDef CreateAttachmentDef(string id = "asquared31415_" + nameof(MultitoolAttachmentConfig), string iconAnim = IconAnim, string anim = Anim0,
            List<AttributeModifier> modifiers = null)
        {
            var equipmentDef = EquipmentTemplates.CreateEquipmentDef(
                id,
                Equipment.ExpandedAssignableSlots.ToolAttachmentId,
                SimHashes.Carbon,
                10f,
                iconAnim,
                // TODO: Can this be null?
                null,
                anim,
                4,
                modifiers ?? new List<AttributeModifier>()
            );

            equipmentDef.OnEquipCallBack = equippable => { Debug.Log("Equipped thingy!"); };

            return equipmentDef;
        }

        public virtual EquipmentDef CreateEquipmentDef()
        {
            return CreateAttachmentDef();
        }

        public virtual void DoPostConfigure(GameObject go)
        {
            go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes, false);
            go.AddOrGet<Equippable>();
            go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
        }
    }
}