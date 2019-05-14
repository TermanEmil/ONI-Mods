using System.IO;
using System.Reflection;
using System.Text;
using Harmony;
using Newtonsoft.Json;

namespace FartFrequently
{
    class FartFrequently
    {
        [HarmonyPatch(typeof(Flatulence),"OnPrefabInit")]
        public class Flatulence_OnPrefabInit_Patches
        {
            public static void Prefix()
            {
                ConfigReader conf = new ConfigReader();
                conf.SetFromConfig();
                Traverse.Create(typeof(TUNING.TRAITS)).Field("FLATULENCE_EMIT_INTERVAL_MIN").SetValue(conf.min);
                Traverse.Create(typeof(TUNING.TRAITS)).Field("FLATULENCE_EMIT_INTERVAL_MAX").SetValue(conf.max);
            }
        }

        public class ConfigReader
        {
            public float min;
            public float max;

            public static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"/config.json";

            public ConfigReader()
            {
                min = 10f;
                max = 40f;
            }

            public void SetFromConfig()
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        var fs = File.Create(path);
                        var text = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(this));
                        fs.Write(text, 0, text.Length);
                        fs.Dispose();
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
                        if(min > max)
                        {
                           Debug.Log("[FartFrequently]: (Config Loader) The minimum value is greater than the maximum, this may cause strange behavior");
                        }
                    }
                }
                catch
                {
                    min = 10f;
                    max = 40f;
                    Debug.Log("[FartFrequently]: (Config Loader) An error occured, please ensure you are using only numerical values in the config file");
                }
            }
        }
    }
}
