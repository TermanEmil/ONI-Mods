using System;
using BrothgarBroth.Buildings;
using CaiLib.Utils;
using Harmony;
using Klei.AI;
using STRINGS;
using static BrothgarBroth.BROTHSTRINGS.DUPLICANTS.MODIFIERS;

namespace BrothgarBroth
{
    public static class BrothgarBrothPatches
    {
        public static class ModLoad
        {
            public static void OnLoad()
            {
                CaiLib.Logger.Logger.LogInit();
                BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Food, BrothBrewerConfig.Id);
                LocString.CreateLocStringKeys(typeof(BROTHSTRINGS.ITEMS));
                LocString.CreateLocStringKeys(typeof(BROTHSTRINGS.DUPLICANTS));
                LocString.CreateLocStringKeys(typeof(BROTHSTRINGS.BUILDINGS));
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        public static class Db_Initialize_Patches
        {
            public static void Postfix() { BrothEffects.InitializeEffects(); }
        }

        [HarmonyPatch(typeof(ChorePreconditions), MethodType.Constructor, new Type[0])]
        public static class ChorePreconditions_Ctor_Patches
        {
            public static void Postfix() { CustomPreconditions.InitializePreconditions(); }
        }
    }

    public static class CustomPreconditions
    {
        public static Chore.Precondition CanDrinkBrothPrecondition;

        public static void InitializePreconditions()
        {
            CanDrinkBrothPrecondition = new Chore.Precondition
                                        {
                                            id = "asquared31415_" +
                                                 nameof(CustomPreconditions.CanDrinkBrothPrecondition),
                                            description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_DO_RECREATION,
                                            fn = (ref Chore.Precondition.Context context, object data) =>
                                            {
                                                if(context.consumerState.consumer == null)
                                                    return false;

                                                if(!(data is IBrothWorkable workable))
                                                    return false;

                                                return workable.CanConsumeBroth(context.consumerState.worker);
                                            }
                                        };
        }
    }

    public static class BrothEffects
    {
        public static Effect BrothStaminaEffect;
        public const  string StaminaEffectId = "asquared31415_" + nameof(BrothStaminaEffect);
        public static Effect BrothSpeedEffect;
        public const  string SpeedEffectId = "asquared31415_" + nameof(BrothSpeedEffect);
        public static Effect BrothCooldownEffect;
        public const  string CooldownEffectId = "asquared31415_" + nameof(BrothEffects.BrothCooldownEffect);

        public static void InitializeEffects()
        {
            var db = Db.Get();
            ResourceSet<Effect> effects = db.effects;

            BrothStaminaEffect = new Effect(
                StaminaEffectId,
                ASQUARED31415_BROTHSTAMINAEFFECT.NAME,
                ASQUARED31415_BROTHSTAMINAEFFECT.DESCRIPTION,
                330f,
                true,
                true,
                false
            );

            BrothStaminaEffect.Add(
                new AttributeModifier(
                    db.Amounts.Stamina.deltaAttribute.Id,
                    +0.1f,
                    ASQUARED31415_BROTHSTAMINAEFFECT.ATTRDESC
                )
            );

            effects.Add(BrothStaminaEffect);

            BrothSpeedEffect = new Effect(
                SpeedEffectId,
                ASQUARED31415_BROTHSPEEDEFFECT.NAME,
                ASQUARED31415_BROTHSPEEDEFFECT.DESCRIPTION,
                330f,
                true,
                true,
                false
            );

            BrothSpeedEffect.Add(
                new AttributeModifier(db.Attributes.Athletics.Id, +7f, ASQUARED31415_BROTHSPEEDEFFECT.ATTRDESC)
            );

            effects.Add(BrothSpeedEffect);

            BrothCooldownEffect = new Effect(CooldownEffectId, "Broth Cooldown", "", 300f, false, false, false);
            effects.Add(BrothCooldownEffect);
        }
    }
}
