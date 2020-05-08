using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using STRINGS;
using UnityEngine;

namespace PriorityZero
{
    public class PriorityZero
    {
        public const string                       ZeroPriority      = "PriorityZero.zeroPriority.png";
        public const string                       ZeroTool          = "PriorityZero.zeroTool.png";
        public const PriorityScreen.PriorityClass PriorityZeroClass = (PriorityScreen.PriorityClass)(-2);
        public const int                          PriorityZeroValue = -200;

        public static void OnLoad()
        {
            Debug.Log(
                $"[PriorityZero] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );
        }
    }

    [HarmonyPatch(
        typeof(Chore),
        MethodType.Constructor,
        typeof(ChoreType),
        typeof(ChoreProvider),
        typeof(bool),
        typeof(Action<Chore>),
        typeof(Action<Chore>),
        typeof(Action<Chore>),
        typeof(PriorityScreen.PriorityClass),
        typeof(int),
        typeof(bool),
        typeof(bool),
        typeof(int),
        typeof(bool),
        typeof(ReportManager.ReportType)
    )]
    public class Chore_Ctor_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
        {
            List<CodeInstruction> codes = origCode.ToList();
            foreach(var c in codes)
            {
                if(c.opcode == OpCodes.Blt)
                {
                    c.operand = 0;
                    return codes;
                }
            }

            Debug.LogWarning("[PriorityZero] Unable to find Chore patch offset.");
            return codes;
        }
    }

    [HarmonyPatch(typeof(MinionTodoChoreEntry), nameof(MinionTodoChoreEntry.Apply))]
    public static class MinionTodoChoreEntry_Apply_Patch
    {
        private static readonly Sprite PriorityZeroIcon = CreateIconSprite();

        private static Sprite CreateIconSprite()
        {
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            using var zeroStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(PriorityZero.ZeroPriority);
            using var memStream = new MemoryStream();
            if(zeroStream != null)
            {
                zeroStream.CopyTo(memStream);
                tex.LoadImage(memStream.ToArray());
            }

            return Sprite.Create(tex, new Rect(0f, 0f, 100f, 100f), Vector2.zero);
        }

        public static void Postfix(MinionTodoChoreEntry __instance, Chore.Precondition.Context context)
        {
            if(context.chore.masterPriority.priority_class == PriorityZero.PriorityZeroClass)
            {
                __instance.priorityIcon.sprite = PriorityZeroIcon;
                __instance.priorityLabel.text = "0";
            }
        }
    }

    [HarmonyPatch(typeof(MinionTodoSideScreen), nameof(MinionTodoSideScreen.priorityInfo), MethodType.Getter)]
    public static class MinionTodoSideScreen_PriorityInfo_Patch
    {
        private static readonly JobsTableScreen.PriorityInfo PriorityZeroInfo = new JobsTableScreen.PriorityInfo(
            (int)PriorityZero.PriorityZeroClass,
            CreatePriZeroSprite(),
            "Priority Zero"
        );

        private static Sprite CreatePriZeroSprite()
        {
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            using var zeroStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(PriorityZero.ZeroPriority);
            using var memStream = new MemoryStream();
            if(zeroStream != null)
            {
                zeroStream.CopyTo(memStream);
                tex.LoadImage(memStream.ToArray());
            }

            return Sprite.Create(tex, new Rect(0f, 0f, 100f, 100f), Vector2.zero);
        }

        public static void Postfix(MinionTodoSideScreen __instance, List<JobsTableScreen.PriorityInfo> __result)
        {
            if(__result.Contains(PriorityZeroInfo))
                return;

            __result.Add(PriorityZeroInfo);
            Traverse.Create(__instance).Field("_priorityInfo").SetValue(__result);
        }
    }

    [HarmonyPatch(typeof(PrioritizeTool), "OnPrefabInit")]
    public static class PrioritizeTool_OnPrefabInit_Patch
    {
        public static void Postfix(PrioritizeTool __instance)
        {
            var zeroTexture = new Texture2D(2, 2);
            using var toolStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(PriorityZero.ZeroTool);
            using var memStream = new MemoryStream();
            if(toolStream != null)
            {
                toolStream.CopyTo(memStream);
                zeroTexture.LoadImage(memStream.ToArray());
            }

            List<Texture2D> newCursors = __instance.cursors.ToList();
            newCursors.Insert(0, zeroTexture);
            __instance.cursors = newCursors.ToArray();
        }
    }

    [HarmonyPatch(typeof(PrioritizeTool), nameof(PrioritizeTool.Update))]
    public static class PrioritizeTool_Update_Patch
    {
        public static void Prefix()
        {
            var priority = Traverse.Create(ToolMenu.Instance.PriorityScreen).Field("lastSelectedPriority");
            var value = (PrioritySetting)priority.GetValue();
            if(value.priority_class == PriorityZero.PriorityZeroClass)
                value.priority_value = 1;
            else
                value.priority_value += 1;

            priority.SetValue(value);
        }

        public static void Postfix()
        {
            var priority = Traverse.Create(ToolMenu.Instance.PriorityScreen).Field("lastSelectedPriority");
            var value = (PrioritySetting)priority.GetValue();
            if(value.priority_class == PriorityZero.PriorityZeroClass)
                value.priority_value = PriorityZero.PriorityZeroValue;
            else
                value.priority_value -= 1;

            priority.SetValue(value);
        }
    }

    [HarmonyPatch(typeof(PriorityScreen), nameof(PriorityScreen.InstantiateButtons))]
    public static class PriorityScreen_InstantiateButtons_Patch
    {
        public static void Prefix(PriorityScreen __instance, Action<PrioritySetting> on_click, bool playSelectionSound)
        {
            var buttonPrefab = (PriorityButton)Traverse.Create(__instance).Field("buttonPrefab_basic").GetValue();
            var priorityButton = Util.KInstantiateUI<PriorityButton>(
                buttonPrefab.gameObject,
                buttonPrefab.transform.parent.gameObject
            );

            priorityButton.playSelectionSound = playSelectionSound;
            priorityButton.onClick = on_click;
            priorityButton.text.text = "0";
            priorityButton.priority = new PrioritySetting(
                PriorityZero.PriorityZeroClass,
                PriorityZero.PriorityZeroValue
            );

            priorityButton.tooltip.SetSimpleTooltip(string.Format(UI.PRIORITYSCREEN.BASIC, 0));

            var buttonsField = Traverse.Create(__instance).Field("buttons_basic");
            var buttons = (List<PriorityButton>)buttonsField.GetValue();
            buttons.Insert(0, priorityButton);
            buttonsField.SetValue(buttons);
        }
    }

    [HarmonyPatch(typeof(PriorityScreen), nameof(PriorityScreen.SetScreenPriority))]
    public static class PriorityScreen_SetScreenPriority_Patches
    {
        // TODO: Clean up
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> origCode)
        {
            List<CodeInstruction> codes = origCode.ToList();
            var lab = codes[28].labels;
            codes.RemoveRange(28, 35);
            // This is the NEW 28
            codes[28].labels = lab;
            return codes;
        }
    }
}
