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

    internal class ShinebugReactor : Generator
    {
        private HandleVector<int>.Handle _accumulator = HandleVector<int>.InvalidHandle;
        private Guid _statusHandle;
        public List<ShinebugSimulator> Shinebugs = new List<ShinebugSimulator>();

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe(824508782, OnActiveChanged);
            Shinebugs.Add(new ShinebugSimulator(0, 10, 99999f));
            _accumulator = Game.Instance.accumulators.Add("Element", this);
            // Meter controller
        }

        public override void EnergySim200ms(float dt)
        {
            base.EnergySim200ms(dt);
            var circuitId = CircuitID;

            operational.SetFlag(wireConnectedFlag, circuitId != ushort.MaxValue);
            if (!operational.IsOperational)
                return;

            var result = 0.0;
            Debug.Log($"Reactor has {Shinebugs.Count} shinebugs");
            for (var i = 0; i < Shinebugs.Count; i++)
            {
                var shinebug = Shinebugs[i];
                if (!shinebug.Simulate(dt * 1000)) Shinebugs.Remove(shinebug);

                result += shinebug.Lux * 0.0015f;
            }

            Debug.Log($"Total j/s is {result}");

            var joulesPerS = (float) result;
            operational.SetActive(joulesPerS > 0.0);
            joulesPerS = Mathf.Clamp(joulesPerS, 0.0f, 1200.0f);
            Debug.Log($"Generation capped to {joulesPerS}");

            Game.Instance.accumulators.Accumulate(_accumulator, joulesPerS * dt);
            if (joulesPerS > 0.0)
            {
                GenerateJoules(Mathf.Max(joulesPerS * dt, 1f * dt));
                Debug.Log($"Generating {joulesPerS * dt}");
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