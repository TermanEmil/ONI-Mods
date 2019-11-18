using UnityEngine;

namespace MoreCanisterFillersMod
{
    internal class AutoDropInv : KMonoBehaviour
    {
        public bool AutoDrop;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            Subscribe((int) GameHashes.RefreshUserMenu, OnRefresh);
            Subscribe((int) GameHashes.OnStorageChange, StorageChanged);
        }

        public void StorageChanged(object data)
        {
            if (!AutoDrop) return;
            var storage = GetComponent<Storage>();
            if ((data as GameObject)?.GetComponent<Pickupable>().TotalAmount >= storage.capacityKg - 0.01)
                storage.DropAll();
        }

        public void OnRefresh(object _)
        {
            var autoDroppingStr = AutoDrop ? "Disable Auto-Drop" : "Enable Auto-Drop";
            var autoDroppingTooltip = AutoDrop
                ? "Disable this building from dropping bottles when full"
                : "Enable this building to drop bottles when full";
            var buttonInfo = new KIconButtonMenu.ButtonInfo("action_building_disabled", autoDroppingStr,
                OnChangeAutoDrop,
                Action.NumActions, null, null, null, autoDroppingTooltip);
            Game.Instance.userMenu.AddButton(gameObject, buttonInfo);
        }

        private void OnChangeAutoDrop()
        {
            AutoDrop = !AutoDrop;
            if (AutoDrop) GetComponent<Storage>().DropAll();
        }
    }
}