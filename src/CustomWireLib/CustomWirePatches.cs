using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;

namespace CustomWireLib
{
    public class CustomWirePatches
    {
        [HarmonyPatch(typeof(Wire), "GetMaxWattageAsFloat")]
        public class WireWattageSwitchPatch
        {
            public static void Postfix(Wire.WattageRating rating, ref float __result)
            {
                if (__result != 0.0f) return;
                var r = CustomWireValues.GetWireRating((int) rating);
                __result = r != -1 ? r : (float)Wire.WattageRating.Max1000;
            }
        }

        [HarmonyPatch(typeof(ElectricalUtilityNetwork), MethodType.Constructor)]
        public class WireIndexFix
        {
            public static void Prefix()
            {
                CustomWireValues.GetAndUpdateWireCount();
            }
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var field = typeof(CustomWireValues).GetField("_newWireCount",
                    BindingFlags.NonPublic | BindingFlags.Static);
                var codes = new List<CodeInstruction>(instr);
                if (field != null)
                    codes[1] = new CodeInstruction(OpCodes.Ldsfld, field);
                else
                    Console.WriteLine("An error occured adding to the wire list.");
                return codes;
            }
        }

        [HarmonyPatch(typeof(ElectricalUtilityNetwork), "UpdateOverloadTime")]
        public class WireOverloadFix
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var field = typeof(CustomWireValues).GetField("_newWireCount",
                    BindingFlags.NonPublic | BindingFlags.Static);
                var codes = new List<CodeInstruction>(instr);
                for (var i = 0; i < codes.Count; ++i)
                {
                    if (!codes[i].opcode.Equals(OpCodes.Ldloc_3)) continue;
                    if (!codes[i + 1].opcode.Equals(OpCodes.Ldc_I4_5)) continue;
                    // Found index count at i + 1
                    codes[i + 1] = new CodeInstruction(OpCodes.Ldsfld, field);
                    return codes;
                }
                Console.WriteLine("An error occured fixing wire overloads.");
                return codes;
            }
        }

        [HarmonyPatch(typeof(ElectricalUtilityNetwork), "Reset")]
        public class WireResetFix
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var field = typeof(CustomWireValues).GetField("_newWireCount",
                    BindingFlags.NonPublic | BindingFlags.Static);
                var codes = new List<CodeInstruction>(instr);
                for (var i = 0; i < codes.Count; ++i)
                {
                    if (!codes[i].opcode.Equals(OpCodes.Ldloc_0)) continue;
                    if (!codes[i + 1].opcode.Equals(OpCodes.Ldc_I4_5)) continue;
                    // Found index count at i + 1
                    codes[i + 1] = new CodeInstruction(OpCodes.Ldsfld, field);
                    return codes;
                }
                Console.WriteLine("An error occured fixing wire resets.");
                return codes;
            }
        }

        [HarmonyPatch(typeof(CircuitManager), "Refresh")]
        public class WireOverloadBridgeFix
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                var field = typeof(CustomWireValues).GetField("_newWireCount",
                    BindingFlags.NonPublic | BindingFlags.Static);
                var codes = new List<CodeInstruction>(instr);
                if (field != null)
                    codes[38] = new CodeInstruction(OpCodes.Ldsfld, field);
                else
                    Console.WriteLine("An error occured fixing wire overloads in bridges.");
                return codes;
            }
        }

        [HarmonyPatch(typeof(LegacyModMain), "LoadBuildings")]
        public class UnRegisterBaseWire
        {
            public static void Prefix(ref List<Type> types)
            {
                types.Remove(typeof(CustomWireMaker.CustomWire));
            }
        }
    }

    
}
