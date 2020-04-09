using System.Collections.Generic;
using BrothgarBroth.Entities;
using TUNING;
using UnityEngine;

namespace BrothgarBroth.Buildings
{
    public class BrothBrewerConfig : IBuildingConfig
    {
        public const  string Id   = "asquared31415_" + nameof(BrothBrewerConfig);
        private const string Anim = "hottub_kanim";
        public static Tag    Tag  = (Tag) Id;

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                Id,
                5,
                2,
                Anim,
                40,
                30f,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER3,
                MATERIALS.RAW_METALS,
                1600f,
                BuildLocationRule.OnFloor,
                BUILDINGS.DECOR.PENALTY.TIER1,
                NOISE_POLLUTION.NONE
            );

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefabTag)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.AddOrGet<DropAllWorkable>();
            go.AddOrGet<Prioritizable>();
            var fabricator = go.AddOrGet<ComplexFabricator>();
            fabricator.resultState = ComplexFabricator.ResultState.Heated;
            fabricator.heatedTemperature = 318.15f;
            fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            go.AddOrGet<FabricatorIngredientStatusManager>();
            go.AddOrGet<CopyBuildingSettings>();
            go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new[]
                                                                     {
                                                                         Assets.GetAnim(
                                                                             "anim_interacts_suit_fabricator_kanim"
                                                                         )
                                                                     };

            Prioritizable.AddRef(go);
            BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
            ConfigureRecipes();
        }

        private static void ConfigureRecipes()
        {
            ComplexRecipe.RecipeElement[] brothIngredients =
            {
                // 50g phosphorus
                new ComplexRecipe.RecipeElement(SimHashes.Phosphorus.CreateTag(), 0.05f),
                new ComplexRecipe.RecipeElement(SimHashes.Algae.CreateTag(), 0.05f),
                new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 0.05f),
                new ComplexRecipe.RecipeElement(SimHashes.DirtyWater.CreateTag(), 0.85f)
            };

            ComplexRecipe.RecipeElement[] brothResult = {new ComplexRecipe.RecipeElement(BrothConfig.Tag, 1f)};

            var _ = new ComplexRecipe(
                        ComplexRecipeManager.MakeRecipeID(BrothBrewerConfig.Id, brothIngredients, brothResult),
                        brothIngredients,
                        brothResult
                    )
                    {
                        time = 30f,
                        description = BROTHSTRINGS.ITEMS.FOOD.ASQUARED31415_BROTHCONFIG.RECIPE_DESC,
                        nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                        fabricators = new List<Tag> {BrothBrewerConfig.Tag}
                    };
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<KPrefabID>().prefabSpawnFn += (KPrefabID.PrefabFn) (game_object =>
            {
                var component = game_object.GetComponent<ComplexFabricatorWorkable>();
                component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
                component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
                component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
                component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
                component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            });
        }
    }
}
