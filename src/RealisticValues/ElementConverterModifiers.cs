using System.Collections.Generic;
using Harmony;

namespace RealisticValues
{
    public class ElementConverterModifiers
    {
        public static readonly Dictionary<string, RecipeElement> BuildingInputs = new Dictionary<string, RecipeElement>
        {
            {"CO2Scrubber", new RecipeElement {Element = SimHashes.MoltenCopper, Amount = 1f}},
            {"WaterPurifier", new RecipeElement {Element = SimHashes.MoltenSalt, Amount = 25f}},
        };

        public static readonly Dictionary<string, RecipeElement> BuildingOutputs = new Dictionary<string, RecipeElement>
        {
            {"CO2Scrubber", new RecipeElement {Element = SimHashes.Unobtanium, Amount = 1f}},
        };

        public struct RecipeElement
        {
            public SimHashes Element;
            public float Amount;
        }

        [HarmonyPatch(typeof(BuildingConfigManager), nameof(BuildingConfigManager.RegisterBuilding))]
        public static class BuildingConfigManager_RegisterBuilding_Patches
        {
            public static void Postfix(Dictionary<IBuildingConfig, BuildingDef> ___configTable, IBuildingConfig config)
            {
                var buildingDef = ___configTable[config];
                var elementConv = buildingDef.BuildingComplete.GetComponent<ElementConverter>();
                if(elementConv != null)
                {
                    if(BuildingInputs.ContainsKey(buildingDef.PrefabID))
                    {
                        var input = BuildingInputs[buildingDef.PrefabID];
                        elementConv.consumedElements = new[]
                            {new ElementConverter.ConsumedElement(input.Element.CreateTag(), input.Amount)};
                    }

                    if(BuildingOutputs.ContainsKey(buildingDef.PrefabID))
                    {
                        var output = BuildingOutputs[buildingDef.PrefabID];
                        var origOut = elementConv.outputElements[0];
                        origOut.elementHash = output.Element;
                        origOut.massGenerationRate = output.Amount;
                        elementConv.outputElements = new[] {origOut};
                    }
                }
            }
        }
    }
}