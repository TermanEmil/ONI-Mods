using System;
using System.Collections.Generic;
using Harmony;

namespace PetPips
{
    internal class PetPipPatches
    {
        public class PetStates : GameStateMachine<PetStates, PetStates.Instance, IStateMachineTarget, PetStates.Def>
        {
            public State Idle;
            public State Follow;
            public State Homeless;

            public override void InitializeStates(out StateMachine.BaseState default_state)
            {
                default_state = Homeless;

            }

            public new class Instance : GameInstance
            {
                public Instance(IStateMachineTarget master) : base(master)
                {
                }
            }

            public class Def : BaseDef
            {
            }
        }

        [HarmonyPatch(typeof(ChoreTable.Builder), nameof(ChoreTable.Builder.CreateTable))]
        class test
        {
            private static void Postfix(ChoreTable.Builder __instance, ChoreTable __result)
            {
                Debug.Log($"Result is {__result}");
                var trav = Traverse.Create(__result);

                // pass in a StateMachine.BaseDef def, int priority, int interrupt_priority 

                var entries = (ChoreTable.Entry[]) trav.Field("entries").GetValue();
                Debug.Log($"Before size is {entries.Length}");
                Array.Resize(ref entries, entries.Length + 10);
                Debug.Log($"After size is {entries.Length}");
            }
        }
    }
}