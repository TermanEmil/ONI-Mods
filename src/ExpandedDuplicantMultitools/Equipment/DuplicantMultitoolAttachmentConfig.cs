using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

namespace ExpandedDuplicantMultitools.Equipment
{
    public class DuplicantMultitoolAttachmentConfig : IEquipmentConfig
    {
        public const string Id       = "asquared31415_" + nameof( DuplicantMultitoolAttachmentConfig );
        public const string IconAnim = "shirt_cold01_kanim";
        public const string Anim0    = "body_shirt_cold01_kanim";

        public EquipmentDef CreateEquipmentDef()
        {
            var equipmentDef = EquipmentTemplates.CreateEquipmentDef(
                Id,
                Equipment.ExpandedAssignableSlots.ToolAttachmentId,
                SimHashes.Carbon,
                10f,
                IconAnim,
                // TODO: Can this be null?
                null,
                Anim0,
                4,
                new List<AttributeModifier>()
            );

            equipmentDef.OnEquipCallBack = equippable => { Debug.Log( "Equipped thingy!" ); };

            return equipmentDef;
        }

        public void DoPostConfigure( GameObject go )
        {
            go.GetComponent<KPrefabID>().AddTag( GameTags.Clothes, false );
            go.AddOrGet<Equippable>();
            //equippable.SetQuality(QualityLevel.Poor);
            go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
        }
    }
}
