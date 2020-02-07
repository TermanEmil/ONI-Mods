using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace PriorityZero
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[PriorityZero] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}");
        }
    }

    public static class Util
    {
        public const string ZeroTool = "zeroTool.png";
        public const string ZeroPriority = "zeroPriority.png";
        public static readonly string ModPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
    }

    [HarmonyPatch(typeof(Chore), MethodType.Constructor, typeof(ChoreType), typeof(ChoreProvider), typeof(bool),
        typeof(Action<Chore>), typeof(Action<Chore>), typeof(Action<Chore>), typeof(PriorityScreen.PriorityClass),
        typeof(int), typeof(bool), typeof(bool), typeof(int), typeof(bool), typeof(ReportManager.ReportType))]
    public class Chore_ctor_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
        {
            var patched = false;
            foreach (var instruction in origCode)
                if (!patched && instruction.opcode == OpCodes.Blt)
                {
                    patched = true;
                    instruction.operand = 0;
                    yield return instruction;
                }
                else
                {
                    yield return instruction;
                }

            if (!patched) Debug.LogWarning("[PriorityZero] Unable to find Chore patch offset.");
        }
    }

    [HarmonyPatch(typeof(MinionTodoChoreEntry), nameof(MinionTodoChoreEntry.Apply))]
    public class MinionTodoChoreEntry_Apply_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
        {
            var patched = false;
            foreach (var instruction in origCode)
                if (!patched && instruction.opcode == OpCodes.Ldc_I4_1)
                {
                    patched = true;
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                }
                else
                {
                    yield return instruction;
                }

            if (!patched) Debug.LogWarning("[PriorityZero] Unable to find TodoChoreEntry offset.");
        }
    }

    [HarmonyPatch(typeof(MinionTodoSideScreen), nameof(MinionTodoSideScreen.priorityInfo), MethodType.Getter)]
    public class MinionTodoSideScreen_priorityInfo_Patch
    {
        private static readonly JobsTableScreen.PriorityInfo PriorityZeroInfo =
            new JobsTableScreen.PriorityInfo(-2, CreatePriZeroSprite(), "Priority Zero");

        private static Sprite CreatePriZeroSprite()
        {
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(File.ReadAllBytes(Path.Combine(Util.ModPath, Util.ZeroPriority)));
            return Sprite.Create(tex, new Rect(0f, 0f, 100f, 100f), Vector2.zero);
        }

        public static void Postfix(MinionTodoSideScreen __instance, List<JobsTableScreen.PriorityInfo> __result)
        {
            if (__result.Contains(PriorityZeroInfo)) return;

            __result.Add(PriorityZeroInfo);
            Traverse.Create(__instance).Field("_priorityInfo").SetValue(__result);
        }
    }

    [HarmonyPatch(typeof(Prioritizable), nameof(Prioritizable.SetMasterPriority))]
    public class Prioritizable_SetMasterPriority_Patch
    {
        public static void Prefix(Prioritizable __instance, ref PrioritySetting priority)
        {
            if (priority.priority_value == 0)
            {
                priority.priority_class = (PriorityScreen.PriorityClass) (-2);
                priority.priority_value = -200;
            }
        }
    }


    [HarmonyPatch(typeof(PrioritizeTool), "OnPrefabInit")]
    public class PrioritizeTool_OnPrefabInit_Patch
    {
        public static void Postfix(PrioritizeTool __instance)
        {
            var zeroTexture = new Texture2D(2, 2);
            zeroTexture.LoadImage(File.ReadAllBytes(Path.Combine(Util.ModPath, Util.ZeroTool)));
            var newCursors = __instance.cursors.ToList();
            newCursors.Insert(0, zeroTexture);
            __instance.cursors = newCursors.ToArray();
        }
    }


    [HarmonyPatch(typeof(PrioritizeTool), nameof(PrioritizeTool.Update))]
    public class PrioritizeTool_Update_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
        {
            var patched = false;
            foreach (var instruction in origCode)
                if (!patched && instruction.opcode == OpCodes.Sub)
                {
                    patched = true;
                    yield return new CodeInstruction(OpCodes.Pop);
                }
                else
                {
                    yield return instruction;
                }

            if (!patched) Debug.LogWarning("[PriorityZero] Unable to find priority cursor offset.");
        }
    }

    [HarmonyPatch(typeof(PriorityScreen), nameof(PriorityScreen.InstantiateButtons))]
    public class PriorityScreen_InstantiateButtons_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
        {
            var patched = false;
            foreach (var inst in origCode)
                if (!patched && inst.opcode == OpCodes.Ldc_I4_1)
                {
                    patched = true;
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                }
                else
                {
                    yield return inst;
                }

            if (!patched) Debug.Log("[PriorityZero] Error finding Priority Screen Buttons offset.");
        }
    }

    [HarmonyPatch(typeof(PriorityScreen), nameof(PriorityScreen.SetScreenPriority))]
    public class PriorityScreen_SetScreenPriority_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
        {
            var patched = false;
            var preLdloc0 = false;
            foreach (var instruction in origCode)
                if (!patched)
                {
                    if (instruction.opcode == OpCodes.Ldloc_0)
                    {
                        preLdloc0 = true;
                        yield return instruction;
                    }
                    else if (preLdloc0 && instruction.opcode == OpCodes.Ldc_I4_1)
                    {
                        patched = true;
                        yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    }
                    else
                    {
                        preLdloc0 = false;
                        yield return instruction;
                    }
                }
                else
                {
                    yield return instruction;
                }

            if (!patched) Debug.LogWarning("[PriorityZero] Unable to find set Priority Screen offset.");
        }
    }
}