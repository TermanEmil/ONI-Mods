namespace InfiniteStorage
{
    public class ShowHideContentsButton : KMonoBehaviour
    {
        public bool showContents;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe( (int) GameHashes.RefreshUserMenu, OnRefresh );
            GetComponent<Storage>().showInUI = showContents;
        }

        public void OnRefresh( object _ )
        {
            // If we are currently showing, display hide strings
            var showContentsStr = showContents
                ? DEEP_STORAGE_STRINGS.UI.SHOWHIDE_CONTENTS.HIDE
                : DEEP_STORAGE_STRINGS.UI.SHOWHIDE_CONTENTS.SHOW;

            var showContentsTooltip = showContents
                ? DEEP_STORAGE_STRINGS.UI.SHOWHIDE_CONTENTS.HIDE_TOOLTIP
                : DEEP_STORAGE_STRINGS.UI.SHOWHIDE_CONTENTS.SHOW_TOOLTIP;

            var buttonInfo = new KIconButtonMenu.ButtonInfo(
                "action_building_disabled",
                showContentsStr,
                OnChangeShowContents,
                Action.NumActions,
                null,
                null,
                null,
                showContentsTooltip
            );

            Game.Instance.userMenu.AddButton( gameObject, buttonInfo );
        }

        private void OnChangeShowContents()
        {
            showContents = !showContents;
            GetComponent<Storage>().showInUI = showContents;
        }
    }
}
