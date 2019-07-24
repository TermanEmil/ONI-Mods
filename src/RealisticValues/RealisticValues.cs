using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace RealisticValues
{
    class RealisticValues
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

        public class VentilationPatches
        {
            [HarmonyPatch(typeof(GasPumpConfig), "CreateBuildingDef")]
            public class GasPumpHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.240f;
                }
            }

            [HarmonyPatch(typeof(GasMiniPumpConfig), "CreateBuildingDef")]
            public class MiniGasPumpHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.060f;
                }
            }

            [HarmonyPatch(typeof(GasFilterConfig), "CreateBuildingDef")]
            public class GasFilterHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.120f;
                }
            }

            [HarmonyPatch(typeof(GasLogicValveConfig), "CreateBuildingDef")]
            public class GasShutoffHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.010f;
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

            [HarmonyPatch(typeof(DesalinatorConfig), "CreateBuildingDef")]
            public class DesalinatorHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.480f;
                }
            }

            [HarmonyPatch(typeof(FertilizerMakerConfig), "CreateBuildingDef")]
            public class FertilizerMakerHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.040f;
                    __result.SelfHeatKilowattsWhenActive = 0.080f;
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

            [HarmonyPatch(typeof(EthanolDistilleryConfig), "CreateBuildingDef")]
            public class EthanolDistilleryHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.02667f;
                    __result.SelfHeatKilowattsWhenActive = 0.21333f;
                }
            }

            [HarmonyPatch(typeof(RockCrusherConfig), "CreateBuildingDef")]
            public class RockGranulatorEnergyPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 560f;
                    __result.SelfHeatKilowattsWhenActive = 0.560f;
                }
            }

            [HarmonyPatch(typeof(MetalRefineryConfig), "CreateBuildingDef")]
            public class MetalRefineryPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 1800f;
                    __result.SelfHeatKilowattsWhenActive = 1.800f;
                }
            }

            [HarmonyPatch(typeof(GlassForgeConfig), "CreateBuildingDef")]
            public class GlassForgePatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 1800f;
                    __result.SelfHeatKilowattsWhenActive = 1.800f;
                }
            }

            [HarmonyPatch(typeof(OilRefineryConfig), "CreateBuildingDef")]
            public class OilRefineryPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 800f;
                    __result.SelfHeatKilowattsWhenActive = 0.160f;
                    __result.ExhaustKilowattsWhenActive = 0.840f;
                }
            }

            [HarmonyPatch(typeof(PolymerizerConfig), "CreateBuildingDef")]
            public class PolymerizerPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 1200f;
                    __result.SelfHeatKilowattsWhenActive = 0.0185f;
                    __result.ExhaustKilowattsWhenActive = 1.1815f;
                }
            }

            [HarmonyPatch(typeof(OxyliteRefineryConfig), "CreateBuildingDef")]
            public class OxyliteRefineryPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 2000f;
                    __result.ExhaustKilowattsWhenActive = 0.6667f;
                    __result.SelfHeatKilowattsWhenActive = 1.3333f;
                }
            }

            [HarmonyPatch(typeof(SupermaterialRefineryConfig), "CreateBuildingDef")]
            public class SupermaterialRefineryPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 2000f;
                    __result.SelfHeatKilowattsWhenActive = 2.000f;
                }
            }
        }

        public class MedicineBalancePatches
        {
            [HarmonyPatch(typeof(HandSanitizer.Work), "OnWorkTick")]
            public class SanitizerSinksPatch
            {
                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
                {
                    var codes = new List<CodeInstruction>(instr);
                    // TODO: find the appropriate method and use that as an offset
                    var start = 101;
                    codes.Insert(start + 0, new CodeInstruction(OpCodes.Dup));
                    codes.Insert(start + 1, new CodeInstruction(OpCodes.Ldc_I4, (int)SimHashes.DirtyWater));
                    codes.Insert(start + 2, new CodeInstruction(OpCodes.Ceq));
                    codes.Insert(start + 3, new CodeInstruction(OpCodes.Brfalse, 0x0D));
                    codes.Insert(start + 4, new CodeInstruction(OpCodes.Ldloc_S, (byte)6));
                    codes.Insert(start + 5, new CodeInstruction(OpCodes.Ldc_R4, 1.3f));
                    codes.Insert(start + 6, new CodeInstruction(OpCodes.Mul));
                    codes.Insert(start + 7, new CodeInstruction(OpCodes.Br, 0x02));
                    return codes;
                }
            }

            [HarmonyPatch(typeof(ApothecaryConfig), "CreateBuildingDef")]
            public class ApothecaryHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.080f;
                    __result.SelfHeatKilowattsWhenActive = 0.160f;
                }
            }

            [HarmonyPatch(typeof(AdvancedDoctorStationConfig), "CreateBuildingDef")]
            public class ClinicHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.160f;
                    __result.SelfHeatKilowattsWhenActive = 0.320f;
                }
            }

            [HarmonyPatch(typeof(MassageTableConfig), "CreateBuildingDef")]
            public class MassageHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.048f;
                    __result.SelfHeatKilowattsWhenActive = 0.192f;
                }
            }
        }

        public class FurnitureBalancePatches
        {
            [HarmonyPatch(typeof(FloorLampConfig), "CreateBuildingDef")]
            public class LampHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.008f;
                }
            }

            [HarmonyPatch(typeof(CeilingLightConfig), "CreateBuildingDef")]
            public class CeilingLightHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.010f;
                }
            }

            [HarmonyPatch(typeof(PhonoboxConfig), "CreateBuildingDef")]
            public class JukebotHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.960f;
                }
            }

            [HarmonyPatch(typeof(ArcadeMachineConfig), "CreateBuildingDef")]
            public class ArcadeHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 1.200f;
                }
            }

            [HarmonyPatch(typeof(EspressoMachineConfig), "CreateBuildingDef")]
            public class EspressoHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.480f;
                }
            }
        }

        public class StationsBalancePatches
        {
            [HarmonyPatch(typeof(ResearchCenterConfig), "CreateBuildingDef")]
            public class ResearchHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.0007f;
                    __result.SelfHeatKilowattsWhenActive = 0.0593f;
                }
            }

            [HarmonyPatch(typeof(AdvancedResearchCenterConfig), "CreateBuildingDef")]
            public class AdvancedResearchHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.01334f;
                    __result.SelfHeatKilowattsWhenActive = 0.1066f;
                }
            }

            [HarmonyPatch(typeof(CosmicResearchCenterConfig), "CreateBuildingDef")]
            public class CosmicResearchHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.01334f;
                    __result.SelfHeatKilowattsWhenActive = 0.1066f;
                }
            }

            [HarmonyPatch(typeof(TelescopeConfig), "CreateBuildingDef")]
            public class TelescopeHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.120f;
                }
            }

            [HarmonyPatch(typeof(ShearingStationConfig), "CreateBuildingDef")]
            public class ShearingStationHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.012f;
                    __result.SelfHeatKilowattsWhenActive = 0.048f;
                }
            }

            [HarmonyPatch(typeof(ResetSkillsStationConfig), "CreateBuildingDef")]
            public class SkillScrubberHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.4266f;
                    __result.SelfHeatKilowattsWhenActive = 0.0534f;
                }
            }

            [HarmonyPatch(typeof(ClothingFabricatorConfig), "CreateBuildingDef")]
            public class ClothingFabricatorHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.240f;
                }
            }

            [HarmonyPatch(typeof(SuitFabricatorConfig), "CreateBuildingDef")]
            public class ExosuitForgeHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.480f;
                }
            }

            [HarmonyPatch(typeof(SuitLockerConfig), "CreateBuildingDef")]
            public class AtmoSuitDockHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.120f;
                }
            }

            [HarmonyPatch(typeof(JetSuitLockerConfig), "CreateBuildingDef")]
            public class JetSuitDockHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.120f;
                }
            }
        }

        public class AutomationBalancePatches
        {
            [HarmonyPatch(typeof(LogicElementSensorGasConfig), "CreateBuildingDef")]
            public class GasSensorHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.025f;
                }
            }

            [HarmonyPatch(typeof(CheckpointConfig), "CreateBuildingDef")]
            public class CheckpointHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.100f;
                }
            }

            [HarmonyPatch(typeof(CometDetectorConfig), "CreateBuildingDef")]
            public class CometHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.120f;
                }
            }
        }

        public class ShippingPatches
        {
            [HarmonyPatch(typeof(SolidTransferArmConfig), "CreateBuildingDef")]
            public class SweeperHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 540f;
                    __result.SelfHeatKilowattsWhenActive = 0.540f;
                }
            }

            [HarmonyPatch(typeof(SolidConduitInboxConfig), "CreateBuildingDef")]
            public class ConveyorLoaderHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 180f;
                    __result.SelfHeatKilowattsWhenActive = 0.180f;
                }
            }

            [HarmonyPatch(typeof(SolidLogicValveConfig), "CreateBuildingDef")]
            public class ConveyorShutoffHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.SelfHeatKilowattsWhenActive = 0.010f;
                }
            }

            [HarmonyPatch(typeof(AutoMinerConfig), "CreateBuildingDef")]
            public class AutoMinerHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.EnergyConsumptionWhenActive = 600f;
                    __result.SelfHeatKilowattsWhenActive = 0.600f;
                }
            }
        }

        public class RocketryBalancePatches
        {
            [HarmonyPatch(typeof(GantryConfig), "CreateBuildingDef")]
            public class GantryHeatPatch
            {
                public static void Postfix(ref BuildingDef __result)
                {
                    __result.ExhaustKilowattsWhenActive = 0.600f;
                    __result.SelfHeatKilowattsWhenActive = 0.600f;
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
                }
            }
        }
    }
}