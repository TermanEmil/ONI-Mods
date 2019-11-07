using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using STRINGS;
using TUNING;
using UnityEngine;
using BUILDINGS = TUNING.BUILDINGS;

namespace CustomWireLib
{
    public class CustomWireValues
    {
        private static readonly Dictionary<int, float> AdditionalWireValues = new Dictionary<int, float>();

        private static readonly Dictionary<float, int> InvAdditionalWireValues = new Dictionary<float, int>();

        private static int _newWireCount = (int) Wire.WattageRating.NumRatings;

        public static int GetAndUpdateWireCount()
        {
            _newWireCount = (int) Wire.WattageRating.NumRatings + AdditionalWireValues.Count + 1;
            return _newWireCount;
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

            CustomWireMaker.CustomWires.Clear();
        }
    }

    public class CustomWireMaker
    {
        //Should register wire types OnLoad or before buildings are processed.
        public static readonly List<CustomWire> CustomWires = new List<CustomWire>();

        public static CustomWire CreateWireWithRating(int index)
        {
            return CreateWireWithRating(CustomWireValues.GetWireRating(index));
        }

        public static CustomWire CreateWireWithRating(float rating)
        {
            var w = new CustomWire(rating);
            CustomWires.Add(w);
            return w;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
        [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
        public class CustomWire : BaseWireConfig
        {
            // To change the anim, set this property on the instance, don't manually set things
            public string Anim = "utilities_electric_insulated_kanim";

            public float ConstructionTime = 3f;
            public BuildingDef Def;
            public string Id;
            public float Insulation = 0.05f;
            public float[] Mass;
            public EffectorValues Noise = NOISE_POLLUTION.NONE;
            public float Rating;

            public CustomWire(float rating)
            {
                Rating = rating;
                Id = rating + "Wire";
                Mass = new[]
                {
                    Math.Max(rating / 50f, 25f)
                };
                // anim is empty here because this is too early to load animations.
                // The animation is set in CreateBuildingDef when the animations are loaded.
                Def = CreateBuildingDef(Id, "", ConstructionTime, Mass, Insulation, BUILDINGS.DECOR.PENALTY.TIER0,
                    Noise);
            }

            public override BuildingDef CreateBuildingDef()
            {
                Def.AnimFiles = new[]
                {
                    Assets.GetAnim(Anim)
                };
                return Def;
            }

            public override void DoPostConfigureComplete(GameObject go)
            {
                base.DoPostConfigureComplete((Wire.WattageRating) CustomWireValues.AddOrGetWireWattageIndex(Rating), go);
            }
        }
    }
}