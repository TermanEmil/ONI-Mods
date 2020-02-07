using System;
using System.Diagnostics;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace ShowAreaInBoxTool
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[ShowAreaInBoxTool] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}");
        }
    }

    [HarmonyPatch(typeof(DragTool), nameof(DragTool.OnMouseMove))]
    public class ShowAreaPatches
    {
        public static string AreaFormat = "{0} x {1} : {2}";

        public static void Postfix(ref DragTool __instance)
        {
            if ((DragTool.Mode) Traverse.Create(__instance).Method("GetMode").GetValue() != DragTool.Mode.Box) return;

            var sizeVector = ((SpriteRenderer) AccessTools.Field(typeof(DragTool), "areaVisualizerSpriteRenderer")
                .GetValue(__instance)).size;
            var visualizerText = AccessTools.Field(typeof(DragTool), "areaVisualizerText");
            if ((Guid) visualizerText.GetValue(__instance) == Guid.Empty) return;
            var roundedSizeVector =
                new Vector2I(Mathf.RoundToInt(sizeVector.x), Mathf.RoundToInt(sizeVector.y));
            var text = NameDisplayScreen.Instance.GetWorldText((Guid) visualizerText.GetValue(__instance))
                .GetComponent<LocText>();
            text.text = string.Format(AreaFormat, roundedSizeVector.x, roundedSizeVector.y,
                roundedSizeVector.x * roundedSizeVector.y);
        }
    }
}