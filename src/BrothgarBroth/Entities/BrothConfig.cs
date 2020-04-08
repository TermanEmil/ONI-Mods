using System.Collections.Generic;
using UnityEngine;
using static BrothgarBroth.BROTHSTRINGS.ITEMS.FOOD;

namespace BrothgarBroth.Entities
{
    public class BrothConfig : IEntityConfig
    {
        public const  string Id   = "asquared31415_" + nameof(BrothConfig);
        private const string Anim = "meallicegrain_kanim";
        public static Tag    Tag  = (Tag) Id;

        // 50g phosphorus
        public const float PhosKg = 0.05f;

        // This is in cal, not kcal
        private const float Calories = 100_000;

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
                0.25f
            );

            var foodInfo = new EdiblesManager.FoodInfo(Id, Calories, 0, 255.15f, 277.15f, 2400f, true).AddEffects(
                new List<string> {BrothEffects.StaminaEffectId, BrothEffects.SpeedEffectId}
            );

            //EntityTemplates.ExtendEntityToFood(entityConfig, foodInfo);
            entityConfig.AddTag(Tag);
            return entityConfig;
        }

        public void OnPrefabInit(GameObject go)
        {
            go.AddOrGet<BrothWorkable>();
        }

        public void OnSpawn(GameObject go)
        {
            
        }
    }
}
