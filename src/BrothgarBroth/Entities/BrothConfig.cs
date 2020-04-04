using System.Collections.Generic;
using UnityEngine;

namespace BrothgarBroth.Entities
{
    public class BrothConfig : IEntityConfig
    {
        public const  string Id   = "asquared31415_" + nameof( BrothConfig );
        public const  string Name = "Brothgar Broth";
        public const  string Desc = "An energy drink composed of questionable ingredients.";
        private const string Anim = "meallicegrain_kanim";
        public static Tag Tag = (Tag) Id;

        // 50g phosphorus
        public const float PhosKg = 0.05f;

        // This is in cal, not kcal
        private const float Calories = 100_000;

        public GameObject CreatePrefab()
        {
            var entityConfig = EntityTemplates.CreateLooseEntity(
                Id,
                Name,
                Desc,
                1f,
                true,
                Assets.GetAnim( Anim ),
                "object",
                Grid.SceneLayer.Front,
                EntityTemplates.CollisionShape.CIRCLE,
                0.25f,
                0.25f,
                true
            );

            var foodInfo =
                new EdiblesManager.FoodInfo( Id, Calories, 0, 255.15f, 277.15f, 2400f, true ).AddEffects(
                    new List<string> {BrothEffects.StaminaEffectId, BrothEffects.SpeedEffectId}
                );

            EntityTemplates.ExtendEntityToFood( entityConfig, foodInfo );
            return entityConfig;
        }

        public void OnPrefabInit( GameObject inst ) { inst.AddOrGet<BrothgarBroth>(); }

        public void OnSpawn( GameObject inst ) { return; }
    }
}
