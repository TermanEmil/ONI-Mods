using System;
using KSerialization;
using UnityEngine;

namespace OldCritterWrangler
{
    internal class AgeCritterWrangler : KMonoBehaviour, IThresholdSwitch, ICheckboxControl, ISim4000ms
    {
        private int _crittersAboveAge;

        private Guid _roomStatusGuid;

        private KSelectable _selectable;

        [field: Serialize] public bool ShouldMurder { get; set; }

        public bool GetCheckboxValue() { return ShouldMurder; }

        public void SetCheckboxValue( bool value ) { ShouldMurder = value; }

        public string CheckboxTitleKey => "STRINGS.UI.SIDESCREENS.AGEDCRITTERWRANGLER.MURDERBOX.TITLE";
        public string CheckboxLabel    => "Kill Creatures";
        public string CheckboxTooltip  => "Kill all creatures above/below a certain age rather than capture.";

        public void Sim4000ms( float dt )
        {
            var roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject( gameObject );
            if ( roomOfGameObject != null )
            {
                _crittersAboveAge = 0;
                foreach ( var creature in roomOfGameObject.cavity.creatures )
                {
                    var ageDb = Db.Get().Amounts.Age.Lookup( creature.gameObject );
                    var percentage = (int) (ageDb.value / ageDb.GetMax() * 100);
                    var capturable = creature.gameObject.GetComponent<Capturable>();
                    var faction = creature.gameObject.GetComponent<FactionAlignment>();
                    var flag = ActivateAboveThreshold ? percentage >= (int) Threshold : percentage <= (int) Threshold;
                    if ( flag )
                    {
                        ++_crittersAboveAge;
                        if ( ShouldMurder )
                        {
                            if ( capturable != null )
                                capturable.MarkForCapture( false );

                            if ( faction == null )
                                continue;

                            if ( FactionManager.Instance.GetDisposition(
                                     FactionManager.FactionID.Duplicant,
                                     faction.Alignment
                                 ) !=
                                 FactionManager.Disposition.Assist )
                                faction.SetPlayerTargeted( true );
                        }
                        else
                        {
                            if ( capturable != null )
                                capturable.MarkForCapture( true );

                            if ( faction != null )
                                faction.gameObject.Trigger( 2127324410 );
                        }
                    }
                    else
                    {
                        if ( capturable != null )
                            capturable.MarkForCapture( false );

                        if ( faction != null )
                            faction.gameObject.Trigger( 2127324410 );
                    }
                }

                if ( !_selectable.HasStatusItem( Db.Get().BuildingStatusItems.NotInAnyRoom ) )
                    return;

                _selectable.RemoveStatusItem( _roomStatusGuid );
            }
            else
            {
                if ( !_selectable.HasStatusItem( Db.Get().BuildingStatusItems.NotInAnyRoom ) )
                    _roomStatusGuid = _selectable.AddStatusItem( Db.Get().BuildingStatusItems.NotInAnyRoom );
            }
        }

        public float GetRangeMinInputField() { return 0.0f; }

        public float GetRangeMaxInputField() { return 100.0f; }

        public LocString ThresholdValueUnits() { return ""; }

        public string Format( float value, bool units )
        {
            return GameUtil.GetFormattedInt( Mathf.RoundToInt( value ) );
        }

        public float ProcessedSliderValue( float input ) { return Mathf.RoundToInt( input ); }

        public float ProcessedInputValue( float input ) { return Mathf.RoundToInt( input ); }

        [field: Serialize] public float Threshold { get; set; }

        [field: Serialize] public bool ActivateAboveThreshold { get; set; }

        public float CurrentValue => _crittersAboveAge;

        public float     RangeMin => 0.0f;
        public float     RangeMax => 100.0f;
        public LocString Title    => "Aged Critter Wrangler";

        public LocString ThresholdValueName => new LocString( "", "STRINGS.UI.UNITSUFFIXES.CRITTERSOVERAGE" );

        public string AboveToolTip =>
            "Wrangle creatures if they are above {0}% of the way through their life";

        public string BelowToolTip =>
            "Wrangle creatures if they are below {0}% of the way through their life";

        public ThresholdScreenLayoutType LayoutType     => ThresholdScreenLayoutType.SliderBar;
        public int                       IncrementScale => 1;
        public NonLinearSlider.Range[]   GetRanges      => NonLinearSlider.GetDefaultRange( RangeMax );

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            _selectable = GetComponent<KSelectable>();
        }
    }
}
