namespace ASB.Job
{
    using System.Threading;
    using System.Threading.Tasks;
    using Bot;
    using Microsoft.Extensions.Hosting;
    using Storage;
    /// <summary>
    /// Warp up service
    /// </summary>
    /// <remarks>
    /// it runs the appropriate instances (<see cref="TelegramBot"/>, <see cref="Scheduler"/>) at the start
    /// and calling Ensure create at <see cref="LocalContext"/>
    /// </remarks>
    public class WarmUpService : BackgroundService
    {
        private readonly TelegramBot _bot;
        private readonly Scheduler _scheduler;
        private readonly LocalContext _ctx;

        public WarmUpService(TelegramBot bot, Scheduler scheduler, LocalContext ctx)
        {
            _bot = bot;
            _scheduler = scheduler;
            _ctx = ctx;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // call to deploy tables into db (inluded _ef_tables)
            await _ctx.Database.EnsureCreatedAsync(stoppingToken);

            _bot.Run();
            _scheduler.Run();
        }
    }
}