namespace ASB.Job
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Quartz;
    using Storage;

    public class CleanUpJob : IJob
    {
        private readonly LocalContext _storage;
        private readonly ILogger<CleanUpJob> _log;
        private readonly object Guarder = new object();

        public CleanUpJob(LocalContext storage, ILogger<CleanUpJob> log)
        {
            _storage = storage;
            _log = log;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            lock (Guarder) { } // todo need improvement

            var oldEntity = await _storage.Status.Where(x => (DateTimeOffset.UtcNow - x.CreatedAt).TotalDays > 2).ToListAsync();

            if (!oldEntity.Any())
                return;

            _log.LogInformation($"Start clean up old entity in db...");

            _storage.RemoveRange(oldEntity);

            var result = await _storage.SaveChangesAsync();

            _log.LogInformation($"Clean up is success. cleared '{result}' entity...");
            
        }
    }
}