using TUNING;
using UnityEngine;

namespace OldCritterWrangler
{
    class OldCritterWranglerConfig : IBuildingConfig
    {
        public static string Id          = "OldCritterWrangler";
        public static string DisplayName = "Elderly Critter Wrangler";

        public static string Description =
            "A Wrangling Station can be used to designate critters to wrangle automatically.";

        public static string Effect = "Allows precise control over the age of critters kept in a room.";

        public override BuildingDef CreateBuildingDef()
        {
            var buildingDef = BuildingTemplates.CreateBuildingDef(
                Id,
                1,
                3,
                "relocator_dropoff_kanim",
                10,
                10f,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER1,
                MATERIALS.RAW_METALS,
                1600f,
                BuildLocationRule.OnFloor,
                BUILDINGS.DECOR.PENALTY.TIER2,
                NOISE_POLLUTION.NOISY.TIER0
            );

            buildingDef.AudioCategory = "Metal";
            buildingDef.ViewMode = OverlayModes.Rooms.ID;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate( GameObject go, Tag prefab_tag )
        {
            go.GetComponent<KPrefabID>().AddTag( RoomConstraints.ConstraintTags.CreatureRelocator );
            var storage = go.AddOrGet<Storage>();
            storage.allowItemRemoval = false;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.BAGABLE_CREATURES;
            storage.workAnims = new HashedString[] {"place", "release"};
            storage.overrideAnims = new[] {Assets.GetAnim( "anim_restrain_creature_kanim" )};
            storage.workAnimPlayMode = KAnim.PlayMode.Once;
            storage.synchronizeAnims = false;
            storage.useGunForDelivery = false;
            storage.allowSettingOnlyFetchMarkedItems = false;
            go.AddOrGet<AgeCritterWrangler>();
        }

        public override void DoPostConfigureComplete( GameObject go ) { }
    }
}
