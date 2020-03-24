using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using KMod;
using Newtonsoft.Json;
using Label = System.Reflection.Emit.Label;

namespace ConfigurablePrintingPod
{
    public class ConfigurablePrintingPod
    {
        public static PodConfig Config;

        private static readonly FileSystemWatcher Watcher = new FileSystemWatcher()
        {
            Path = ConfigHelper.ConfigDir,
            Filter = "config.json",
            NotifyFilter = NotifyFilters.LastWrite
        };

        public static void OnLoad()
        {
            Debug.Log(
                $"[ConfigurablePrintingPod] Loading mod version " +
                $"{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );
            Config = ConfigHelper.ReadConfig();
            Watcher.Changed += OnConfigChanged;
            Watcher.EnableRaisingEvents = true;
        }

        private static void OnConfigChanged(object o, FileSystemEventArgs e)
        {
            Debug.Log("[ConfigurablePrintingPod] Detected config file change, updating...");
            Config = ConfigHelper.ReadConfig();
        }
    }

    [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
    public class CharacterSelectionControler_InitializeContainers_Patches
    {
        private static readonly FieldInfo TargetPackageOptions =
            AccessTools.Field(typeof(CharacterSelectionController), "numberOfCarePackageOptions");

        private static readonly FieldInfo TargetDupeOptions =
            AccessTools.Field(typeof(CharacterSelectionController), "numberOfDuplicantOptions");

        private static readonly MethodInfo PackageCount = AccessTools.Method(typeof(CharacterSelectionControler_InitializeContainers_Patches), nameof(GetRandomPackageCount));
        private static readonly MethodInfo DupeCount = AccessTools.Method(typeof(CharacterSelectionControler_InitializeContainers_Patches), nameof(GetRandomDuplicantCount));
        
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> orig)
        {
            var codes = orig.ToList();

            var label = new Label();
            for (var i = 0; i < codes.Count; ++i)
            {
                // Store label for the else branch
                if (codes[i].operand as string == "Enabled")
                {
                    label = (Label) codes[i+2].operand;
                }

                // the br.s is what goes to the end of the if...else
                // Despite the fact that the IL viewer claims it's a br.s, it's a br???
                if (codes[i].opcode == OpCodes.Br)
                {
                    // ... so we start at the next instruction
                    i++;
                    // Search for and remove both stores
                    var first = codes.FindIndex(i, ci => ci.opcode == OpCodes.Stfld);
                    var finalIndex = codes.FindIndex(first + 1, ci => ci.opcode == OpCodes.Stfld);
                    if (finalIndex != -1)
                    {
                        codes.RemoveRange(i, finalIndex - i + 1);
                        
                        // Then start adding our code
                        // We call helpers to GREATLY simplify the IL
                        // First get packages, then dupes
                        codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0){labels = new List<Label> {label}});
                        codes.Insert(i++, new CodeInstruction(OpCodes.Call, PackageCount));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Stfld, TargetPackageOptions));
                        
                        codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
                        codes.Insert(i++, new CodeInstruction(OpCodes.Call, DupeCount));
                        codes.Insert(i, new CodeInstruction(OpCodes.Stfld, TargetDupeOptions));
                    }
                    else
                    {
                        Debug.LogError("[ConfigurablePrintingPod] Could not patch InitializeContainers: Index");
                    }
                    
                    return codes;
                }
            }
            
            Debug.LogError("[ConfigurablePrintingPod] Something went really wrong!");
            return codes;
        }

        private static int GetRandomPackageCount()
        {
            var conf = ConfigurablePrintingPod.Config;
            var min =  (conf.NumberCarePackages - conf.CarePackageRange).Clamp(0, int.MaxValue);
            var max = (conf.NumberCarePackages + conf.CarePackageRange).Clamp(0, int.MaxValue);
            var rnd = UnityEngine.Random.Range(min, max);
            Debug.Log($"Random care packages: {rnd} from {min} - {max}");
            return rnd;
        }

        private static int GetRandomDuplicantCount()
        {
            var conf = ConfigurablePrintingPod.Config;
            var min =  (conf.NumberDuplicants - conf.DuplicantsRange).Clamp(0, int.MaxValue);
            var max = (conf.NumberDuplicants + conf.DuplicantsRange).Clamp(0, int.MaxValue);
            var rnd = UnityEngine.Random.Range(min, max);
            Debug.Log($"Random dupes: {rnd} from {min} - {max}");
            return rnd;
        }
    }

    public static class MathExtensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            if (val.CompareTo(max) > 0) return max;
            return val;
        }
    }

    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
    public class PodConfig
    {
        public int NumberCarePackages = 1;
        public int CarePackageRange = 1;
        public int NumberDuplicants = 3;
        public int DuplicantsRange = 1;
    }
    
    public static class ConfigHelper
    {
        public static readonly string ConfigDir =
            Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;

        private static readonly string ConfigLocation = Path.Combine(ConfigDir, "config.json");
        
        public static PodConfig ReadConfig()
        {
            if (!File.Exists(ConfigLocation))
            {
                var conf = new PodConfig();
                WriteConfig(conf);
                return conf;
            }

            return JsonConvert.DeserializeObject<PodConfig>(File.ReadAllText(ConfigLocation));
        }

        private static void WriteConfig(PodConfig conf) =>
            File.WriteAllText(ConfigLocation, JsonConvert.SerializeObject(conf));
    }
}
