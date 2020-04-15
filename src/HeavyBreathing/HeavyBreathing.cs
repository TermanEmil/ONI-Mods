using System.Diagnostics;
using System.IO;
using System.Reflection;
using Harmony;

namespace HeavyBreathing
{
    class HeavyBreathing
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[HeavyBreathing] Loading mod version {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );

            Watcher.Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Watcher.NotifyFilter = NotifyFilters.LastWrite;

            // Add event handlers.
            Watcher.Changed += OnChanged;

            // Begin watching.
            Watcher.EnableRaisingEvents = true;
            Co2ManagerSpawnBreathPatches.SetValues();
        }

        public static readonly  ConfigReader      Conf    = new ConfigReader();
        private static readonly FileSystemWatcher Watcher = new FileSystemWatcher();

        private static void OnChanged(object source, FileSystemEventArgs a)
        {
            Co2ManagerSpawnBreathPatches.SetValues();
        }
    }

    [HarmonyPatch(typeof(CO2Manager), "SpawnBreath")]
    public class Co2ManagerSpawnBreathPatches
    {
        private static float _emitAmount = 0.02f;

        public static void Prefix(ref float mass) { mass = _emitAmount; }

        public static void SetValues()
        {
            HeavyBreathing.Conf.SetFromConfig();
            _emitAmount = HeavyBreathing.Conf.EmitAmount;
            Debug.Log(
                "[Heavy Breathing]: (Config Loader) The emit amount has been changed to " +
                HeavyBreathing.Conf.EmitAmount +
                "Kg"
            );
        }
    }
}
