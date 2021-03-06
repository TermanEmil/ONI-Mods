﻿using System;
using UnityEngine;

namespace EdibleDuplicants
{
    public class DuplicantMeatConfig : IEntityConfig
    {
        public const string Id = "asquared31415.DuplicantMeat";
        public const string Name = "Mystery Meat";

        public const string Desc =
            "Meat from an... unknown source.  It is quite tough, and other duplicants are rightfully wary of it.";

        public static Tag MeatTag = new Tag(Id);

        // THIS IS CAL, NOT KCAL
        // 10000 kcal
        private const float Calories = 10000000f;

        public GameObject CreatePrefab()
        {
            var entity = EntityTemplates.CreateLooseEntity(Id, Name, Desc, 1f, false,
                Assets.GetAnim("meallicegrain_kanim"), "object", Grid.SceneLayer.Front,
                EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, true);
            var foodInfo = new EdiblesManager.FoodInfo(Id, Calories, Int32.MinValue, 255.15f, 277.15f, 2400f, true);
            EntityTemplates.ExtendEntityToFood(entity, foodInfo);
            return entity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}