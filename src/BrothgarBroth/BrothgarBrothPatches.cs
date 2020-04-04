using BrothgarBroth.Entities;
using Harmony;
using Klei.AI;

namespace BrothgarBroth
{
    public static class BrothgarBrothPatches
    {
        public static class ModLoad
        {
            public static void OnLoad() { CaiLib.Logger.Logger.LogInit(); }
        }

        [HarmonyPatch( typeof( Db ), "Initialize" )]
        public static class Db_Initialize_Patches
        {
            public static void Postfix() { BrothEffects.InitializeEffects(); }
        }

        [HarmonyPatch( typeof( Edible ), "OnStopWork" )]
        public static class Edible_OnStopWork_Patches
        {
            public static void Prefix( Edible __instance, Worker worker )
            {
                if ( __instance.GetComponent<BrothgarBroth>() != null )
                {
                    // spawn phos based on how much of it is eaten
                    var element = ElementLoader.FindElementByHash( SimHashes.Phosphorus );
                    element.substance.SpawnResource(
                        Grid.CellToPosCCC( Grid.PosToCell( worker.transform.position ), Grid.SceneLayer.Ore ),
                        __instance.unitsConsumed * BrothConfig.PhosKg,
                        __instance.GetComponent<PrimaryElement>().Temperature,
                        byte.MaxValue,
                        0
                    );
                }
            }
        }
    }

    public static class BrothEffects
    {
        public static Effect BrothStaminaEffect;
        public const  string StaminaEffectId = "asquared31415_" + nameof( BrothStaminaEffect );
        public static Effect BrothSpeedEffect;
        public const  string SpeedEffectId = "asquared31415_" + nameof( BrothSpeedEffect );

        public static void InitializeEffects()
        {
            var db = Db.Get();
            ResourceSet<Effect> effects = db.effects;

            BrothStaminaEffect = new Effect( StaminaEffectId, "Energy effect", "energy desc", 100f, true, true, false );
            BrothStaminaEffect.Add( new AttributeModifier( db.Amounts.Stamina.deltaAttribute.Id, +0.1f, "ENERGY" ) );
            effects.Add( BrothStaminaEffect );

            BrothSpeedEffect = new Effect( SpeedEffectId, "Speed effect", "speed desc", 100f, true, true, false );
            BrothSpeedEffect.Add( new AttributeModifier( db.Attributes.Athletics.Id, +7, "SPEED" ) );
            effects.Add( BrothSpeedEffect );
        }
    }
}
