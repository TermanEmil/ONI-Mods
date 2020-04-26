using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Harmony;
using UnityEngine;

namespace AssignableEverything
{
    internal class AssignableEverythingPatches
    {
        private static readonly Dictionary<string, AssignableSlot> Assignables = new Dictionary<string, AssignableSlot>();

        //TODO: Find all buildings

        [HarmonyPatch(typeof(MinionAssignablesProxy), nameof(MinionAssignablesProxy.ConfigureAssignableSlots))]
        public class MinionAssignablesProxy_ConfigureAssignableSlots_Patch
        {
            public static void Postfix(MinionAssignablesProxy __instance, bool ___slotsConfigured)
            {
                Debug.Log($"Adding assignables!!!! {___slotsConfigured}");
                if (___slotsConfigured) return;

                var ownables = __instance.GetComponent<Ownables>();

                foreach (var assignable in Assignables)
                {
                    Debug.Log($"Adding assignable ${assignable.Key}");
                    ownables.Add(new OwnableSlotInstance(ownables, (OwnableSlot) assignable.Value));

                    __instance.ownables = new List<Ownables>
                    {
                        __instance.gameObject.AddOrGet<Ownables>()
                    };
                }
            }
        }

        [HarmonyPatch(typeof(Assignable), "get_slot")]
        public class Assignable_get_slot_Patch
        {
            public static void Prefix(Assignable __instance, ref AssignableSlot ____slot)
            {
                Assignables.TryGetValue(__instance.slotID, out ____slot);
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            private static readonly HarmonyInstance HarmonyInst =
                HarmonyInstance.Create("asquared31415.AssignableEverything");

            public static void Prefix(List<Type> types)
            {
                var building = typeof(IBuildingConfig);
                var buildingTypeList = new List<Type>();

                foreach (var type in types)
                    if (building.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface &&
                        type != typeof(AirborneCreatureLureConfig))
                    {
                        buildingTypeList.Add(type);
                        var name = type.ToString();
                        Debug.Log($"Adding to list {name}");
                        Assignables.Add(name, new OwnableSlot(name, name));
                    }

                var post = typeof(GeneratedBuildings_LoadGeneratedBuildings_Patch).GetMethod("DynamicPost");
                foreach (var buildingType in buildingTypeList)
                {
                    var orig = buildingType.GetMethod("ConfigureBuildingTemplate");
                    HarmonyInst.Patch(orig, null, new HarmonyMethod(post));
                }
            }

            public static void DynamicPost(IBuildingConfig __instance, GameObject go)
            {
                if (go.GetComponent<Workable>() == null) return;
                Debug.Log($"Adding ownable to gameobject {go}");
                var ownable = go.AddOrGet<Ownable>();
                var name = __instance.GetType().ToString();
                Debug.Log($"\tname={name}, id={Assignables[name].Id}");
                ownable.slotID = Assignables[name].Id;
                ownable.canBePublic = true;
            }
        }

        /*
        [HarmonyPatch(typeof(ShowerConfig), nameof(ShowerConfig.ConfigureBuildingTemplate))]
        public class ShowerConfig_ConfigureBuildingTemplate_Patch
        {
            public static void Postfix(GameObject go)
            {
                var ownable = go.AddOrGet<Ownable>();
                ownable.slotID = Assignables[0].Id;
                ownable.canBePublic = true;
            }
        }*/
    }
}