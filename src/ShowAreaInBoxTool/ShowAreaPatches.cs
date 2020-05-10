using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace ShowAreaInBoxTool
{
    public class ShowAreaPatches
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[ShowAreaInBoxTool] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );
        }

        public static void PostPatch(HarmonyInstance harmonyInstance)
        {
            var transpiler = typeof(ShowAreaPatches).GetMethod("DrawerTranspiler");
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                                 .Where(type => type.IsSubclassOf(typeof(HoverTextConfiguration)));

            foreach(var hoverType in types)
            {
                var methodInfo = AccessTools.Method(hoverType, "UpdateHoverElements");
                harmonyInstance.Patch(methodInfo, null, null, new HarmonyMethod(transpiler));
            }
        }

        public static IEnumerable<CodeInstruction> DrawerTranspiler(IEnumerable<CodeInstruction> orig)
        {
            var codes = orig.ToList();
            var endDrawing = AccessTools.Method(typeof(HoverTextDrawer), nameof(HoverTextDrawer.EndShadowBar));

            for(var i = 0; i < codes.Count; ++i)
            {
                var ci = codes[i];
                if(ci.opcode == OpCodes.Callvirt && (MethodInfo)ci.operand == endDrawing)
                {
                    var drawPanel = AccessTools.Method(typeof(ShowAreaPatches), "DrawPanel");
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
                    codes.Insert(i, new CodeInstruction(OpCodes.Call, drawPanel));

                    return codes;
                }
            }

            Debug.LogError("[DragToolArea] Patching info cards failed!");
            return codes;
        }

        public static void DrawPanel(HoverTextConfiguration instance)
        {
            var toolInst = ToolMenu.Instance;
            if(toolInst == null)
                return;

            var tool = toolInst.activeTool;
            if(tool != null && tool is DragTool dragTool)
                if(dragTool.GetMode() == DragTool.Mode.Box)
                {
                    var roundedSizeVector = Vector2I.zero;

                    if(dragTool.Dragging)
                    {
                        var sizeVector = dragTool.areaVisualizerSpriteRenderer.size;
                        roundedSizeVector = new Vector2I(
                            Mathf.RoundToInt(sizeVector.x),
                            Mathf.RoundToInt(sizeVector.y)
                        );
                    }

                    const string areaFormat = "Selection Size: {0} x {1} : {2} cells";
                    var text = string.Format(
                        areaFormat,
                        roundedSizeVector.x,
                        roundedSizeVector.y,
                        roundedSizeVector.x * roundedSizeVector.y
                    );

                    var drawer = HoverTextScreen.Instance.drawer;
                    drawer.NewLine();
                    drawer.DrawText(text, instance.Styles_Title.Standard);
                }
        }

        [HarmonyPatch(typeof(DragTool), "OnMouseMove")]
        public class DragTool_OnMouseMove_Patches
        {
            private const string AreaFormat = "{0} x {1} : {2}";

            public static void Postfix(ref DragTool __instance)
            {
                if(__instance.GetMode() != DragTool.Mode.Box)
                    return;

                var sizeVector = __instance.areaVisualizerSpriteRenderer.size;
                if(__instance.areaVisualizerText == Guid.Empty)
                    return;

                var roundedSizeVector = new Vector2I(Mathf.RoundToInt(sizeVector.x), Mathf.RoundToInt(sizeVector.y));
                NameDisplayScreen.Instance.GetWorldText(__instance.areaVisualizerText).GetComponent<LocText>().text =
                    string.Format(
                        AreaFormat,
                        roundedSizeVector.x,
                        roundedSizeVector.y,
                        roundedSizeVector.x * roundedSizeVector.y
                    );
            }
        }
    }
}
