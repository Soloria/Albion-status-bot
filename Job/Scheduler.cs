namespace AlbionStatusBot.Job
{
    using Quartz;
    using Quartz.Impl;

    public class Scheduler
    {
        private readonly NinjectJobFactory _jobFactory;

        public Scheduler(NinjectJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
        }

        public async void Run()
        {
            var factory = new StdSchedulerFactory();

            var scheduler = await factory.GetScheduler();

            scheduler.JobFactory = _jobFactory;

            var updateJob = JobBuilder.Create<UpdateStatusJob>()
                .WithIdentity("update-job", "albion")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("update-trigger", "albion")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever())
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(updateJob, trigger);
            await scheduler.Start();
        }
    }
}