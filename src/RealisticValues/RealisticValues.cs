using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace RealisticValues
{
    internal class RealisticValues
    {
        public class CarbonSkimmerBalances
        {
            [HarmonyPatch(typeof(CO2ScrubberConfig), "CreateBuildingDef")]
            public class CarbonSkimmerHeatPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if (!((codes[i].operand as FieldInfo)?.Name.Equals("SelfHeatKilowattsWhenActive") ?? false))
                            continue;
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.120f);
                        break;
                    }

                    return codes;
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
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name.Equals("ExhaustKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.040f);
                        else if ((codes[i].operand as FieldInfo)?.Name.Equals("SelfHeatKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.080f);
                    }

                    return codes;
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
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                        if ((codes[i].operand as FieldInfo)?.Name.Equals("ExhaustKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.024f);
                        else if ((codes[i].operand as FieldInfo)?.Name.Equals("SelfHeatKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.096f);
                    return codes;
                }
            }
        }

        public class CoalGeneratorPatches
        {
            [HarmonyPatch(typeof(GeneratorConfig), "CreateBuildingDef")]
            public class GeneratorEnergyPatches
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                        if ((codes[i].operand as FieldInfo)?.Name.Equals("GeneratorWattageRating") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 1000.0f);
                    return codes;
                }
            }
        }

        public class HydrogenGeneratorPatches
        {
            [HarmonyPatch(typeof(HydrogenGeneratorConfig), "CreateBuildingDef")]
            public class GeneratorHeatPatches
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                        if ((codes[i].operand as FieldInfo)?.Name.Equals("ExhaustKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.200f);
                        else if ((codes[i].operand as FieldInfo)?.Name.Equals("SelfHeatKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.200f);
                    return codes;
                }
            }
        }

        public class FlushToiletPatches
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
        }

        public class ShowerPatches
        {
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
        }

        public class FoodPatches
        {
            [HarmonyPatch(typeof(MicrobeMusherConfig), "CreateBuildingDef")]
            public class MusherHeatPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                        if ((codes[i].operand as FieldInfo)?.Name.Equals("ExhaustKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.048f);
                        else if ((codes[i].operand as FieldInfo)?.Name.Equals("SelfHeatKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.192f);
                    return codes;
                }
            }

            [HarmonyPatch(typeof(CookingStationConfig), "CreateBuildingDef")]
            public class GrillHeatPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                        if ((codes[i].operand as FieldInfo)?.Name.Equals("ExhaustKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.012f);
                        else if ((codes[i].operand as FieldInfo)?.Name.Equals("SelfHeatKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.048f);
                    return codes;
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
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                        if ((codes[i].operand as FieldInfo)?.Name.Equals("ExhaustKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.120f);
                    return codes;
                }
            }
        }

        public class RefinementPatches
        {
            [HarmonyPatch(typeof(WaterPurifierConfig), "CreateBuildingDef")]
            public class WaterPurifierHeatPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if (!((codes[i].operand as FieldInfo)?.Name.Equals("SelfHeatKilowattsWhenActive") ?? false))
                            continue;
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.120f);
                        break;
                    }

                    return codes;
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
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                        if ((codes[i].operand as FieldInfo)?.Name.Equals("ExhaustKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.040f);
                        else if ((codes[i].operand as FieldInfo)?.Name.Equals("SelfHeatKilowattsWhenActive") ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.080f);
                    return codes;
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
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        var fieldInfo = codes[i].operand as FieldInfo;
                        if ((fieldInfo?.Name.Equals("O2toCO2conversion") ?? false))
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.25f);
                        if ((fieldInfo?.Name.Equals("minCO2ToEmit") ?? false))
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.100f);
                    }

                    return codes;
                }
            }

            [HarmonyPatch(typeof(CO2Manager), "SpawnBreath")]
            public class BreathModifier
            {
                public static void Postfix(Vector3 position, float mass, float temperature)
                {
                    var cell = Grid.CellAbove(Grid.PosToCell(position));
                    SimMessages.AddRemoveSubstance(cell, SimHashes.Oxygen, CellEventLogger.Instance.OxygenModifierSimUpdate,
                        mass, temperature + 0.911547f, byte.MaxValue, 0);
                    Console.WriteLine("Adding warmer oxygen: " + mass + "kg at " + temperature + 0.911547f + "degrees Kelvin");
                }
            }
        }
    }
}