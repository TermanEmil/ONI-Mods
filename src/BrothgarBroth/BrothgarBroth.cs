using Klei.AI;
using UnityEngine;

namespace BrothgarBroth
{
    public class BrothgarBroth : Workable
    {
        private BrothgarBrothSm.Instance _smi;

        public BrothgarBroth()
        {
            SetReportType(ReportManager.ReportType.PersonalTime);
            Debug.Log("CONSTRUCTOR");
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            _smi = new BrothgarBrothSm.Instance(this);
            _smi.StartSM();
        }

        protected override void OnPrefabInit()
        {
            Debug.Log("Prefabinit");
            base.OnPrefabInit();
            overrideAnims = new[] {Assets.GetAnim("anim_interacts_juicer_kanim")};
            synchronizeAnims = false;
            SetWorkTime(5f);
        }

        protected override void OnCompleteWork(Worker worker)
        {
            var effects = worker.GetComponent<Effects>();
            effects.Add(BrothEffects.BrothCooldownEffect, true);
            effects.Add(BrothEffects.BrothSpeedEffect, true);
            effects.Add(BrothEffects.BrothStaminaEffect, true);
        }

        protected virtual WorkChore<BrothgarBroth> CreateWorkChore()
        {
            return new WorkChore<BrothgarBroth>(Db.Get().ChoreTypes.Relax, this, null, false, null, null, null, false);
        }

        public class BrothgarBrothSm : GameStateMachine<BrothgarBrothSm, BrothgarBrothSm.Instance, BrothgarBroth>
        {
            public State usable;
            public State unusable;

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = usable;
                usable.ToggleChore(smi => smi.master.CreateWorkChore(), unusable);
                unusable.Enter(smi => Util.KDestroyGameObject(smi.master.gameObject));
            }

            public new class Instance : GameInstance
            {
                public Instance(BrothgarBroth master) : base(master) { }
            }
        }
    }
}
