namespace ASB.Job
{
    using Quartz;
    using Quartz.Impl;

    public class Scheduler
    {
        private readonly ClassicJobFactory _jobFactory;

        public Scheduler(ClassicJobFactory jobFactory) => _jobFactory = jobFactory;

        public async void Run() // TODO refactoring needed
        {
            var factory = new StdSchedulerFactory();

            var scheduler = await factory.GetScheduler();

            scheduler.JobFactory = _jobFactory;

            var updateJob = JobBuilder.Create<UpdateStatusJob>()
                .WithIdentity("update-job", "albion")
                .Build();
            var cleanUpJob = JobBuilder.Create<CleanUpJob>()
                .WithIdentity("clean-up-job", "albion")
                .Build();

            var triggerForUpdate = TriggerBuilder.Create()
                .WithIdentity("update-trigger", "albion")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever())
                .StartNow()
                .Build();
            var triggerForCleanUp = TriggerBuilder.Create()
                .WithIdentity("clean-up-trigger", "albion")
                .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever())
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(updateJob, triggerForUpdate);
            await scheduler.ScheduleJob(cleanUpJob, triggerForCleanUp);
            await scheduler.Start();
        }
    }
}