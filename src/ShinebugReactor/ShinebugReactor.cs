#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using UnityEngine;

namespace ShinebugReactor
{
    internal class ReactorStatusItem
    {
        public static StatusItem ShinebugReactorWattageStatus;
    }

    public struct ShinebugEggData
    {
        public float TimeToHatch;
        public float AdultLife;
        public float AdultLux;

        public override string ToString()
        {
            return $"Egg has {TimeToHatch}s egg time and makes {AdultLux} Lux for {AdultLife}s";
        }
    }

    [SerializationConfig(MemberSerialization.OptIn)]
    public class ShinebugReactor : Generator
    {
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
#if DEBUG
                        TimeToHatch = 15f,
                        AdultLife = 45f,
#else
                        TimeToHatch = 3000f,
                        AdultLife = 15000f,
#endif
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

        private HandleVector<int>.Handle _accumulator = HandleVector<int>.InvalidHandle;

        [Serialize] private List<ShinebugEggSimulator> _shinebugEggs;
        [Serialize] private List<ShinebugSimulator> _shinebugs;
        private Guid _statusHandle;

        private Storage _storage;

        public float CurrentWattage;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            if (_shinebugs == null)
                _shinebugs = new List<ShinebugSimulator>();
            if (_shinebugEggs == null)
                _shinebugEggs = new List<ShinebugEggSimulator>();
            Subscribe((int) GameHashes.ActiveChanged, OnActiveChanged);
            Subscribe((int) GameHashes.OnStorageChange, OnStorageChanged);
            Subscribe((int) GameHashes.DeconstructComplete, OnDeconstruct);
            _storage = gameObject.GetComponent<Storage>();
            // TODO: Meter controller
            _accumulator = Game.Instance.accumulators.Add("Element", this);
#if DEBUG
            Debug.Log($"Eggs when loaded: {_shinebugEggs.Count}");
            foreach (var egg in _shinebugEggs) Debug.Log($"\t{egg}");
            Debug.Log($"Bugs when loaded: {_shinebugs.Count}");
            foreach (var shinebug in _shinebugs) Debug.Log($"\t{shinebug}");
#endif
        }

        private void OnActiveChanged(object data)
        {
            GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power,
                !((Operational) data).IsActive
                    ? Db.Get().BuildingStatusItems.GeneratorOffline
                    : Db.Get().BuildingStatusItems.Wattage, this);
        }

        private void OnStorageChanged(object data)
        {
            var go = data as GameObject;
            if (go == null) return;
            var nameStr = go.name ?? "invalid";
            if (!_shinebugEggValues.ContainsKey(nameStr))
            {
                var dropped = _storage.Drop(go);
                dropped.transform.SetPosition(transform.GetPosition() + new Vector3(-4f, 1f, 0));
            }
            else
            {
                DestroyImmediate(go.GetComponent<StateMachineController>());
                var values = _shinebugEggValues[nameStr];
                _shinebugEggs.Add(new ShinebugEggSimulator(nameStr, values.TimeToHatch, values.AdultLife,
                    values.AdultLux, go));
            }
        }

        private void OnDeconstruct(object data)
        {
            foreach (var item in _storage.items)
                if (_shinebugEggValues.Keys.Contains(item.name))
                {
                    var def = item.AddOrGetDef<IncubationMonitor.Def>();
                    def.spawnedCreature = new Tag(item.name.Replace("Egg", "Baby"));
                    // 100 divided by numbers seconds to hatch
                    def.baseIncubationRate = 100.0f / _shinebugEggValues[item.name].TimeToHatch;
                }

            foreach (var shinebug in _shinebugs)
            {
                GameUtil.KInstantiate(Assets.GetPrefab(shinebug.Name.Replace("Egg", "")),
                    Grid.CellToPosCBC(Grid.PosToCell(transform.position), Grid.SceneLayer.Creatures),
                    Grid.SceneLayer.Creatures).SetActive(true);
            }
        }

        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);
            var circuitId = CircuitID;

            operational.SetFlag(wireConnectedFlag, circuitId != ushort.MaxValue);
            if (!operational.IsOperational)
                return;

            for (var i = 0; i < _shinebugEggs.Count; ++i)
            {
                var egg = _shinebugEggs[i];
                if (!egg.Simulate(dt)) continue;

                if (egg.Item != null)
                {
                    _storage.items.Remove(egg.Item);
                    egg.TimeToHatch = _shinebugEggValues[egg.Name].TimeToHatch;
                    var newName = egg.Name.Replace("Egg", "");
                    _shinebugs.Add(new ShinebugSimulator(newName, 0, egg.GrownLifeTime,
                        egg.LuxToGive, egg, egg.Item));
                }
                else if (egg.Shinebug != null)
                {
                    _shinebugs.Add(egg.Shinebug);
                    var eggItem = GameUtil.KInstantiate(Assets.GetPrefab(egg.Name),
                        Grid.CellToPosCBC(Grid.PosToCell(transform.position), Grid.SceneLayer.Front),
                        Grid.SceneLayer.Front);
                    egg.Item = eggItem;
                    _storage.Store(eggItem);
                }
                else
                {
                    var eggItem = GameUtil.KInstantiate(Assets.GetPrefab(egg.Name),
                        Grid.CellToPosCBC(Grid.PosToCell(transform.position), Grid.SceneLayer.Front),
                        Grid.SceneLayer.Front);
                    egg.Item = eggItem;
                    _shinebugs.Add(new ShinebugSimulator(egg.Name, 0, egg.GrownLifeTime, egg.LuxToGive, egg, egg.Item));
                    _storage.Store(eggItem);
                }


#if DEBUG
                Debug.LogWarning($"[Shinebug Reactor] Didn't find shinebug data for egg {egg}, creating new entity.");
#endif
                _shinebugEggs.Remove(egg);
            }

            CurrentWattage = 0.0f;
            for (var i = 0; i < _shinebugs.Count; ++i)
            {
                var shinebug = _shinebugs[i];
                CurrentWattage += shinebug.Lux * 0.00159f;

                // Skip if not time to die
                if (!shinebug.Simulate(dt)) continue;

                if (shinebug.Egg != null)
                {
                    _shinebugEggs.Add(shinebug.Egg);
                    _storage.items.Add(shinebug.Egg.Item);
                }
                else if (shinebug.Item != null)
                {
                    _storage.items.Add(shinebug.Item);

                    var values = _shinebugEggValues[shinebug.Name];
                    shinebug.Age = 0;
                    var newName = shinebug.Name + "Egg";
                    _shinebugEggs.Add(new ShinebugEggSimulator(newName, values.TimeToHatch, values.AdultLife,
                        values.AdultLux, shinebug));
                }
                else
                {
                    var eggItem = GameUtil.KInstantiate(Assets.GetPrefab(shinebug.Name + "Egg"),
                        Grid.CellToPosCBC(Grid.PosToCell(transform.position), Grid.SceneLayer.Front),
                        Grid.SceneLayer.Front);
                    _storage.Store(eggItem);
#if DEBUG
                    Debug.Log($"Had to manually add egg for {shinebug}");
#endif
                }

                _shinebugs.Remove(shinebug);
            }

#if DEBUG
            Debug.Log($"Total j/s is {CurrentWattage}");
#endif

            CurrentWattage = Mathf.Clamp(CurrentWattage, 0.0f, 1250.0f);

            operational.SetActive(CurrentWattage > 0.0f);
            Game.Instance.accumulators.Accumulate(_accumulator, CurrentWattage * dt);

            if (CurrentWattage > 0.0)
            {
                var toGen = CurrentWattage * dt;
                GenerateJoules(Mathf.Max(toGen, dt));
#if DEBUG
                Debug.Log($"Generating {toGen} over {dt} seconds");
#endif
            }

            UpdateStatusItem();
        }


        private void UpdateStatusItem()
        {
            selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.Wattage);
            if (_statusHandle == Guid.Empty)
                _statusHandle = selectable.AddStatusItem(ReactorStatusItem.ShinebugReactorWattageStatus, this);
            else
                GetComponent<KSelectable>()
                    .ReplaceStatusItem(_statusHandle, ReactorStatusItem.ShinebugReactorWattageStatus, this);
        }
    }
}