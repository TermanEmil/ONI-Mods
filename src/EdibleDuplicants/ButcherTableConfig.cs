using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using EQUIPMENT = TUNING.EQUIPMENT;

namespace EdibleDuplicants
{
    public class ButcherTableConfig : IBuildingConfig
    {
        public const string Id = "ButcherTable";
        public static string DisplayName = "Butcher Table";

        public static string Description = "Butchers the corpses of dead duplicants." +
                                           "  Duplicants that perform \"work\" here will definitely not have fun, but sometimes… you need to eat.";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id, 3, 2, "clothingfactory_kanim", 100, 120,
                new[] {100f}, MATERIALS.ALL_METALS, 473.15f, BuildLocationRule.OnFloor, DECOR.PENALTY.TIER2,
                NOISE_POLLUTION.NOISY.TIER0);
            buildingDef.AudioCategory = "Metal";
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<CopyBuildingSettings>();
            //var butchering = go.AddOrGet<ButcherTable>();
            var fabricator = go.AddOrGet<ComplexFabricator>();
            var workable = go.AddOrGet<ComplexFabricatorWorkable>();
            workable.overrideAnims = new[]
            {
                Assets.GetAnim("anim_interacts_clothingfactory_kanim")
            };
            workable.AnimOffset = new Vector3(-1f, 0.0f, 0.0f);
            fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
            BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);

            var ingredients = new[]
                {new ComplexRecipe.RecipeElement(GameTags.Minion, DUPLICANTSTATS.DEFAULT_MASS)};
            var results = new[] {new ComplexRecipe.RecipeElement(DuplicantMeatConfig.MeatTag, 20f)};
            // 1 30kg corpse to 20kg meat
            // Should we keep this?
            var recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(Id, ingredients, results), ingredients,
                results)
            {
                time = EQUIPMENT.VESTS.WARM_VEST_FABTIME,
                description = STRINGS.EQUIPMENT.PREFABS.WARM_VEST.RECIPE_DESC,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag> {(Tag) Id},
                sortOrder = 1
            };
        }
    }
}