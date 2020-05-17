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

    [SerializationConfig(MemberSerialization.OptIn)]
    public class ShinebugReactor : Generator
    {
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
            var item = data as GameObject;
            if (item is null)
                return;

            var stats = ShinebugStats.Get(item.name);
            if (stats is null)
            {
                var dropped = _storage.Drop(item);
                dropped.transform.SetPosition(transform.GetPosition() + new Vector3(-4f, 1f, 0));
            }
            else
            {
                if (_storage.items.Contains(item))
                {
                    // The item was added
                    item.SetActive(false);

                    _storage.items.Add(item);
                    _shinebugEggs.Add(new ShinebugEggSimulator { Name = item.name, Incubation = stats.TimeToHatch });
                }
                else
                {
                    // The item was removed
                    item.SetActive(true);
                }
            }
        }

        private void OnDeconstruct(object data)
        {
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

        public void HatchAll()
        {
            foreach (var egg in _shinebugEggs.ToList())
                HatchShinebugEgg(egg);
        }

        private void SimulatePower(float dt)
        {
            CurrentWattage = ShinebugPowerMath.ComputeShinebugWattage(_shinebugs.Sum(x => ShinebugStats.Get(x.Name).AdultLux));
            
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
                egg.Simulate(dt);
                if (egg.TimeToHatch)
                    HatchShinebugEgg(egg);
            }
        }

        private ShinebugSimulator HatchShinebugEgg(ShinebugEggSimulator egg)
        {
            var stats = ShinebugStats.Get(egg.Name);
            var shinebug = new ShinebugSimulator
            {
                Name = egg.Name,
                MaxAge = stats.AdultLife
            };

            SpawnUtility.SpawnShinebugEggShell(transform.GetPosition());

            _shinebugs.Add(shinebug);
            _shinebugEggs.Remove(egg);
            _storage.items.Remove(_storage.items.First(x => x.PrefabID().Name == egg.Name));
            return shinebug;
        }

        private void SimulateShinebugs(float dt)
        {
            foreach (var shinebug in _shinebugs.ToList())
            {
                shinebug.Simulate(dt);
                if (shinebug.IsDead)
                    LayAnEggAndDie(shinebug);
            }
        }

        private ShinebugEggSimulator LayAnEggAndDie(ShinebugSimulator shinebug)
        {
            var stats = ShinebugStats.Get(shinebug.Name);
            var egg = new ShinebugEggSimulator
            {
                Name = shinebug.Name,
                Incubation = stats.TimeToHatch
            };
            _shinebugEggs.Add(egg);
            _shinebugs.Remove(shinebug);

            var eggGameObject = SpawnUtility.SpawnShinebugEgg(egg.Name, transform.position);
            _storage.items.Add(eggGameObject);
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