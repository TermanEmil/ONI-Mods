namespace ShinebugReactor
{
	public class HatchAllEggsButton : KMonoBehaviour
    {
        private static bool ButtonIsVisible => Game.Instance.SandboxModeActive || DebugHandler.InstantBuildMode;

		protected override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int) GameHashes.RefreshUserMenu, OnRefresh);
        }

		public void OnRefresh(object _)
        {
			if (!ButtonIsVisible)
				return;

			var buttonInfo = new KIconButtonMenu.ButtonInfo(
				text: "Hatch all eggs",
				on_click: () => GetComponent<ShinebugReactor>().HatchAll());

			Game.Instance.userMenu?.AddButton(gameObject, buttonInfo);
		}
	}
}