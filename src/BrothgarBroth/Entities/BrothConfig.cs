using UnityEngine;
using static BrothgarBroth.BROTHSTRINGS.ITEMS.FOOD;

namespace BrothgarBroth.Entities
{
    public class BrothConfig : IEntityConfig
    {
        public const  string Id   = "asquared31415_" + nameof(BrothConfig);
        private const string Anim = "brothgar_broth_kanim";
        public static Tag    Tag  = (Tag) Id;

        // 50g phosphorus
        public const float PhosKg = 0.05f;

        public GameObject CreatePrefab()
        {
            var entityConfig = EntityTemplates.CreateLooseEntity(
                Id,
                ASQUARED31415_BROTHCONFIG.NAME,
                ASQUARED31415_BROTHCONFIG.DESC,
                1f,
                true,
                Assets.GetAnim(Anim),
                "object",
                Grid.SceneLayer.Front,
                EntityTemplates.CollisionShape.CIRCLE,
                0.25f,
                0.25f,
                true
            );

            entityConfig.AddTag(Tag);
            return entityConfig;
        }

        public void OnPrefabInit(GameObject go)
        {
            go.AddOrGet<BrothgarBroth>();
        }

        public void OnSpawn(GameObject go) { }
    }
}
