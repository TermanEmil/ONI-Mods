using System.Collections.Generic;
using EquipmentExpanded.Equipment;
using TUNING;
using UnityEngine;
using static TUNING.BUILDINGS;
using static EquipmentExpanded.MULTITOOLSSTRINGS.EQUIPMENT.PREFABS;

namespace EquipmentExpanded.Buildings
{
    public class MultitoolAttachmentFabricatorConfig : IBuildingConfig
    {
        public const string Id = "asquared31415_" + nameof(MultitoolAttachmentFabricatorConfig);
        public const string Desc = "TODO DESC";
        public const string Effect = "TODO EFFECT";
        public const string Anim = "suit_maker_kanim";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(Id,
                4,
                3,
                Anim,
                100,
                CONSTRUCTION_TIME_SECONDS.TIER4,
                CONSTRUCTION_MASS_KG.TIER4,
                MATERIALS.REFINED_METALS,
                MELTING_POINT_KELVIN.TIER1,
                BuildLocationRule.OnFloor,
                BUILDINGS.DECOR.PENALTY.TIER0,
                NOISE_POLLUTION.NONE);

            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 360f;
            buildingDef.ViewMode = OverlayModes.Power.ID;
            buildingDef.SelfHeatKilowattsWhenActive = 0.1f;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
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
            
            var fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
            fabricatorWorkable.overrideAnims = new[]
            {
                Assets.GetAnim((HashedString) "anim_interacts_suit_fabricator_kanim")
            };
            fabricatorWorkable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
            fabricatorWorkable.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
            fabricatorWorkable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
            fabricatorWorkable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
            fabricatorWorkable.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
            
            Prioritizable.AddRef(go);
            BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
            
            ConfigureRecipes();
        }

        private static void ConfigureRecipes()
        {
            var neutroniumIngredients = new[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Ceramic.CreateTag(), 500f),
                new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 500f),
                new ComplexRecipe.RecipeElement(SimHashes.Katairite.CreateTag(), 400f)
            };
            var neutroniumResult = new[]
            {
                new ComplexRecipe.RecipeElement(NeutroniumMinerAttachmentConfig.Id.ToTag(), 1f)
            };
            
            var neutroniumMinerRecipe = new ComplexRecipe(
                ComplexRecipeManager.MakeRecipeID(Id, neutroniumIngredients, neutroniumResult), neutroniumIngredients, neutroniumResult)
            {
                time = 40f,
                description = ASQUARED31415_NEUTRONIUMMINERATTACHMENTCONFIG.RECIPE_DESC,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag>
                {
                    (Tag) Id
                }
            };

            var superSpeedIngredients = new[]
            {
                new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 500f),
                new ComplexRecipe.RecipeElement(SimHashes.Katairite.CreateTag(), 250f)
            };
            var superSpeedResult = new[]
            {
                new ComplexRecipe.RecipeElement(SuperSpeedMinerAttachmentConfig.Id.ToTag(), 1f)
            };
            var superSpeedRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID(Id, superSpeedIngredients, superSpeedResult), superSpeedIngredients, superSpeedResult)
            {
                time = 40f,
                description = ASQUARED31415_SUPERSPEEDMINERATTACHMENTCONFIG.RECIPE_DESC,
                nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
                fabricators = new List<Tag>
                {
                    (Tag) Id
                }
            };
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            // Hopefully this allows it to be patched by others?
            return;
        }
    }
}