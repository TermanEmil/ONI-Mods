using Klei.AI;

namespace BrothgarBroth.Entities
{
    public class BrothgarBroth : Workable, IBrothWorkable
    {
        private BrothgarBrothSm.Instance _smi;

        public BrothgarBroth()
        {
            SetReportType(ReportManager.ReportType.PersonalTime);
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            _smi = new BrothgarBrothSm.Instance(this);
            _smi.StartSM();
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            overrideAnims = new[] {Assets.GetAnim("anim_interacts_juicer_kanim")};
            synchronizeAnims = false;
            SetWorkTime(5f);
        }

        protected override void OnStartWork(Worker worker)
        {
            var controller = GetComponent<KBatchedAnimController>();
            if(controller == null)
            {
                Debug.LogWarning("[BrothgarBroth] How in the world did you get a broth without an anim controller?!");
                return;
            }

            controller.animScale = 0f;
        }

        protected override void OnCompleteWork(Worker worker)
        {
            var effects = worker.GetComponent<Effects>();
            effects.Add(BrothEffects.BrothCooldownEffect, true);
            effects.Add(BrothEffects.BrothSpeedEffect, true);
            effects.Add(BrothEffects.BrothStaminaEffect, true);
        }

        public bool CanConsumeBroth(Worker worker)
        {
            var effects = worker.GetComponent<Effects>();
            return effects != null && !effects.HasEffect(BrothEffects.BrothCooldownEffect);
        }

        protected virtual WorkChore<BrothgarBroth> CreateWorkChore()
        {
            var chore = new WorkChore<BrothgarBroth>(Db.Get().ChoreTypes.Relax, this, null, false, null, null, null, false);
            chore.AddPrecondition(CustomPreconditions.CanDrinkBrothPrecondition, this);
            return chore;
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
