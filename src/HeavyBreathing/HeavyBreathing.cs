using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Harmony;
using Newtonsoft.Json;

namespace HeavyBreathing
{
    public class ModOnLoad
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[HeavyBreathing] Loading mod version {FileVersionInfo.GetVersionInfo( Assembly.GetExecutingAssembly().Location ).FileVersion}"
            );
        }
    }

    internal class HeavyBreathing
    {
        [HarmonyPatch( typeof( SplashMessageScreen ), "OnPrefabInit" )]
        public class SplashMessageScreenOnPrefabInitPatches
        {
            public static ConfigReader      Conf    = new ConfigReader();
            public static FileSystemWatcher Watcher = new FileSystemWatcher();

            public static void Postfix()
            {
                Watcher.Path = Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );

                Watcher.NotifyFilter = NotifyFilters.LastWrite;

                // Add event handlers.
                Watcher.Changed += OnChanged;

                // Begin watching.
                Watcher.EnableRaisingEvents = true;
                Co2ManagerSpawnBreathPatches.SetValues();
            }

            private static void OnChanged( object source, FileSystemEventArgs a )
            {
                Co2ManagerSpawnBreathPatches.SetValues();
            }
        }

        [HarmonyPatch( typeof( CO2Manager ), "SpawnBreath" )]
        public class Co2ManagerSpawnBreathPatches
        {
            public static float EmitAmount = 0.02f;

            public static void Prefix( ref float mass )
            {
                Debug.Log( mass );
                mass = EmitAmount;
                Debug.Log( mass );
            }

            public static void SetValues()
            {
                SplashMessageScreenOnPrefabInitPatches.Conf.SetFromConfig();
                EmitAmount = SplashMessageScreenOnPrefabInitPatches.Conf.EmitAmount;
                Debug.Log(
                    "[Heavy Breathing]: (Config Loader) The emit amount has been changed to " +
                    SplashMessageScreenOnPrefabInitPatches.Conf.EmitAmount +
                    "Kg"
                );
            }
        }

        public class ConfigReader
        {
            public static string Path = System.IO.Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) +
                                        "/config.json";

            public float EmitAmount;

            public ConfigReader() { EmitAmount = 0.02f; }

            public void SetFromConfig()
            {
                try
                {
                    if ( !File.Exists( Path ) )
                    {
                        using ( var fs = File.Create( Path ) )
                        {
                            var text = new UTF8Encoding( true ).GetBytes( JsonConvert.SerializeObject( this ) );
                            fs.Write( text, 0, text.Length );
                        }
                    }
                    else
                    {
                        string json;
                        using ( var sr = new StreamReader( Path ) )
                        {
                            json = sr.ReadToEnd();
                        }

                        var newConf = JsonConvert.DeserializeObject<ConfigReader>( json );
                        EmitAmount = newConf.EmitAmount;
                        if ( !(EmitAmount <= 0) )
                            return;

                        EmitAmount = 0.02f;
                        Debug.Log(
                            "[Heavy Breathing]: (Config Loader) The emit amount is set to a negative or zero value, resetting to 0.02 Kg"
                        );
                    }
                }
                catch
                {
                    EmitAmount = 0.002f;
                    Debug.Log(
                        "[Heavy Breathing]: (Config Loader) An error occured, please ensure you are using only numerical values in the config file"
                    );
                }
            }
        }
    }
}
