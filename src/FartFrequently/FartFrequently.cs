using System.IO;
using System.Collections;
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
        [HarmonyPatch(typeof(Flatulence), "OnPrefabInit")]
        public class Flatulence_OnPrefabInit_Patches
        {
            public static void Prefix()
            {
                ConfigReader conf = new ConfigReader();
                conf.SetFromConfig();
                Traverse.Create(typeof(TUNING.TRAITS)).Field("FLATULENCE_EMIT_INTERVAL_MIN").SetValue(conf.min);
                Traverse.Create(typeof(TUNING.TRAITS)).Field("FLATULENCE_EMIT_INTERVAL_MAX").SetValue(conf.max);
                var harmony = HarmonyInstance.Create("asquared31415.FartFrequently");
                harmony.Patch((MethodBase)Traverse.Create(typeof(Flatulence)).Method("Emit").GetValue(), null, null, new HarmonyMethod(typeof(Flatulence_Emit_Transpiler).GetMethod("Transpiler")));
            }
        }
        

        public class Flatulence_Emit_Transpiler
        {
            public static float GasEmitAmount = 0.1f;

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> instList = instructions.ToList();
                for(var i=0;i<instList.Count;i++)
                {
                    Debug.Log(i + ": " + instList[i]);
                }
                instList[93] = new CodeInstruction(OpCodes.Ldsfld, typeof(Flatulence_Emit_Transpiler).GetField("GasEmitAmount"));
                for (var i = 0; i < instList.Count; i++)
                {
                    Debug.Log(i + ": " + instList[i]);
                }
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
                        Traverse.Create(typeof(Flatulence_Emit_Transpiler)).Field("GasEmitAmount").SetValue(emitAmount);
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
