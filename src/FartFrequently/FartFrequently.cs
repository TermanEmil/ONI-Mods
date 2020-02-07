using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Harmony;
using Newtonsoft.Json;
using TUNING;

namespace FartFrequently
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log($"[FartFrequently] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}");
        }
    }

    internal class FartFrequently
    {
        public static bool TryParse<TEnum>(string value, out TEnum result)
            where TEnum : struct, IConvertible
        {
            var retValue = value != null && Enum.IsDefined(typeof(TEnum), value);
            result = retValue ? (TEnum) Enum.Parse(typeof(TEnum), value) : default(TEnum);
            return retValue;
        }

        [HarmonyPatch(typeof(SplashMessageScreen), "OnPrefabInit")]
        public class SplashMessageScreenOnPrefabInitPatches
        {
            public static ConfigReader Conf = new ConfigReader();
            public static FileSystemWatcher Watcher = new FileSystemWatcher();

            public static void Postfix()
            {
                Watcher.Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                Watcher.NotifyFilter = NotifyFilters.LastWrite;

                // Add event handlers.
                Watcher.Changed += OnChanged;

                // Begin watching.
                Watcher.EnableRaisingEvents = true;
                FlatulenceOnPrefabInitPatches.SetValues();
            }

            private static void OnChanged(object source, FileSystemEventArgs a)
            {
                FlatulenceOnPrefabInitPatches.SetValues();
            }
        }

        [HarmonyPatch(typeof(Flatulence), "OnPrefabInit")]
        public class FlatulenceOnPrefabInitPatches
        {
            public static void Prefix(Flatulence __instance)
            {
                SetValues();
            }

            public static void SetValues()
            {
                SplashMessageScreenOnPrefabInitPatches.Conf.SetFromConfig();
                TRAITS.FLATULENCE_EMIT_INTERVAL_MIN = SplashMessageScreenOnPrefabInitPatches.Conf.Min;
                TRAITS.FLATULENCE_EMIT_INTERVAL_MAX = SplashMessageScreenOnPrefabInitPatches.Conf.Max;
                FlatulenceEmitTranspiler.GasEmitAmount = SplashMessageScreenOnPrefabInitPatches.Conf.EmitAmount;
                var harmony = HarmonyInstance.Create("asquared31415.FartFrequently");
                harmony.Patch(AccessTools.Method(typeof(Flatulence), "Emit"), null, null,
                    new HarmonyMethod(typeof(FlatulenceEmitTranspiler).GetMethod("Transpiler")));
                Debug.Log("[FartFrequently]: (Config Loader) The farting config has been changed to emit " +
                          SplashMessageScreenOnPrefabInitPatches.Conf.EmitAmount + "Kg of " +
                          FlatulenceEmitTranspiler.EmitElement + " at a " +
                          SplashMessageScreenOnPrefabInitPatches.Conf.Min + "-" +
                          SplashMessageScreenOnPrefabInitPatches.Conf.Max + " interval");
            }
        }


        public class FlatulenceEmitTranspiler
        {
            public static float GasEmitAmount = 0.1f;
            public static SimHashes EmitElement = SimHashes.Methane;

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instList = instructions.ToList();
                instList[90] = new CodeInstruction(OpCodes.Ldsfld,
                    typeof(FlatulenceEmitTranspiler).GetField("EmitElement"));
                instList[93] = new CodeInstruction(OpCodes.Ldsfld,
                    typeof(FlatulenceEmitTranspiler).GetField("GasEmitAmount"));
                return instList.AsEnumerable();
            }
        }

        public class ConfigReader
        {
            public static string Path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                        "/config.json";

            public string Element;
            public float EmitAmount;
            public float Max;
            public float Min;

            public ConfigReader()
            {
                Min = 10f;
                Max = 40f;
                EmitAmount = 0.1f;
                Element = "Methane";
            }

            public void SetFromConfig()
            {
                try
                {
                    if (!File.Exists(Path))
                    {
                        using (var fs = File.Create(Path))
                        {
                            var text = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(this));
                            fs.Write(text, 0, text.Length);
                        }
                    }
                    else
                    {
                        string json;
                        using (var sr = new StreamReader(Path))
                        {
                            json = sr.ReadToEnd();
                        }

                        var newConf = JsonConvert.DeserializeObject<ConfigReader>(json);
                        Min = newConf.Min;
                        Max = newConf.Max;
                        EmitAmount = newConf.EmitAmount;
                        Element = newConf.Element;
                        if (Min > Max)
                            Debug.Log(
                                "[FartFrequently]: (Config Loader) The minimum value is greater than the maximum, this may cause strange behavior");
                        if (EmitAmount <= 0)
                        {
                            EmitAmount = 0.1f;
                            Debug.Log(
                                "[FartFrequently]: (Config Loader) The emit amount is set to a negative or zero value, resetting to 0.1");
                        }

                        if (!TryParse(Element, out SimHashes elementEnum)) elementEnum = SimHashes.Methane;
                        Debug.Log(elementEnum);
                    }
                }
                catch
                {
                    Min = 10f;
                    Max = 40f;
                    EmitAmount = 0.1f;
                    Element = "Methane";
                    Debug.Log(
                        "[FartFrequently]: (Config Loader) An error occured, please ensure you are using only numerical values in the config file");
                }
            }
        }
    }
}