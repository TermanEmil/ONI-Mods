using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using CaiLib.Utils;
using Harmony;
using UnityEngine;

namespace EdibleDuplicants
{
    public class EdibleDuplicantsPatches
    {
        [HarmonyPatch(typeof(WorldInventory), "OnAddedFetchable")]
        public class abcd
        {
            public static void Prefix(object data)
            {
                var go = (GameObject) data;
                Debug.Log($"{go} health: {go.GetComponent<Health>()}");
            }
        }

        [HarmonyPatch(typeof(Assets), nameof(Assets.AddPrefab))]
        public class Assets_AddPrefab_Patch
        {
            public static void Postfix(KPrefabID prefab)
            {
                //Debug.Log($"Adding prefab {prefab.name}");
            }
        }

        [HarmonyPatch(typeof(DeathMonitor.Instance), nameof(DeathMonitor.Instance.ApplyDeath))]
        public class DeathMonitor_Instance_ApplyDeath_Patch
        {
            public static void Postfix(DeathMonitor.Instance __instance)
            {
                Debug.Log("AAAAAAAAAAAA");
                Object.DestroyImmediate(__instance.GetComponent<OxygenBreather>());
                Object.DestroyImmediate(__instance.GetComponent<Health>());
                Debug.Log($"Health: {__instance.GetComponent<Health>()}");
                Debug.Log(__instance.gameObject);
                __instance.Trigger((int) GameHashes.AddedFetchable, __instance.gameObject);
                //__instance.GetComponent<Health>().DestroyRequiredComponentsAndSelf();
            }
        }

        public static void OnLoad()
        {
            CaiLib.Logger.Logger.LogInit();

            BuildingUtils.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Food, ButcherTableConfig.Id);
            Strings.Add("STRINGS.BUILDINGS.PREFABS.BUTCHERTABLE.NAME", ButcherTableConfig.DisplayName);
            Strings.Add("STRINGS.BUILDINGS.PREFABS.BUTCHERTABLE.EFFECT", "EFFECT");
            Strings.Add("STRINGS.BUILDINGS.PREFABS.BUTCHERTABLE.DESC", ButcherTableConfig.Description);
        }
    }
}