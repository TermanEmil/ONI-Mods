using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Klei.AI;
using UnityEngine;

namespace RealisticValues
{
    public static class DuplicantChanges
    {
        public const float O2Conversion = 0.25f;
        public const float MinCo2       = 0.100f;
        public const float MinO2        = 0.150f;

        [HarmonyPatch(typeof(MinionConfig), "CreatePrefab")]
        public class MinionBreathPatch
        {
            public static void Postfix(ref GameObject __result)
            {
                __result.AddOrGet<OxygenBreather>().O2toCO2conversion = O2Conversion;
                __result.AddOrGet<OxygenBreather>().minCO2ToEmit = MinCo2;
            }
        }

        [HarmonyPatch(typeof(OxygenBreather), nameof(OxygenBreather.Sim200ms))]
        public static class BreatherPatches
        {
            public static float AccumulatedO2;

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> orig)
            {
                List<CodeInstruction> codes = orig.ToList();
                var shouldEmit = AccessTools.Method(typeof(OxygenBreather.IGasProvider), "ShouldEmitCO2");
                for(var i = 0; i < codes.Count; ++i)
                {
                    if(codes[i].opcode == OpCodes.Callvirt && codes[i].operand is MethodInfo m && m == shouldEmit)
                    {
                        // Needs to properly accumulate and emit
                        // Probably pass to helper
                        var idx = i + 2;
                        // inst
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldarg_0));
                        // mass
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldloc_0));
                        // temp
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldarg_0));
                        var temp = AccessTools.Field(typeof(OxygenBreather), "temperature");
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldfld, temp));
                        var val = AccessTools.Field(typeof(AmountInstance), "value");
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldfld, val));
                        var accumulate = AccessTools.Method(
                            typeof(BreatherPatches),
                            nameof(BreatherPatches.AccumulateO2)
                        );

                        codes.Insert(idx, new CodeInstruction(OpCodes.Call, accumulate));

                        idx = codes.Count - 1;
                        // inst
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldarg_0));
                        // hasAir
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Dup));
                        var hasAir = AccessTools.Field(typeof(OxygenBreather), "hasAir");
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldfld, hasAir));
                        // temp
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldarg_0));
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldfld, temp));
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Ldfld, val));
                        var dumpAll = AccessTools.Method(typeof(BreatherPatches), nameof(BreatherPatches.DumpAll));
                        codes.Insert(idx, new CodeInstruction(OpCodes.Call, dumpAll));

                        return codes;
                    }
                }

                Debug.Log("something wrong");
                return codes;
            }

            private static void DumpAll(OxygenBreather inst, bool hasAir, float temp)
            {
                if(hasAir)
                    return;

                var position = inst.transform.GetPosition();
                position.x += inst.GetComponent<Facing>().GetFacing() ? -inst.mouthOffset.x : inst.mouthOffset.x;
                position.y += inst.mouthOffset.y;
                position.z -= 0.5f;
                var cell = Grid.CellAbove(Grid.PosToCell(position));
                Debug.Log($"Producing {DuplicantChanges.MinO2}");
                SimMessages.AddRemoveSubstance(
                    cell,
                    SimHashes.Oxygen,
                    CellEventLogger.Instance.OxygenModifierSimUpdate,
                    AccumulatedO2,
                    temp,
                    byte.MaxValue,
                    0
                );

                AccumulatedO2 = 0;
            }

            private static void AccumulateO2(OxygenBreather inst, float mass, float temp)
            {
                var toRecycle = mass * (1 - DuplicantChanges.O2Conversion);
                Debug.Log($"Mass: {mass} Recycle: {toRecycle}");
                AccumulatedO2 += toRecycle;
                if(AccumulatedO2 >= DuplicantChanges.MinO2)
                {
                    AccumulatedO2 -= DuplicantChanges.MinO2;
                    var position = inst.transform.GetPosition();
                    position.x += inst.GetComponent<Facing>().GetFacing() ? -inst.mouthOffset.x : inst.mouthOffset.x;
                    position.y += inst.mouthOffset.y;
                    position.z -= 0.5f;
                    var cell = Grid.CellAbove(Grid.PosToCell(position));
                    Debug.Log($"Producing {DuplicantChanges.MinO2}");
                    SimMessages.AddRemoveSubstance(
                        cell,
                        SimHashes.Oxygen,
                        CellEventLogger.Instance.OxygenModifierSimUpdate,
                        DuplicantChanges.MinO2,
                        temp,
                        byte.MaxValue,
                        0
                    );
                }
            }
        }

        public static class ShowOxygenStatusItems
        {
            [HarmonyPatch(typeof(OxygenBreather), "OnSpawn")]
            public static class OxygenBreather_OnSpawn_Patches
            {
                public static void Postfix(OxygenBreather __instance)
                {
                    __instance.GetComponent<KSelectable>().AddStatusItem(
                        ExtraDuplicantStatusItems.EmittingO2Status,
                        __instance
                    );
                }
            }

            [HarmonyPatch(typeof(OxygenBreather), "OnDeath")]
            public static class OxygenBreather_OnDeath_Patches
            {
                public static void Postfix(OxygenBreather __instance)
                {
                    __instance.GetComponent<KSelectable>().RemoveStatusItem(ExtraDuplicantStatusItems.EmittingO2Status);
                }
            }
        }
    }
}
