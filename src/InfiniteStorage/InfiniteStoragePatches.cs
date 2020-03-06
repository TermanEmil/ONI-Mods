using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using CaiLib.Utils;
using Harmony;

namespace InfiniteStorage
{
    public class InfiniteStoragePatches
    {
        public static void OnLoad()
        {
            CaiLib.Logger.Logger.LogInit();

            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Base, DeepItemStorage.Id);
            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Base, DeepLiquidStorage.Id);
            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Base, DeepGasStorage.Id);

            // This registers the locstrings
            // No need to STRINGS.Add or use a custom building strings anything 
            LocString.CreateLocStringKeys(typeof(DEEP_STORAGE_STRINGS.BUILDINGS));
            LocString.CreateLocStringKeys(typeof(DEEP_STORAGE_STRINGS.UI));
        }

        [HarmonyPatch(typeof(Storage), nameof(Storage.MassStored))]
        public class Storage_MassStored_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
            {
                var codes = codeInstructions.ToList();
                bool patched = false;

                for (var i = 0; i < codes.Count; ++i)
                {
                    var ci = codes[i];
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (ci.opcode == OpCodes.Ldc_R4 && (float) ci.operand == 1_000f)
                    {
                        // This replaces the code that multiplies and divides by 1000
                        // with code that just uses Math.Round(num, 3)
                        // why didn't you do this Klei?
                        codes[i] = new CodeInstruction(OpCodes.Ldc_I4_3);
                        codes[i + 1] = new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(Math), "Round", new[] {typeof(double), typeof(int)}));
                        codes[i + 2] = new CodeInstruction(OpCodes.Conv_R4);
                        codes[i + 3] = new CodeInstruction(OpCodes.Ret);

                        codes.RemoveRange(i + 4, codes.Count - (i + 4));
                        patched = true;
                        break;
                    }
                }

                if (!patched) Debug.LogWarning("[InfiniteStorage] Unable to patch storage display mass");
                else Debug.Log("[InfiniteStorage] Patched storage display mass.");

#if DEBUG
                foreach (var ci in codes) Console.WriteLine($"{ci.opcode} {ci.operand}");
#endif

                    return codes.AsEnumerable();
            }
        }
    }
}