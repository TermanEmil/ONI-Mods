using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Harmony;
using UnityEngine;

namespace HeavyBreathing
{
    class HeavyBreathing
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
                CO2Manager_SpawnBreath_Patches.SetValues();
            }

            private static void OnChanged(object source, FileSystemEventArgs a)
            {
                CO2Manager_SpawnBreath_Patches.SetValues();
            }
        }

        [HarmonyPatch(typeof(CO2Manager), "SpawnBreath")]
        public class CO2Manager_SpawnBreath_Patches
        {
            public static float emitAmount = 0.002f;

            public static void Prefix(ref float mass)
            {
                Debug.Log(mass);
                mass = emitAmount;
                Debug.Log(mass);
            }

            public static void SetValues()
            {
                SplashMessageScreen_OnPrefabInit_Patches.conf.SetFromConfig();
                CO2Manager_SpawnBreath_Patches.emitAmount = SplashMessageScreen_OnPrefabInit_Patches.conf.emitAmount;
                Debug.Log("[Heavy Breathing]: (Config Loader) The emit amount has been changed to " + SplashMessageScreen_OnPrefabInit_Patches.conf.emitAmount + "Kg");
            }
        }

        public class ConfigReader
        {
            public float emitAmount;

            public static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/config.json";

            public ConfigReader()
            {
                emitAmount = 0.002f;
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
                        emitAmount = newConf.emitAmount;
                        if (emitAmount <= 0)
                        {
                            emitAmount = 0.002f;
                            Debug.Log("[Heavy Breathing]: (Config Loader) The emit amount is set to a negative or zero value, resetting to 0.02 Kg");
                        }
                    }
                }
                catch
                {
                    emitAmount = 0.002f;
                    Debug.Log("[Heavy Breathing]: (Config Loader) An error occured, please ensure you are using only numerical values in the config file");
                }
            }
        }
    }
}
