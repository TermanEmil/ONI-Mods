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
        private readonly List<ShinebugEggSimulator> _shinebugEggs = new List<ShinebugEggSimulator>();

        private readonly Dictionary<string, ShinebugEggData> _shinebugEggValues =
            new Dictionary<string, ShinebugEggData>
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

        private readonly List<ShinebugSimulator> _shinebugs = new List<ShinebugSimulator>();

        private HandleVector<int>.Handle _accumulator = HandleVector<int>.InvalidHandle;
        private Guid _statusHandle;

        public float CurrentWattage;


        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int) GameHashes.ActiveChanged, OnActiveChanged);
            Subscribe((int) GameHashes.OnStorageChange, OnStorageChanged);
            // TODO: Meter controller
            _accumulator = Game.Instance.accumulators.Add("Element", this);
        }

        private void OnStorageChanged(object data)
        {
            var storage = gameObject.GetComponent<Storage>();
            var nameStr = ((GameObject) data)?.name;
            if (!_shinebugEggValues.ContainsKey(nameStr))
            {
                var go = storage.Drop((GameObject) data);
                go.transform.SetPosition(transform.GetPosition() + new Vector3(-4f, 1f, 0));
            }
            else
            {
                AddEggFromName(nameStr);
            }

            storage?.items?.Remove((GameObject) data);
        }

        private void AddEggFromName(string nameStr)
        {
            var values = _shinebugEggValues[nameStr];
            _shinebugEggs.Add(new ShinebugEggSimulator(nameStr, values.TimeToHatch, values.AdultLife,
                values.AdultLux));
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
                _shinebugs.Add(new ShinebugSimulator(shinebugEgg.Name, 0, shinebugEgg.GrownLifeTime,
                    shinebugEgg.LuxToGive));
                _shinebugEggs.Remove(shinebugEgg);
            }

            CurrentWattage = 0.0f;
            //Debug.Log($"Reactor has {Shinebugs.Count} shinebugs");
            for (var i = 0; i < _shinebugs.Count; i++)
            {
                var shinebug = _shinebugs[i];
                CurrentWattage += shinebug.Lux * 0.00159f;
                if (shinebug.Simulate(dt)) continue;

                _shinebugs.Remove(shinebug);
                // When a shinebug dies, we should add an egg of the same kind
                AddEggFromName(shinebug.Name);
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