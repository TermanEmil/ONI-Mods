using UnityEngine;

namespace MoreCanisterFillersMod
{
    public class PipedLiquidBottler : Workable
    {
        public bool AutoDropBottles;
        public Controller.Instance Smi;
        public Storage Storage;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int) GameHashes.RefreshUserMenu, OnRefreshUserMenu);
            Subscribe((int) GameHashes.OnStorageChange, StorageChanged);
            Smi = new Controller.Instance(this);
            Smi.StartSM();
        }

        protected override void OnCleanUp()
        {
            Smi?.StopSM(nameof(OnCleanUp));
            base.OnCleanUp();
        }

        public void StorageChanged(object data)
        {
            if (!AutoDropBottles) return;
            var storage = GetComponent<Storage>();
            if ((data as GameObject)?.GetComponent<Pickupable>().TotalAmount >= storage.capacityKg - 0.01)
                storage.DropAll();
        }

        public void OnRefreshUserMenu(object _)
        {
            var autoDroppingStr = AutoDropBottles ? "Disable Auto-Drop" : "Enable Auto-Drop";
            var autoDroppingTooltip = AutoDropBottles
                ? "Disable this building from dropping bottles when full"
                : "Enable this building to drop bottles when full";
            var buttonInfo = new KIconButtonMenu.ButtonInfo("action_building_disabled", autoDroppingStr,
                OnChangeAutoDropBottles,
                Action.NumActions, null, null, null, autoDroppingTooltip);
            Game.Instance.userMenu.AddButton(gameObject, buttonInfo);
        }

        private void OnChangeAutoDropBottles()
        {
            AutoDropBottles = !AutoDropBottles;
            if (AutoDropBottles) GetComponent<Storage>().DropAll();
            Smi.MakeUnoperational();
        }

        public class Controller : GameStateMachine<Controller, Controller.Instance, PipedLiquidBottler>
        {
            public State Empty;
            public State Filling;
            public State Pickup;
            public State Ready;

            public override void InitializeStates(out BaseState outDefaultState)
            {
                outDefaultState = Empty;
                Empty.PlayAnim("off")
                    .EventTransition(GameHashes.OnStorageChange, Filling,
                        smi => smi.master.Storage.IsFull());
                Filling.PlayAnim("working")
                    .OnAnimQueueComplete(Ready);

                Ready.EventTransition(GameHashes.OnStorageChange, Pickup, smi => !smi.master.Storage.IsFull())
                    .Enter(EventEnterExit)
                    .Exit(EventEnterExit);
                Pickup.PlayAnim("pick_up")
                    .OnAnimQueueComplete(Empty);
            }

            private static void EventEnterExit(Instance smi)
            {
                smi.master.Storage.allowItemRemoval = true;
                foreach (var go in smi.master.Storage.items) go.Trigger(-778359855, smi.master.Storage);
            }

            public class Instance : GameInstance
            {
                public Instance(PipedLiquidBottler master) : base(master)
                {
                }

                public void MakeUnoperational()
                {
                    GoTo(sm.defaultState);
                }
            }
        }
    }
}