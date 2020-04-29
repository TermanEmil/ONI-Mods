using System.Collections.Generic;
using Harmony;

namespace RealisticValues
{
    //public class 

    public static class ElementConverterModifiers
    {
        public static readonly Dictionary<string, ElementConverter.ConsumedElement[]> BuildingInputs =
            new Dictionary<string, ElementConverter.ConsumedElement[]>
            {
                //{"CO2Scrubber", new RecipeElement {Element = SimHashes.DirtyWater, Amount = 1f}},
            };

        public static readonly Dictionary<string, ElementConverter.OutputElement[]> BuildingOutputs =
            new Dictionary<string, ElementConverter.OutputElement[]>
            {
                {
                    "CO2Scrubber",
                    new[] {new ElementConverter.OutputElement(1.3f, SimHashes.DirtyWater, 0f, false, true)}
                },
                {
                    "MineralDeoxidizer",
                    new[]
                    {
                        new ElementConverter.OutputElement(0.5f, SimHashes.Oxygen, 0f, false, false, 0f, 1f, 0.909f),
                        new ElementConverter.OutputElement(
                            0.05f,
                            SimHashes.ContaminatedOxygen,
                            0f,
                            false,
                            false,
                            1f,
                            1f,
                            0.09f
                        )
                    }
                },
                {
                    "AirFilter",
                    new[]
                    {
                        new ElementConverter.OutputElement(
                            0.143333346f,
                            SimHashes.ToxicSand,
                            0f,
                            false,
                            true,
                            0f,
                            0.5f,
                            0.25f
                        ),
                        new ElementConverter.OutputElement(
                            0.09f,
                            SimHashes.Oxygen,
                            0.0f,
                            false,
                            false,
                            0.0f,
                            0.0f,
                            0.75f
                        )
                    }
                },
                {
                    "Electrolyzer",
                    new[]
                    {
                        new ElementConverter.OutputElement(0.888f, SimHashes.Oxygen, 0f, false, false, 0f, 1f),
                        new ElementConverter.OutputElement(0.111999989f, SimHashes.Hydrogen, 0f, false, false, 0f, 1f)
                    }
                },
                {
                    "GourmetCookingStation",
                    new[]
                    {
                        new ElementConverter.OutputElement(
                            0.100f,
                            SimHashes.CarbonDioxide,
                            348.15f,
                            false,
                            false,
                            0f,
                            3f
                        )
                    }
                },
                {"Shower", new[] {new ElementConverter.OutputElement(1.3f, SimHashes.DirtyWater, 0f, false, true)}},
                {
                    "WaterPurifier",
                    new[]
                    {
                        new ElementConverter.OutputElement(1.0f, SimHashes.ToxicSand, 0f, false, true, 0f, 0.5f, 0.25f),
                        new ElementConverter.OutputElement(3.84615f, SimHashes.Water, 0f, false, true, 0f, 0.5f, 0.75f)
                    }
                },
                {
                    "AlgaeDistillery",
                    new[]
                    {
                        new ElementConverter.OutputElement(0.2f, SimHashes.Algae, 303.15f, false, true, 0.0f, 1f),
                        new ElementConverter.OutputElement(
                            0.35f,
                            SimHashes.DirtyWater,
                            0f,
                            false,
                            true,
                            0f,
                            0.5f,
                            0.875f
                        ),
                        new ElementConverter.OutputElement(0.05f, SimHashes.Clay, 0f, false, false, 0f, 0f, 0.125f)
                    }
                }
            };

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
                        elementConv.consumedElements = BuildingInputs[buildingDef.PrefabID];

                    if(BuildingOutputs.ContainsKey(buildingDef.PrefabID))
                        elementConv.outputElements = BuildingOutputs[buildingDef.PrefabID];
                }
            }
        }
    }
}
