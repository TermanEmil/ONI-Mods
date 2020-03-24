using System;
using System.Diagnostics;
using System.Linq;
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

        public static void PostPatch(HarmonyInstance harmonyInstance)
        {
            var postfix = typeof(ModOnLoad).GetMethod("DrawerPostfix");
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(HoverTextConfiguration)));
            foreach (var hoverType in types)
            {
                var methodInfo = AccessTools.Method(hoverType, "UpdateHoverElements");
                harmonyInstance.Patch(methodInfo, null, new HarmonyMethod(postfix));
            }
        }

        public static void DrawerPostfix(HoverTextConfiguration __instance)
        {
            var tool = ToolMenu.Instance.activeTool;
            var roundedSizeVector = Vector2I.zero;
            if (tool != null && tool.GetType().IsSubclassOf(typeof(DragTool)))
            {
                if ((DragTool.Mode) Traverse.Create(tool).Method("GetMode").GetValue() == DragTool.Mode.Box)
                {
                    if (((DragTool) tool).Dragging)
                    {
                        var sizeVector = ((SpriteRenderer) AccessTools
                            .Field(typeof(DragTool), "areaVisualizerSpriteRenderer")
                            .GetValue(tool)).size;
                        roundedSizeVector = new Vector2I(Mathf.RoundToInt(sizeVector.x), Mathf.RoundToInt(sizeVector.y));
                    }
                }
            }
            
            const string areaFormat = "Selection Size: {0} x {1} : {2} cells";
            var text = string.Format(areaFormat, roundedSizeVector.x, roundedSizeVector.y,
                roundedSizeVector.x * roundedSizeVector.y);
            var drawer = HoverTextScreen.Instance.drawer;
            drawer.BeginShadowBar();
            drawer.DrawText(text, __instance.Styles_Title.Standard);
            drawer.EndShadowBar();
            drawer.EndDrawing();
        }

        [HarmonyPatch(typeof(DragTool), "OnMouseMove")]
        public class ShowAreaPatches
        {
            private const string AreaFormat = "{0} x {1} : {2}";

            public static void Postfix(ref DragTool __instance)
            {
                if ((DragTool.Mode) Traverse.Create(__instance).Method("GetMode").GetValue() !=
                    DragTool.Mode.Box) return;

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
}