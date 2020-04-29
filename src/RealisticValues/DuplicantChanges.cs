using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace RealisticValues
{
    public static class DuplicantChanges
    {
        public const float O2Conversion = 0.25f;
        public const float MinCo2 = 0.050f;
        public const float MinO2 = 0.150f;

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
        public static class OxygenBreather_Sim200ms_Patches
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();
                var shouldEmit = AccessTools.Method(typeof(OxygenBreather.IGasProvider), "ShouldEmitCO2");
                for(var i = 0; i < codes.Count; ++i)
                {
                    if((MethodInfo)codes[i].operand == shouldEmit)
                    {
                        // Needs to properly accumulate and emit
                        // Probably pass to helper
                        var idx = i + 3;
                        codes.Insert(idx++, new CodeInstruction(OpCodes.Dup));
                    }
                }
            }

            public static void SpawnO2()
            {
                
            }
        }

        [HarmonyPatch(typeof(CO2Manager), "SpawnBreath")]
        public class BreathModifier
        {
            public static void Postfix(CO2Manager __instance, Vector3 position, float mass, float temperature)
            {
                var o2Mass = mass * (1 / O2Conversion) * (1 - O2Conversion);
                Debug.Log($"CO2 Mass: {mass}");
                Debug.Log($"Breath mass: {o2Mass}");
                var cell = Grid.CellAbove(Grid.PosToCell(position));
                //Debug.Log($"{temperature} -> {outTemp}");
                SimMessages.AddRemoveSubstance(
                    cell,
                    SimHashes.Oxygen,
                    CellEventLogger.Instance.OxygenModifierSimUpdate,
                    mass,
                    temperature,
                    byte.MaxValue,
                    0
                );
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
