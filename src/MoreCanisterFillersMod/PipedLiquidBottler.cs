namespace MoreCanisterFillersMod
{
    public class PipedLiquidBottler : Workable
    {
        public Controller.Instance Smi;
        public Storage             Storage;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Smi = new Controller.Instance( this );
            Smi.StartSM();
        }

        protected override void OnCleanUp()
        {
            Smi?.StopSM( nameof( OnCleanUp ) );
            base.OnCleanUp();
        }

        public class Controller : GameStateMachine<Controller, Controller.Instance, PipedLiquidBottler>
        {
            public State Empty;
            public State Filling;
            public State Pickup;
            public State Ready;

            public override void InitializeStates( out BaseState outDefaultState )
            {
                outDefaultState = Empty;
                Empty.PlayAnim( "off" ).EventTransition(
                    GameHashes.OnStorageChange,
                    Filling,
                    smi => smi.master.Storage.IsFull()
                );

                Filling.PlayAnim( "working" ).OnAnimQueueComplete( Ready );

                Ready.EventTransition( GameHashes.OnStorageChange, Pickup, smi => !smi.master.Storage.IsFull() )
                     .Enter( EventEnterExit ).Exit( EventEnterExit );

                Pickup.PlayAnim( "pick_up" ).OnAnimQueueComplete( Empty );
            }

            private static void EventEnterExit( Instance smi )
            {
                smi.master.Storage.allowItemRemoval = true;
                foreach ( var go in smi.master.Storage.items )
                    go.Trigger( (int) GameHashes.OnStorageInteracted, smi.master.Storage );
            }

            public class Instance : GameInstance
            {
                public Instance( PipedLiquidBottler master ) : base( master ) { }
            }
        }
    }
}
