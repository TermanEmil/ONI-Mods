using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace RealisticValues
{
    class RealisticValuesIL
    {
        public class WaterPurifierPatches
        {
            [HarmonyPatch(typeof(WaterPurifierConfig), "CreateBuildingDef")]
            public class WaterPurifierHeatPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name?.Equals("SelfHeatKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.120f);
                            break;
                        }
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
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        int? operand = codes[i].operand as int?;
                        if (operand?.Equals(869554203) ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 1.0f);
                        }
                        else if (operand?.Equals(1836671383) ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 3.84615f);
                        }
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
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name?.Equals("SelfHeatKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.120f);
                            break;
                        }
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
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as int?)?.Equals(1832607973) ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 1.3f);
                            break;
                        }
                    }
                    return codes;
                }
            }
        }

        public class ElectrolyzerBalances
        {
            [HarmonyPatch(typeof(ElectrolyzerConfig), "ConfigureBuildingTemplate")]
            public class ElectrolyzerOutputPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as int?)?.Equals(-1528777920) ?? false)
                        {
                            codes[i + 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.0f);
                        }
                        else if ((codes[i].operand as int?)?.Equals(-1046145888) ?? false)
                        {
                            codes[i + 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.0f);
                        }
                    }
                    return codes;

                }
            }

            [HarmonyPatch(typeof(ElectrolyzerConfig), "CreateBuildingDef")]
            public class ElectrolyzerTempPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name?.Equals("ExhaustKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.024f);
                        }
                        else if ((codes[i].operand as FieldInfo)?.Name?.Equals("SelfHeatKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.096f);
                        }
                    }
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
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name?.Equals("GeneratorWattageRating") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 1000.0f);
                        }
                    }
                    return codes;
                }
            }
        }

        public class HydrogenGeneratorPatches
        {
            [HarmonyPatch(typeof(HydrogenGeneratorConfig), "CreateBuildingDef")]
            public class GeneratorEnergyPatches
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name?.Equals("ExhaustKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.200f);
                        }
                        else if ((codes[i].operand as FieldInfo)?.Name?.Equals("SelfHeatKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.200f);
                        }
                    }
                    return codes;
                }
            }
        }

        public class FlushToiletPatches
        {
            [HarmonyPatch(typeof(FlushToiletConfig), "ConfigureBuildingTemplate")]
            public class GeneratorEnergyPatches
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name?.Equals("massEmittedPerUse") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 6.5f);
                        }
                    }
                    return codes;
                }
            }
        }

        public class ShowerPatches
        {
            [HarmonyPatch(typeof(ShowerConfig), "ConfigureBuildingTemplate")]
            public class GeneratorEnergyPatches
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name?.Equals("capacityKg") ?? false)
                        {
                            codes[i - 18] = new CodeInstruction(OpCodes.Ldc_R4, 1.3f);
                        }
                    }
                    return codes;
                }
            }
        }

        public class MicrobeMusherPatches
        {
            [HarmonyPatch(typeof(MicrobeMusherConfig), "CreateBuildingDef")]
            public class GeneratorEnergyPatches
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name?.Equals("ExhaustKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.048f);
                        }
                        else if ((codes[i].operand as FieldInfo)?.Name?.Equals("SelfHeatKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.192f);
                        }
                    }
                    return codes;
                }
            }
        }

        public class GrillPatches
        {
            [HarmonyPatch(typeof(CookingStationConfig), "CreateBuildingDef")]
            public class GeneratorEnergyPatches
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as FieldInfo)?.Name?.Equals("ExhaustKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.012f);
                        }
                        else if ((codes[i].operand as FieldInfo)?.Name?.Equals("SelfHeatKilowattsWhenActive") ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.048f);
                        }
                    }
                    return codes;
                }
            }
        }

        public class GasGrillPatches
        {
            [HarmonyPatch(typeof(GourmetCookingStationConfig), "ConfigureBuildingTemplate")]
            public class GeneratorEnergyPatches
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        if ((codes[i].operand as int?)?.Equals(1960575215) ?? false)
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.100f);
                        }
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
                    for (int i = 0; i < codes.Count; ++i)
                    {
                        FieldInfo fieldInfo = codes[i].operand as FieldInfo;
                        if ((fieldInfo?.Name?.Equals("O2toCO2conversion") ?? false) || (fieldInfo?.Name?.Equals("minCO2ToEmit") ?? false))
                        {
                            codes[i - 1] = new CodeInstruction(OpCodes.Ldc_R4, 0.25f);
                        }
                    }
                    return codes;
                }
            }

            [HarmonyPatch(typeof(CO2Manager), "SpawnBreath")]
            public class BreathModifier
            {
                public static void Postfix(Vector3 position, float mass, float temperature)
                {
                    CustomBreather.SpawnHotO2(position, mass*3, temperature);
                }
            }
        }
    }
}
