using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EquipmentExpanded.Items
{
    public class PrintingPodUpgradeConfig : IEntityConfig
    {
        public const string Id = "asquared31415_" + nameof(PrintingPodUpgradeConfig);
        public static Tag Tag = new Tag(Id);

        public const string Name = "Printing Pod Upgrade";
        public const string Desc = "Upgrades the Printing Pod";
        public const string Anim = "meallicegrain_kanim";
        
        public GameObject CreatePrefab()
        {
            var entity = EntityTemplates.CreateLooseEntity(
                Id,
                Name,
                Desc,
                1f,
                true,
                Assets.GetAnim(Anim),
                "object",
                Grid.SceneLayer.Front,
                EntityTemplates.CollisionShape.RECTANGLE,
                0.3f,
                0.3f,
                true,
                0,
                SimHashes.Unobtanium
            );
            
            return entity;
        }

        public void OnPrefabInit(GameObject inst)
        {
            return;
        }

        public void OnSpawn(GameObject inst)
        {
            return;
        }
    }
}