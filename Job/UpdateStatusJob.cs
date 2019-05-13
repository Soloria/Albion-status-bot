namespace ASB.Job
{
    using System.Threading.Tasks;
    using API;
    using Bot;
    using Quartz;
    using Storage;

    public class UpdateStatusJob : IJob
    {
        private readonly AlbionApiClient _albion;
        private readonly LocalContext _storage;
        private readonly TelegramBot _telegram;

        public UpdateStatusJob(AlbionApiClient albion, LocalContext storage, TelegramBot telegram)
        {
            _albion = albion;
            _storage = storage;
            _telegram = telegram;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var status = await _albion.GetServerStatus();
            var isUpdated = await _storage.UpdateStatus(status);

            if (isUpdated)
                await _telegram.SendServerStatusMessage(status);
        }
    }
}