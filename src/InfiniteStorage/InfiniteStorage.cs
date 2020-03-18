namespace InfiniteStorage
{
    public class InfiniteStorage : KMonoBehaviour
    {
        public FilteredStorage _filteredStorage;

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            _filteredStorage = new FilteredStorage( this, null, null, null, false, Db.Get().ChoreTypes.StorageFetch );
        }
    }
}
