using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;
using Harmony;
using Newtonsoft.Json;

namespace FartFrequently
{
    class FartFrequently
    {
        [HarmonyPatch(typeof(SplashMessageScreen), "OnPrefabInit")]
        public class SplashMessageScreen_OnPrefabInit_Patches
        {
            public static ConfigReader conf = new ConfigReader();
            public static FileSystemWatcher watcher = new FileSystemWatcher();

            public static void Postfix()
            {
                watcher.Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                watcher.NotifyFilter = NotifyFilters.LastWrite;

                // Add event handlers.
                watcher.Changed += OnChanged;

                // Begin watching.
                watcher.EnableRaisingEvents = true;
                Flatulence_OnPrefabInit_Patches.SetValues();
            }

            private static void OnChanged(object source, FileSystemEventArgs a)
            {
                Flatulence_OnPrefabInit_Patches.SetValues();
            }
        }

        [HarmonyPatch(typeof(Flatulence), "OnPrefabInit")]
        public class Flatulence_OnPrefabInit_Patches
        {
            public static void Prefix(Flatulence __instance)
            {
                SetValues();
            }

            public static void SetValues()
            {
                SplashMessageScreen_OnPrefabInit_Patches.conf.SetFromConfig();
                TUNING.TRAITS.FLATULENCE_EMIT_INTERVAL_MIN = SplashMessageScreen_OnPrefabInit_Patches.conf.min;
                TUNING.TRAITS.FLATULENCE_EMIT_INTERVAL_MAX = SplashMessageScreen_OnPrefabInit_Patches.conf.max;
                Flatulence_Emit_Transpiler.GasEmitAmount = SplashMessageScreen_OnPrefabInit_Patches.conf.emitAmount;
                var harmony = HarmonyInstance.Create("asquared31415.FartFrequently");
                harmony.Patch(AccessTools.Method(typeof(Flatulence), "Emit"), null, null, new HarmonyMethod(typeof(Flatulence_Emit_Transpiler).GetMethod("Transpiler")));
                Debug.Log("[FartFrequently]: (Config Loader) The farting config has been changed to " + SplashMessageScreen_OnPrefabInit_Patches.conf.emitAmount + "Kg at a " + SplashMessageScreen_OnPrefabInit_Patches.conf.min + "-" + SplashMessageScreen_OnPrefabInit_Patches.conf.max + " interval");
            }
        }
        

        public class Flatulence_Emit_Transpiler
        {
            public static float GasEmitAmount = 0.1f;

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> instList = instructions.ToList();
                instList[93] = new CodeInstruction(OpCodes.Ldsfld, typeof(Flatulence_Emit_Transpiler).GetField("GasEmitAmount"));
                return instList.AsEnumerable();
            }
        }
        
        public class ConfigReader
        {
            public float min;
            public float max;
            public float emitAmount;

            public static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"/config.json";

            public ConfigReader()
            {
                min = 10f;
                max = 40f;
                emitAmount = 0.1f;
            }

            public void SetFromConfig()
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        using (var fs = File.Create(path))
                        {
                            var text = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(this));
                            fs.Write(text, 0, text.Length);
                        }
                    }
                    else
                    {
                        string json = "";
                        using (var sr = new StreamReader(path))
                        {
                            json = sr.ReadToEnd();
                        }
                        var newConf = JsonConvert.DeserializeObject<ConfigReader>(json);
                        min = newConf.min;
                        max = newConf.max;
                        emitAmount = newConf.emitAmount;
                        if(min > max)
                        {
                           Debug.Log("[FartFrequently]: (Config Loader) The minimum value is greater than the maximum, this may cause strange behavior");
                        }
                        if(emitAmount <= 0)
                        {
                            emitAmount = 0.1f;
                            Debug.Log("[FartFrequently]: (Config Loader) The emit amount is set to a negative or zero value, resetting to 0.1");
                        }
                    }
                }
                catch
                {
                    min = 10f;
                    max = 40f;
                    emitAmount = 0.1f;
                    Debug.Log("[FartFrequently]: (Config Loader) An error occured, please ensure you are using only numerical values in the config file");
                }
            }
        }
    }
}
