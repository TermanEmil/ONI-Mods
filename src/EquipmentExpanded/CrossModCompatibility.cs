using System;
using EquipmentExpanded.Buildings;
using EquipmentExpanded.Equipment.ModIntegrations;

namespace EquipmentExpanded
{
    public class CrossModCompatibility
    {
        public static void CheckAndRunAll()
        {
            Debug.Log("Other Mod Things");
            var owoSlicksterType =
                Type.GetType(
                    "ILoveSlicksters.OwO_OilFloaterBabyConfig, ILoveSlicksters, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (owoSlicksterType != null)
            {
                Debug.Log("I Love Slicksters is enabled! Yay!");
                EquipmentConfigManager.Instance.RegisterEquipment(new OwOSlicksterSuit());
                CustomSuitFabricatorConfig.RegisterRecipe(
                    new[]
                    {
                        new ComplexRecipe.RecipeElement(((SimHashes) Hash.SDBMLower("Antigel")).CreateTag(), 250f),
                        new ComplexRecipe.RecipeElement(SimHashes.Ethanol.CreateTag(), 150f),
                        //new ComplexRecipe.RecipeElement(ILoveSlicksters.OwO_OilFloaterBabyConfig.ID.Replace("Baby","").ToTag(), 1f), 
                    },
                    new[]
                    {
                        new ComplexRecipe.RecipeElement(OwOSlicksterSuit.Id.ToTag(), 1f)
                    },
                    "TESTING"
                );
            }
            else
            {
                Debug.Log("No I Love Slicksters D:");
            }
        }
    }
}