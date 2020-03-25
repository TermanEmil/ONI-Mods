using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace MoreCanisterFillersMod
{
    internal class UnfilteredBottleEmptier : StateMachineComponent<UnfilteredBottleEmptier.StatesInstance>,
                                             IEffectDescriptor
    {
        public float EmptyRate = 10f;

        public List<Descriptor> GetDescriptors( BuildingDef def ) { return null; }

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi.StartSM();
        }

        public class
            StatesInstance : GameStateMachine<States, StatesInstance, UnfilteredBottleEmptier, object>.GameInstance
        {
            private FetchChore _chore;

            public StatesInstance( UnfilteredBottleEmptier smi ) : base( smi )
            {
                Meter = new MeterController(
                    GetComponent<KBatchedAnimController>(),
                    "meter_target",
                    nameof( Meter ),
                    global::Meter.Offset.Infront,
                    Grid.SceneLayer.NoLayer,
                    "meter_target",
                    "meter_arrow",
                    "meter_scale"
                );

                Subscribe( (int) GameHashes.OnStorageChange, OnStorageChange );
            }

            public MeterController Meter { get; }

            private void OnStorageChange( object data )
            {
                var component = GetComponent<Storage>();
                Meter.SetPositionPercent( Mathf.Clamp01( component.RemainingCapacity() / component.capacityKg ) );
            }

            private PrimaryElement GetFirstPrimaryElement()
            {
                var storage = GetComponent<Storage>();
                for ( var i = 0; i < storage.Count; ++i )
                {
                    var storageGameObject = storage[i];
                    if ( storageGameObject == null )
                        continue;

                    var element = storageGameObject.GetComponent<PrimaryElement>();
                    if ( element != null )
                        return element;
                }

                return null;
            }

            public void Emit( float dt )
            {
                var firstPrimaryElement = GetFirstPrimaryElement();
                if ( firstPrimaryElement == null )
                    return;

                var storage = GetComponent<Storage>();
                var amountToConsume = Mathf.Min( firstPrimaryElement.Mass, master.EmptyRate * dt );
                if ( amountToConsume <= 0.0 )
                    return;

                var consumedTag = firstPrimaryElement.GetComponent<KPrefabID>().PrefabTag;
                storage.ConsumeAndGetDisease(
                    consumedTag,
                    amountToConsume,
                    out var diseaseInfo,
                    out var aggregateTemperature
                );

                var position = transform.GetPosition();
                position.y += 1.8f;
                var flippedH = GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH;
                position.x += flippedH ? -0.2f : 0.2f;
                var cell = Grid.PosToCell( position ) + (flippedH ? -1 : 1);
                if ( Grid.Solid[cell] )
                    cell += flippedH ? 1 : -1;

                var element = firstPrimaryElement.Element;
                var idx = element.idx;
                if ( element.IsLiquid )
                    FallingWater.instance.AddParticle(
                        cell,
                        idx,
                        amountToConsume,
                        aggregateTemperature,
                        diseaseInfo.idx,
                        diseaseInfo.count,
                        true
                    );
                else
                    SimMessages.ModifyCell(
                        cell,
                        idx,
                        aggregateTemperature,
                        amountToConsume,
                        diseaseInfo.idx,
                        diseaseInfo.count
                    );
            }
        }

        public class States : GameStateMachine<States, StatesInstance, UnfilteredBottleEmptier>
        {
            private StatusItem _statusItem;
            public  State      Emptying;
            public  State      Unoperational;
            public  State      Waitingfordelivery;

            public override void InitializeStates( out BaseState outDefaultState )
            {
                outDefaultState = Waitingfordelivery;
                _statusItem = new StatusItem(
                                  nameof( UnfilteredBottleEmptier ),
                                  string.Empty,
                                  string.Empty,
                                  string.Empty,
                                  StatusItem.IconType.Info,
                                  NotificationType.Neutral,
                                  false,
                                  OverlayModes.None.ID
                              )
                              {
                                  resolveStringCallback = ( str, data ) =>
                                  {
                                      var bottleEmptier = (UnfilteredBottleEmptier) data;
                                      if ( bottleEmptier == null )
                                          return str;

                                      return (string) BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME;
                                  },
                                  resolveTooltipCallback = ( str, data ) =>
                                  {
                                      var bottleEmptier = (UnfilteredBottleEmptier) data;
                                      if ( bottleEmptier == null )
                                          return str;

                                      return (string) BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP;
                                  }
                              };

                root.ToggleStatusItem( _statusItem, smi => (object) smi.master );
                Unoperational.TagTransition( GameTags.Operational, Waitingfordelivery ).PlayAnim( "off" );
                Waitingfordelivery.TagTransition( GameTags.Operational, Unoperational, true ).EventTransition(
                    GameHashes.OnStorageChange,
                    Emptying,
                    smi => !smi.GetComponent<Storage>().IsEmpty()
                ).PlayAnim( "on" );

                Emptying.TagTransition( GameTags.Operational, Unoperational, true ).EventTransition(
                    GameHashes.OnStorageChange,
                    Waitingfordelivery,
                    smi => smi.GetComponent<Storage>().IsEmpty()
                ).Update( "Emit", ( smi, dt ) => smi.Emit( dt ) ).PlayAnim(
                    "working_loop",
                    KAnim.PlayMode.Loop
                );
            }
        }
    }
}
