namespace AlbionStatusBot.Storage
{
    using AlbionApi;
    using LiteDB;

    public class StatusStorage
    {
        private readonly LiteCollection<AlbionServerStatus> _statuses;

        public StatusStorage(LiteDatabase storage)
        {
            _statuses = storage.GetCollection<AlbionServerStatus>("statuses");
        }

        public bool UpdateStatus(AlbionServerStatus status)
        {
            _statuses.EnsureIndex(x => x.CreatedAt, true);

            var last = GetLast();

            if (last == null || last.CreatedAt != status.CreatedAt)
            {
                _statuses.Insert(status);

                return true;
            }

            return false;
        }

        public AlbionServerStatus GetLast()
        {
            return _statuses.FindOne(Query.All("CreatedAt", Query.Descending));
        }
    }
}