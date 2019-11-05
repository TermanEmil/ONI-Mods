using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShinebugReactor
{
    internal class ReactorStatusItem
    {
        public static StatusItem ShinebugReactorWattageStatus;
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
        private HandleVector<int>.Handle _accumulator = HandleVector<int>.InvalidHandle;
        private Guid _statusHandle;
        private List<ShinebugEggSimulator> _shinebugEggs = new List<ShinebugEggSimulator>();
        private List<ShinebugSimulator> _shinebugs = new List<ShinebugSimulator>();

        public float CurrentWattage;

        private readonly Dictionary<string, ShinebugEggData> _shinebugEggValues = new Dictionary<string, ShinebugEggData>
        {
            {
                "LightBugBlackEgg",
                new ShinebugEggData
                {
                    TimeToHatch = 9000f,
                    AdultLife = 45000f,
                    AdultLux = 0f
                }
            },
            {
                "LightBugBlueEgg",
                new ShinebugEggData
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800f
                }
            },
            {
                "LightBugEgg",
                new ShinebugEggData
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800f
                }
            },
            {
                "LightBugCrystalEgg",
                new ShinebugEggData
                {
                    TimeToHatch = 9000f,
                    AdultLife = 45000f,
                    AdultLux = 1800f
                }
            },
            {
                "LightBugOrangeEgg",
                new ShinebugEggData
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800
                }
            },
            {
                "LightBugPinkEgg",
                new ShinebugEggData
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800f
                }
            },
            {
                "LightBugPurpleEgg",
                new ShinebugEggData
                {
                    TimeToHatch = 3000f,
                    AdultLife = 15000f,
                    AdultLux = 1800f
                }
            }
        };


        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int) GameHashes.ActiveChanged, OnActiveChanged);
            Subscribe((int) GameHashes.OnStorageChange, OnStorageChanged);
            _accumulator = Game.Instance.accumulators.Add("Element", this);
            // Meter controller
        }

        private void OnStorageChanged(object data)
        {
            var storage = gameObject.GetComponent<Storage>();
            if (!_shinebugEggValues.ContainsKey(((GameObject) data)?.name))
            {
                Debug.Log($"invalid egg {data}");
                var go = storage.Drop((GameObject) data);
                go.transform.SetPosition(transform.GetPosition() + new Vector3(-4f, 1f, 0));
            }
            else
            {
                var values = _shinebugEggValues[((GameObject) data)?.name];
                Debug.Log($"Egg data: {values}");
                _shinebugEggs.Add(new ShinebugEggSimulator(values.TimeToHatch, values.AdultLife, values.AdultLux));
                storage?.items?.Remove((GameObject) data);
            }
        }

        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);
            var circuitId = CircuitID;

            operational.SetFlag(wireConnectedFlag, circuitId != ushort.MaxValue);
            if (!operational.IsOperational)
                return;

            for (var i = 0; i < _shinebugEggs.Count; i++)
            {
                var shinebugEgg = _shinebugEggs[i];
                if (!shinebugEgg.Simulate(dt)) continue;
                _shinebugs.Add(new ShinebugSimulator(0, shinebugEgg.GrownLifeTime, shinebugEgg.LuxToGive));
                _shinebugEggs.Remove(shinebugEgg);
            }

            CurrentWattage = 0.0f;
            //Debug.Log($"Reactor has {Shinebugs.Count} shinebugs");
            for (var i = 0; i < _shinebugs.Count; i++)
            {
                var shinebug = _shinebugs[i];
                CurrentWattage += shinebug.Lux * 0.00159f;
                if (!shinebug.Simulate(dt)) _shinebugs.Remove(shinebug);
            }

            //Debug.Log($"Total j/s is {joulesPerS}");
            CurrentWattage = Mathf.Clamp(CurrentWattage, 0.0f, 1250.0f);

            operational.SetActive(CurrentWattage > 0.0);
            Game.Instance.accumulators.Accumulate(_accumulator, CurrentWattage * dt);

            if (CurrentWattage > 0.0)
            {
                var toGen = CurrentWattage * dt;
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