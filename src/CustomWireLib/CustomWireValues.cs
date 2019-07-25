using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

namespace CustomWireLib
{
    class CustomWireValues
    {
        private static Dictionary<int, float> AdditionalWireValues = new Dictionary<int, float>();

        private static Dictionary<float, int> InvAdditionalWireValues = new Dictionary<float, int>();

        private static int newWireCount = (int) Wire.WattageRating.NumRatings;

        public static int GetAndUpdateWireCount ()
        {
            newWireCount = (int) Wire.WattageRating.NumRatings + AdditionalWireValues.Count + 1;
            return newWireCount;
        }

        public static int AddOrGetWireWattageIndex(float rating)
        {
            // New rating index is current count plus 1
            var i = GetAndUpdateWireCount();
            try
            {
                InvAdditionalWireValues.Add(rating, i);
                AdditionalWireValues.Add(i, rating);
                Console.WriteLine("Added new rating of " + rating + "W at index " + i);
                return i;
            }
            catch (ArgumentException)
            {
                // Get the index from the value and give that to the user
                i = InvAdditionalWireValues[rating];
                Console.WriteLine("A wire with wattage " + rating + " already exists with index " + i);
                return i;
            }
        }

        public static float GetWireRating(int index)
        {
            return AdditionalWireValues.TryGetValue(index, out var val) ? val : -1f;
        }

        public static void RegisterBuildings()
        {
            foreach (var w in CustomWireMaker.customWires)
            {
                // Register buildings and add to plan screen
                BuildingConfigManager.Instance.RegisterBuilding(w as IBuildingConfig);
                ModUtil.AddBuildingToPlanScreen("Power", w.id);
                // Register strings
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{w.id.ToUpperInvariant()}.NAME", w.rating + "W Wire");
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{w.id.ToUpperInvariant()}.DESC", "Electrical wire is used to connect generators, batteries, and buildings.");
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{w.id.ToUpperInvariant()}.EFFECT", "Connects buildings to " + UI.FormatAsLink("Power", "POWER") + " sources.\n\nCan be run through wall and floor tile.");
            }
        }
    }

    class CustomWireMaker
    {
        public static List<CustomWire> customWires = new List<CustomWire>();

        public static void CreateWireWithRating(int index)
        {
            CreateWireWithRating(CustomWireValues.GetWireRating(index));
        }

        public static void CreateWireWithRating(float rating)
        {
            var w = new CustomWire
            {
                rating = rating
            };
            customWires.Add(w);
        }

        public class CustomWire : BaseWireConfig
        {
            public float rating = 0;
            public string id = "Wire";
            string anim = "utilities_electric_kanim";
            float construction_time = 3f;
            float[] mass = new float[]
            {
                25f
            };
            float insulation = 0.05f;
            EffectorValues noise = NOISE_POLLUTION.NONE;

            public override BuildingDef CreateBuildingDef()
            {
                id = rating + id;
                mass[0] = Math.Max(rating / 50f, 25f);
                return base.CreateBuildingDef(id, anim, construction_time, mass, insulation, TUNING.BUILDINGS.DECOR.PENALTY.TIER0, noise);
            }

            public override void DoPostConfigureComplete(GameObject go)
            {
                base.DoPostConfigureComplete((Wire.WattageRating)CustomWireValues.AddOrGetWireWattageIndex(rating), go);
            }
        }
    }
}
