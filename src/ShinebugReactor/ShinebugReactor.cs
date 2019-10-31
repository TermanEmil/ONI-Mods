using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShinebugReactor
{
    internal class ReactorStatusItem
    {
        public static readonly StatusItem ShinebugReactorWattageStatus = Db.Get().BuildingStatusItems
            .Add(new StatusItem("ShinebugReactorWattage", "BUILDING", string.Empty, StatusItem.IconType.Info,
                NotificationType.Neutral, false, OverlayModes.Power.ID));
    }

    internal struct ShinebugEggData
    {
        public float TimeToHatch;
        public float AdultLife;
        public float AdultLux;

        public override string ToString()
        {
            return $"Egg has {TimeToHatch}s egg time and makes {AdultLux} Lux for {AdultLife}s";
        }
    }

    internal class ShinebugReactor : Generator
    {

        public Dictionary<string, ShinebugEggData> shinebugEggValues = new Dictionary<string, ShinebugEggData>{
            {"LightBugBlackEgg",
                new ShinebugEggData()
                {
                    TimeToHatch = 9000f,
                    AdultLife = 45000f,
                    AdultLux = 0f
                }
            },
            {"LightBugBlueEgg",
                new ShinebugEggData()
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800f
                }
            },
            {"LightBugEgg",
                new ShinebugEggData()
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800f
                }
            },
            {"LightBugCrystalEgg",
                new ShinebugEggData()
                {
                    TimeToHatch = 9000f,
                    AdultLife = 45000f,
                    AdultLux = 1800f
                }
            },
            {"LightBugOrangeEgg",
                new ShinebugEggData()
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800
                }
            },
            {"LightBugPinkEgg",
                new ShinebugEggData()
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800f
                }
            },
            {"LightBugPurpleEgg",
                new ShinebugEggData()
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800f
                }
            }
        };

        private HandleVector<int>.Handle _accumulator = HandleVector<int>.InvalidHandle;
        private Guid _statusHandle;
        public List<ShinebugSimulator> Shinebugs = new List<ShinebugSimulator>();
        public List<ShinebugEggSimulator> ShinebugEggs = new List<ShinebugEggSimulator>();

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int) GameHashes.ActiveChanged, OnActiveChanged);
            Subscribe((int)GameHashes.OnStorageChange, OnStorageChanged);
            _accumulator = Game.Instance.accumulators.Add("Element", this);
            // Meter controller
        }

        private void OnStorageChanged(object data)
        {
            var values = shinebugEggValues[((GameObject)data)?.name];
            Debug.Log($"Egg data: {values}");
            ShinebugEggs.Add(new ShinebugEggSimulator(values.TimeToHatch, values.AdultLife, values.AdultLux));
            var storage = gameObject.GetComponent<Storage>();
            storage?.items?.Remove((GameObject)data);
        }

        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);
            var circuitId = CircuitID;

            operational.SetFlag(wireConnectedFlag, circuitId != ushort.MaxValue);
            if (!operational.IsOperational)
                return;

            for (var i = 0; i < ShinebugEggs.Count; i++)
            {
                var shinebugEgg = ShinebugEggs[i];
                if (shinebugEgg.Simulate(dt))
                {
                    Shinebugs.Add(new ShinebugSimulator(0, shinebugEgg.GrownLifeTime, shinebugEgg.LuxToGive));
                    ShinebugEggs.Remove(shinebugEgg);
                }
            }

            var joulesPerS = 0.0f;
            //Debug.Log($"Reactor has {Shinebugs.Count} shinebugs");
            for (var i = 0; i < Shinebugs.Count; i++)
            {
                var shinebug = Shinebugs[i];
                joulesPerS += shinebug.Lux * 0.0015f;
                if (!shinebug.Simulate(dt)) Shinebugs.Remove(shinebug);
            }

            //Debug.Log($"Total j/s is {joulesPerS}");
            joulesPerS = Mathf.Clamp(joulesPerS, 0.0f, 1200.0f);

            operational.SetActive(joulesPerS > 0.0);
            Game.Instance.accumulators.Accumulate(_accumulator, joulesPerS * dt);

            if (joulesPerS > 0.0)
            {
                var toGen = joulesPerS * dt;
                GenerateJoules(Mathf.Max(toGen, dt));
                //Debug.Log($"Generating {toGen} over {dt} seconds");
            }

            UpdateStatusItem();
        }

        private void OnActiveChanged(object data)
        {
            GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power,
                !((Operational) data).IsActive
                    ? Db.Get().BuildingStatusItems.GeneratorOffline
                    : Db.Get().BuildingStatusItems.Wattage, this);
        }

        private void UpdateStatusItem()
        {
            selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.Wattage);
            if (_statusHandle == Guid.Empty)
            {
                _statusHandle = selectable.AddStatusItem(ReactorStatusItem.ShinebugReactorWattageStatus, this);
            }
            else
            {
                if (!(_statusHandle != Guid.Empty))
                    return;
                GetComponent<KSelectable>()
                    .ReplaceStatusItem(_statusHandle, ReactorStatusItem.ShinebugReactorWattageStatus, this);
            }
        }
    }
}