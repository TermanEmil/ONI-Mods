using System;
using System.Collections.Generic;
using System.Linq;
using Klei.AI;
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

            foreach (var item in _storage.items)
            {
                Destroy(item.GetComponent<StateMachineController>());
                var modiferComponents = item.GetComponents<Modifiers>();
                foreach (var modifier in modiferComponents)
                    Destroy(modifier);
            }

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
                var modiferComponents = go.GetComponents<Modifiers>();
                foreach (var modifier in modiferComponents)
                    Destroy(modifier);
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

            operational.SetFlag(wireConnectedFlag, CircuitID != ushort.MaxValue);
            if (!operational.IsOperational)
            {
                operational.SetActive(false);
                return;
            }

            var initialShinebugAndEggsCount = _shinebugs.Count + _shinebugEggs.Count;
            SimulatePower(dt);
            SimulateShinebugEggs(dt);
            SimulateShinebugs(dt);
            UpdateStatusItem();
            var finalShinebugAndEggsCount = _shinebugs.Count + _shinebugEggs.Count;

            if (initialShinebugAndEggsCount != finalShinebugAndEggsCount)
                Debug.LogError($"Started with: {initialShinebugAndEggsCount}. Ended with {finalShinebugAndEggsCount}");
        }

        private void SimulatePower(float dt)
        {
            CurrentWattage = ShinebugPowerMath.ComputeShinebugWattage(_shinebugs.Sum(x => x.Lux));
            
            operational.SetActive(CurrentWattage > 0.0f);

            var toGenerate = CurrentWattage * dt;
            Game.Instance.accumulators.Accumulate(_accumulator, toGenerate);
            
            if (toGenerate > 0.0f)
                GenerateJoules(Mathf.Max(toGenerate, dt));
        }

        private void SimulateShinebugEggs(float dt)
        {
            foreach (var egg in _shinebugEggs.ToList())
            {
                if (!egg.Simulate(dt))
                    continue;

                var shinebug = HatchShinebugEgg(egg);
                
                _shinebugs.Add(shinebug);
                _shinebugEggs.Remove(egg);
                _storage.items.Remove(egg.EggItem);
            }
        }

        private ShinebugSimulator HatchShinebugEgg(ShinebugEggSimulator egg)
        {
            var shinebug = new ShinebugSimulator(
                name: egg.Name.Replace("Egg", ""),
                maxAge: egg.GrownLifeTime,
                lux: egg.LuxToGive);

            SpawnUtility.SpawnShinebugEggShell(transform.GetPosition());
            return shinebug;
        }

        private void SimulateShinebugs(float dt)
        {
            foreach (var shinebug in _shinebugs.ToList())
            {
                if (!shinebug.Simulate(dt))
                    continue;

                var egg = LayAnEgg(shinebug);

                _shinebugEggs.Add(egg);
                _storage.items.Add(egg.EggItem);
                _shinebugs.Remove(shinebug);
            }
        }

        private ShinebugEggSimulator LayAnEgg(ShinebugSimulator shinebug)
        {
            var eggName = shinebug.Name + "Egg";
            var eggStats = _shinebugEggValues[eggName];

            var eggGameObject = SpawnUtility.SpawnShinebugEgg(eggName, transform.position);
            var egg = new ShinebugEggSimulator(
                eggName,
                eggStats.TimeToHatch,
                eggStats.AdultLife,
                eggStats.AdultLux,
                eggGameObject);

            return egg;
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