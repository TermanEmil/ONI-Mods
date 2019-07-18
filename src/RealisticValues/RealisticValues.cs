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
        public class WaterPurifierPatches
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
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        var operand = codes[i].operand as int?;
                        if (operand?.Equals(869554203) ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 1.0f);
                        else if (operand?.Equals(1836671383) ?? false)
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 3.84615f);
                    }

                    return codes;
                }
            }
        }

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
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if (!((codes[i].operand as int?)?.Equals(1832607973) ?? false)) continue;
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 1.3f);
                        break;
                    }

                    return codes;
                }
            }
        }

        public class OxygenGenBalances
        {
            // Dumb Algae Terrarium is broken
            /*[HarmonyPatch(typeof(AlgaeHabitatConfig), "ConfigureBuildingTemplate")]
            public class AlgaeTerrariumRatesPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as int?)?.Equals(1832607973) ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.29333333f);
                            Console.WriteLine(i + "Patching with ldc.r4 0.2933333");
                        }
                        else if ((codes[i].operand as int?)?.Equals(1960575215) ?? false)
                        {
                            codes[i + 3] = new CodeInstruction(OpCodes.Ldc_R4, 0.00333333f);
                            Console.WriteLine(i + "Patching with ldc.r4 0.0033333");
                        }
                    }

                    return codes;
                }
            }*/

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
                        elementConverter.outputElements[0],
                        new ElementConverter.OutputElement(0.05f, SimHashes.ContaminatedOxygen, 0f, false, false, 1f, 1f, 0.09f)
                    };
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if (!((codes[i].operand as int?)?.Equals(-1528777920) ?? false)) continue;
                        codes[i + 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.0f);
                        codes[i + 6] = new CodeInstruction(OpCodes.Ldc_R4, 0.909f);
                        break;
                    }

                    return codes;
                }
            }

            [HarmonyPatch(typeof(AirFilterConfig), "ConfigureBuildingTemplate")]
            public class DeodorizerElementPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if (!((codes[i].operand as int?)?.Equals(867327137) ?? false)) continue;
                        codes[i] = new CodeInstruction(OpCodes.Ldc_I4, (int)SimHashes.ToxicSand);
                        break;
                    }

                    return codes;
                }
            }

            [HarmonyPatch(typeof(ElectrolyzerConfig), "ConfigureBuildingTemplate")]
            public class ElectrolyzerOutputPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                        if ((codes[i].operand as int?)?.Equals(-1528777920) ?? false)
                            codes[i + 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.0f);
                        else if ((codes[i].operand as int?)?.Equals(-1046145888) ?? false)
                            codes[i + 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.0f);
                    return codes;
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
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if (!((codes[i].operand as FieldInfo)?.Name.Equals("massEmittedPerUse") ?? false)) continue;
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 6.5f);
                        break;
                    }

                    return codes;
                }
            }
        }

        public class ShowerPatches
        {
            [HarmonyPatch(typeof(ShowerConfig), "ConfigureBuildingTemplate")]
            public class ShowerMassPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if (!((codes[i].operand as FieldInfo)?.Name.Equals("capacityKg") ?? false)) continue;
                        codes[i - 18] = new CodeInstruction(OpCodes.Ldc_R4, 1.3f);
                        Console.WriteLine("Thing found at" + i);
                    }

                    return codes;
                }
            }
        }

        public class MicrobeMusherPatches
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
        }

        public class GrillPatches
        {
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
        }

        public class GasGrillPatches
        {
            [HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureBuildingTemplate")]
            public class GasRangeCo2Patch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (var i = 0; i < codes.Count; ++i)
                    {
                        if (!((codes[i].operand as int?)?.Equals(1960575215) ?? false)) continue;
                        codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.100f);
                        break;
                    }

                    return codes;
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