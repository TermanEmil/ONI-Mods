using System;
using System.Collections.Generic;
using Database;
using Harmony;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JackOfAllTrades
{
    [HarmonyPatch(typeof(MinionStartingStats), "GenerateAttributes")]
    internal class DuplicantInterestPatches
    {
        public static void Postfix(MinionStartingStats __instance)
        {
            var pointsLeft = 9;
            var aptitudesLeft = __instance.skillAptitudes.Keys.Count;
            foreach (var keyValuePair in __instance.skillAptitudes)
            {
                --aptitudesLeft;
                int increase;
                do
                {
                    increase = Random.Range(1, pointsLeft);
                } while (pointsLeft - increase < aptitudesLeft);

                pointsLeft -= increase;
                foreach (var l in __instance.StartingLevels)
                {
                    Console.WriteLine("StartingLevels: " + l.Key + " " + l.Value);
                }
                foreach (var attribute in keyValuePair.Key.relevantAttributes)
                {
                    Console.WriteLine(attribute);
                    Console.WriteLine(attribute.Id);
                    Console.WriteLine(attribute.Description);
                    __instance.StartingLevels[attribute.Id] = __instance.StartingLevels[attribute.Id] + 100;
                }
            }
        }
    }

    [HarmonyPatch(typeof(CharacterContainer), "SetInfoText")]
    public class AttributeTextPatch
    {
        public static void Postfix(CharacterContainer __instance, Transform ___attributeLabelAptitude)
        {
            Console.WriteLine(__instance.attributeLabelAptitude.gameObject);
        }
    }
}