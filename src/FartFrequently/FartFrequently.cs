using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using TUNING;

namespace FartFrequently
{
    public class FartFrequently
    {
        public static readonly ConfigReader      Conf    = new ConfigReader();
        public static readonly FileSystemWatcher Watcher = new FileSystemWatcher();

        private static void OnChanged(object source, FileSystemEventArgs a)
        {
            FlatulenceOnPrefabInitPatches.SetValues();
        }

        public static void OnLoad()
        {
            Debug.Log(
                $"[FartFrequently] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );

            Watcher.Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Watcher.NotifyFilter = NotifyFilters.LastWrite;

            // Add event handlers.
            Watcher.Changed += OnChanged;

            // Begin watching.
            Watcher.EnableRaisingEvents = true;
            FlatulenceOnPrefabInitPatches.SetValues();
        }
    }

    [HarmonyPatch(typeof(Flatulence), "Emit")]
    public class FlatulenceOnPrefabInitPatches
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();

            for(var i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                if(code.operand is int val)
                    if(val == (int) SimHashes.Methane)
                    {
                        codes.RemoveAt(i);
                        codes.Insert(
                            i++,
                            new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(FartFrequently), "Conf"))
                        );

                        codes.Insert(
                            i++,
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ConfigReader), "ElementHash"))
                        );

                        var idx = codes.FindIndex(i, ci => ci.operand is float v && v == 0.1f);
                        codes[idx++] = new CodeInstruction(
                            OpCodes.Ldsfld,
                            AccessTools.Field(typeof(FartFrequently), "Conf")
                        );

                        codes.Insert(
                            idx,
                            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ConfigReader), "EmitAmount"))
                        );
                    }
            }

            codes.ForEach(Debug.Log);
            return codes;
        }

        public static void SetValues()
        {
            FartFrequently.Conf.SetFromConfig();
            TRAITS.FLATULENCE_EMIT_INTERVAL_MIN = FartFrequently.Conf.Min;
            TRAITS.FLATULENCE_EMIT_INTERVAL_MAX = FartFrequently.Conf.Max;

            Debug.Log(
                "[FartFrequently]: (Config Loader) The farting config has been changed to emit " +
                FartFrequently.Conf.EmitAmount +
                "Kg of " +
                FartFrequently.Conf.Element +
                " at a " +
                FartFrequently.Conf.Min +
                "-" +
                FartFrequently.Conf.Max +
                " interval"
            );
        }
    }
}
