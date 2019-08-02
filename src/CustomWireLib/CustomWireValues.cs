using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace CustomWireLib
{
    public class CustomWireValues
    {
        private static Dictionary<int, float> _additionalWireValues = new Dictionary<int, float>();

        private static Dictionary<float, int> _invAdditionalWireValues = new Dictionary<float, int>();

        private static int _newWireCount = (int) Wire.WattageRating.NumRatings;

        public static int GetAndUpdateWireCount ()
        {
            _newWireCount = (int) Wire.WattageRating.NumRatings + _additionalWireValues.Count + 1;
            return _newWireCount;
        }

        public static int AddOrGetWireWattageIndex(float rating)
        {
            // New rating index is current count plus 1
            var i = GetAndUpdateWireCount();
            try
            {
                _invAdditionalWireValues.Add(rating, i);
                _additionalWireValues.Add(i, rating);
                Console.WriteLine("Added new rating of " + rating + "W at index " + i);
                return i;
            }
            catch (ArgumentException)
            {
                // Get the index from the value and give that to the user
                i = _invAdditionalWireValues[rating];
                Console.WriteLine("A wire with wattage " + rating + " already exists with index " + i);
                return i;
            }
        }

        public static float GetWireRating(int index)
        {
            return _additionalWireValues.TryGetValue(index, out var val) ? val : -1f;
        }

        // Call this to register all buildings created using the CustomWireMaker class.
        public static List<CustomWireMaker.CustomWire> RegisterBuildings()
        {
            foreach (var w in CustomWireMaker.CustomWires)
            {
                // Register buildings and add to plan screen
                BuildingConfigManager.Instance.RegisterBuilding(w);
                ModUtil.AddBuildingToPlanScreen("Power", w.Id);
                // Register strings
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{w.Id.ToUpperInvariant()}.NAME", w.Rating + "W Wire");
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{w.Id.ToUpperInvariant()}.DESC",
                    "Electrical wire is used to connect generators, batteries, and buildings.");
                Strings.Add($"STRINGS.BUILDINGS.PREFABS.{w.Id.ToUpperInvariant()}.EFFECT",
                    "Connects buildings to " + UI.FormatAsLink("Power", "POWER") +
                    " sources.\n\nCan be run through wall and floor tile.");
            }
            var outList = CustomWireMaker.CustomWires;
            CustomWireMaker.CustomWires.Clear();
            return outList;
        }
    }

    public class CustomWireMaker
    {
        // Any calls here should be made **AFTER** (or in a postfix of) GeneratedBuildings.LoadGeneratedBuildings.
        // RegisterBuildings must be called to properly register all buildings.
        public static List<CustomWire> CustomWires = new List<CustomWire>();

        public static BuildingDef GetBuildingDef(int index)
        {
            var r = CustomWireValues.GetWireRating(index);
            if (r != -1f)
            {
                return GetBuildingDef(r);
            }
            Console.WriteLine("Error getting rating from index " + index);
            return null;
        }

        public static BuildingDef GetBuildingDef(float rating)
        {
            foreach (var w in CustomWires)
            {
                if (w.Rating != rating) continue;
                var traverse = (Dictionary<IBuildingConfig, BuildingDef>)Traverse.Create(BuildingConfigManager.Instance).Field("configTable").GetValue();
                return traverse[w];
            }
            Console.WriteLine("Error finding wire with rating " + rating);
            return null;
        }

        public static void CreateWireWithRating(int index)
        {
            CreateWireWithRating(CustomWireValues.GetWireRating(index));
        }

        public static void CreateWireWithRating(float rating)
        {
            var w = new CustomWire
            {
                Rating = rating
            };
            CustomWires.Add(w);
        }

        public class CustomWire : BaseWireConfig
        {
            public float Rating;
            public string Id = "Wire";
            readonly string _anim = "utilities_electric_kanim";
            float _constructionTime = 3f;
            float[] mass = {
                25f
            };

            readonly float _insulation = 0.05f;
            readonly EffectorValues _noise = NOISE_POLLUTION.NONE;

            public override BuildingDef CreateBuildingDef()
            {
                Id = Rating + Id;
                mass[0] = Math.Max(Rating / 50f, 25f);
                return base.CreateBuildingDef(Id, _anim, _constructionTime, mass, _insulation, BUILDINGS.DECOR.PENALTY.TIER0, _noise);
            }

            public override void DoPostConfigureComplete(GameObject go)
            {
                base.DoPostConfigureComplete((Wire.WattageRating)CustomWireValues.AddOrGetWireWattageIndex(Rating), go);
            }
        }
    }
}
