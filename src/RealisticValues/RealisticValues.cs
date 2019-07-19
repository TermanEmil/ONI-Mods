using System;
using Harmony;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace RealisticValues
{
    internal class RealisticValues
    {
        public class CarbonSkimmerBalances
        {
            [HarmonyPatch(typeof(CO2ScrubberConfig), "CreateBuildingDef")]
            public class CarbonSkimmerHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.120f;
                }
            }

            [HarmonyPatch(typeof(CO2ScrubberConfig), "ConfigureBuildingTemplate")]
            public class CarbonSkimmerValuePatch
            {
                public static void Postfix(ref GameObject go)
                {
                    var elementConverter = go.AddOrGet<ElementConverter>();
                    elementConverter.outputElements = new[]
                    {
                        new ElementConverter.OutputElement(1.3f, SimHashes.DirtyWater, 0f, false, true)
                    };
                }
            }
        }

        public class OxygenGenBalances
        {
            // Dumb Algae Terrarium is broken
            //[HarmonyPatch(typeof(AlgaeHabitatConfig), "ConfigureBuildingTemplate")]


            [HarmonyPatch(typeof(MineralDeoxidizerConfig), "CreateBuildingDef")]
            public class DeoxidizerHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.040f;
                    __result.SelfHeatKilowattsWhenActive = 0.080f;
                }
            }

            [HarmonyPatch(typeof(MineralDeoxidizerConfig), "ConfigureBuildingTemplate")]
            public class DeoxidizerOutputPatch
            {
                public static void Postfix(ref GameObject go)
                {
                    var elementConverter = go.AddOrGet<ElementConverter>();
                    elementConverter.outputElements = new[]
                    {
                        new ElementConverter.OutputElement(0.5f, SimHashes.Oxygen, 0f, false, false,
                            0f, 1f, 0.909f),
                        new ElementConverter.OutputElement(0.05f, SimHashes.ContaminatedOxygen, 0f, false,
                            false, 1f,
                            1f, 0.09f)
                    };
                }
            }

            [HarmonyPatch(typeof(AirFilterConfig), "ConfigureBuildingTemplate")]
            public class DeodorizerElementPatch
            {
                public static void Postfix(ref GameObject go)
                {
                    var elementConverter = go.AddOrGet<ElementConverter>();
                    elementConverter.outputElements = new[]
                    {
                        new ElementConverter.OutputElement(0.143333346f, SimHashes.ToxicSand, 0f, false,
                            true, 0f, 0.5f, 0.25f),
                        elementConverter.outputElements[1]
                    };
                }
            }

            [HarmonyPatch(typeof(ElectrolyzerConfig), "ConfigureBuildingTemplate")]
            public class ElectrolyzerOutputPatch
            {
                public static void Postfix(ref GameObject go)
                {
                    var elementConverter = go.AddOrGet<ElementConverter>();
                    elementConverter.outputElements = new[]
                    {
                        new ElementConverter.OutputElement(0.888f, SimHashes.Oxygen, 0f, false, false,
                            0f, 1f),
                        new ElementConverter.OutputElement(0.111999989f, SimHashes.Hydrogen, 0f, false,
                            false, 0f, 1f)
                    };
                }
            }

            [HarmonyPatch(typeof(ElectrolyzerConfig), "CreateBuildingDef")]
            public class ElectrolyzerTempPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.024f;
                    __result.SelfHeatKilowattsWhenActive = 0.096f;
                }
            }
        }

        public class CoalGeneratorPatches
        {
            [HarmonyPatch(typeof(GeneratorConfig), "CreateBuildingDef")]
            public class GeneratorEnergyPatches
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.GeneratorWattageRating = 1000.0f;
                }
            }
        }

        public class HydrogenGeneratorPatches
        {
            [HarmonyPatch(typeof(HydrogenGeneratorConfig), "CreateBuildingDef")]
            public class GeneratorHeatPatches
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.200f;
                    __result.SelfHeatKilowattsWhenActive = 0.200f;
                }
            }
        }

        public class PlumbingPatches
        {
            [HarmonyPatch(typeof(FlushToiletConfig), "ConfigureBuildingTemplate")]
            public class ToiletMassPatch
            {
                public static void Postfix(ref GameObject go)
                {
                    var flushToilet = go.AddOrGet<FlushToilet>();
                    flushToilet.massEmittedPerUse = 6.5f;
                }
            }

            [HarmonyPatch(typeof(ShowerConfig), "ConfigureBuildingTemplate")]
            public class ShowerMassPatch
            {
                public static void Postfix(ref GameObject go)
                {
                    var elementConverter = go.AddOrGet<ElementConverter>();
                    elementConverter.outputElements = new[]
                    {
                        new ElementConverter.OutputElement(1.3f, SimHashes.DirtyWater, 0f, false, true)
                    };
                }
            }

            [HarmonyPatch(typeof(LiquidPumpConfig), "CreateBuildingDef")]
            public class LiquidPumpHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.240f;
                }
            }


            [HarmonyPatch(typeof(LiquidFilterConfig), "CreateBuildingDef")]
            public class LiquidFilterHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.120f;
                }
            }

            [HarmonyPatch(typeof(LiquidMiniPumpConfig), "CreateBuildingDef")]
            public class LiquidMiniPumpHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.060f;
                }
            }

            [HarmonyPatch(typeof(LiquidLogicValveConfig), "CreateBuildingDef")]
            public class LiquidLogicValveHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.010f;
                }
            }
        }

        public class FoodPatches
        {
            [HarmonyPatch(typeof(MicrobeMusherConfig), "CreateBuildingDef")]
            public class MusherHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.048f;
                    __result.SelfHeatKilowattsWhenActive = 0.192f;
                }
            }

            [HarmonyPatch(typeof(CookingStationConfig), "CreateBuildingDef")]
            public class GrillHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.012f;
                    __result.SelfHeatKilowattsWhenActive = 0.048f;
                }
            }

            [HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureBuildingTemplate")]
            public class GasRangeCo2Patch
            {
                public static void Postfix(ref GameObject go)
                {
                    var elementConverter = go.AddOrGet<ElementConverter>();
                    elementConverter.outputElements = new[]
                    {
                        new ElementConverter.OutputElement(0.100f, SimHashes.CarbonDioxide, 348.15f, false,
                            false, 0f, 3f)
                    };
                }
            }

            [HarmonyPatch(typeof(RefrigeratorConfig), "CreateBuildingDef")]
            public class RefrigeratorHeatPatches
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.120f;
                }
            }
        }

        public class RefinementPatches
        {
            [HarmonyPatch(typeof(WaterPurifierConfig), "CreateBuildingDef")]
            public class WaterPurifierHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.120f;
                }
            }

            [HarmonyPatch(typeof(WaterPurifierConfig), "ConfigureBuildingTemplate")]
            public class WaterPurifierDirtPatch
            {
                public static void Postfix(ref GameObject go)
                {
                    var elementConverter = go.AddOrGet<ElementConverter>();
                    elementConverter.outputElements = new[]
                    {
                        new ElementConverter.OutputElement(1.0f, SimHashes.ToxicSand, 0f, false, true,
                            0f, 0.5f, 0.25f),
                        new ElementConverter.OutputElement(3.84615f, SimHashes.Water, 0f, false, true,
                            0f, 0.5f, 0.75f)
                    };
                }
            }

            [HarmonyPatch(typeof(AlgaeDistilleryConfig), "CreateBuildingDef")]
            public class AlgaeDistilleryHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.040f;
                    __result.SelfHeatKilowattsWhenActive = 0.080f;
                }
            }

            [HarmonyPatch(typeof(AlgaeDistilleryConfig), "ConfigureBuildingTemplate")]
            public class AlgaeDistilleryOutputPatch
            {
                public static void Postfix(ref GameObject go)
                {
                    var elementConverter = go.AddOrGet<ElementConverter>();
                    elementConverter.outputElements = new[]
                    {
                        elementConverter.outputElements[0],
                        new ElementConverter.OutputElement(0.35f, SimHashes.DirtyWater, 0f, false, true,
                            0f, 0.5f, 0.875f),
                        new ElementConverter.OutputElement(0.05f, SimHashes.Clay, 0f, false, false,
                            0f, 0f, 0.125f)
                    };
                }
            }
        }


        public class DuplicantBalancePatches
        {
            [HarmonyPatch(typeof(MinionConfig), "CreatePrefab")]
            public class MinionBreathPatch
            {
                public static void Postfix(ref GameObject __result)
                {
                    __result.AddOrGet<OxygenBreather>().O2toCO2conversion = 0.25f;
                    __result.AddOrGet<OxygenBreather>().minCO2ToEmit = 0.100f;
                }
            }

            [HarmonyPatch(typeof(CO2Manager), "SpawnBreath")]
            public class BreathModifier
            {
                public static void Postfix(Vector3 position, float mass, float temperature)
                {
                    var cell = Grid.CellAbove(Grid.PosToCell(position));
                    SimMessages.AddRemoveSubstance(cell, SimHashes.Oxygen,
                        CellEventLogger.Instance.OxygenModifierSimUpdate,
                        mass, temperature + 0.911547f, byte.MaxValue, 0);
                    Console.WriteLine("Adding warmer oxygen: " + mass + "kg at " + temperature + 0.911547f +
                                      "degrees Kelvin");
                }
            }
        }
    }
}