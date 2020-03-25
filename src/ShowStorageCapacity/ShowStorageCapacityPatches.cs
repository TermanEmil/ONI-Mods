using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Harmony;
using UnityEngine;

namespace ShowStorageCapacity
{
    public class ShowStorageCapacity
    {
        public static void OnLoad()
        {
            Debug.Log(
                $"[ShowStorageCapacity] Loading mod version " +
                $"{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}"
            );
        }
    }

    [HarmonyPatch(typeof(SimpleInfoScreen), "RefreshStorage")]
    public class SimpleInfoScreen_RefreshStorage_Patches
    {
        public static void Postfix(GameObject ___storagePanel, GameObject ___selectedTarget)
        {
            if (___selectedTarget != null)
            {
                Storage[] storages = ___selectedTarget.GetComponentsInChildren<Storage>();
                var panel = ___storagePanel.GetComponent<CollapsibleDetailContentPanel>();
                if (panel != null && storages.Length > 0)
                {
                    var storedMass = storages.Sum(storage => storage.MassStored());
                    var totalMass = storages.Sum(storage => storage.Capacity());
                    panel.HeaderLabel.text += ": " + string.Format(STRINGS.UI.STARMAP.STORAGESTATS.STORAGECAPACITY, storedMass, totalMass) + STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM;
                }
            }
        }
    }
}