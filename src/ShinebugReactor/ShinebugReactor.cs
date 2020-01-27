using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using KSerialization;
using STRINGS;
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

            foreach (var shinebug in _shinebugs)
                shinebug.Name = shinebug.Name.Replace("Egg", "");

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
            var nameStr = go.name;
            if (!_shinebugEggValues.ContainsKey(nameStr))
            {
                var dropped = _storage.Drop(go);
                dropped.transform.SetPosition(transform.GetPosition() + new Vector3(-4f, 1f, 0));
            }
            else
            {
                Destroy(go.GetComponent<StateMachineController>());
                Destroy(go.GetComponent<Modifiers>());
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
                GameUtil.KInstantiate(Assets.GetPrefab(shinebug.Name.Replace("Egg", "")),
                    Grid.CellToPosCBC(Grid.PosToCell(transform.position), Grid.SceneLayer.Creatures),
                    Grid.SceneLayer.Creatures).SetActive(true);
        }

        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);
            var circuitId = CircuitID;
            operational.SetFlag(wireConnectedFlag, circuitId != ushort.MaxValue);
            if (!operational.IsOperational)
            {
                operational.SetActive(false);
                return;
            }

            var toRemove = new List<ShinebugEggSimulator>();
            foreach (var egg in _shinebugEggs.Where(egg => egg.Simulate(dt)))
            {
#if DEBUG
                Debug.Log($"Egg name {egg.Name}");
#endif
                _shinebugs.Add(new ShinebugSimulator(egg.Name.Replace("Egg", ""), 0, egg.GrownLifeTime, egg.LuxToGive));
                var eggShell = Util.KInstantiate(Assets.GetPrefab((Tag) "EggShell"), gameObject.transform.GetPosition());
                eggShell.GetComponent<PrimaryElement>().Mass = 0.1f;
                eggShell.SetActive(true);
                toRemove.Add(egg);
            }

            foreach (var r in toRemove)
            {
                _shinebugEggs.Remove(r);
                _storage.items.Remove(r.EggItem);
                Trigger((int) GameHashes.OnStorageChange, r);
            }

            CurrentWattage = 0.0f;
            var shinebugsRemove = new List<ShinebugSimulator>();
            foreach (var shinebug in _shinebugs)
            {
                CurrentWattage += shinebug.Lux * 0.00159f;

                // Skip if not time to die
                if (!shinebug.Simulate(dt)) continue;

#if DEBUG
                Debug.Log($"Shinebug name: {shinebug.Name}");
#endif
                var eggName = shinebug.Name + "Egg";
                var eggItem = GameUtil.KInstantiate(Assets.GetPrefab(eggName),
                    Grid.CellToPosCBC(Grid.PosToCell(transform.position), Grid.SceneLayer.Front),
                    Grid.SceneLayer.Front);
#if DEBUG
                Debug.Log($"Shinebug Egg: {eggItem}");
#endif
                var eggStats = _shinebugEggValues[eggName];
                _shinebugEggs.Add(new ShinebugEggSimulator(eggName, eggStats.TimeToHatch, eggStats.AdultLife,
                    eggStats.AdultLux, eggItem));
                _storage.Store(eggItem, true);
                shinebugsRemove.Add(shinebug);
            }

            foreach (var r in shinebugsRemove)
                _shinebugs.Remove(r);
#if DEBUG
            Debug.Log($"Total j/s is {CurrentWattage}");
#endif

            CurrentWattage = Mathf.Clamp(CurrentWattage, 0.0f, 1250.0f);

            operational.SetActive(CurrentWattage > 0.0f);
            Game.Instance.accumulators.Accumulate(_accumulator, CurrentWattage * dt);
            if (CurrentWattage > 0.0f)
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