namespace AlbionStatusBot.Job
{
    using Ninject;
    using Ninject.Syntax;
    using Quartz;
    using Quartz.Spi;

    public class NinjectJobFactory : IJobFactory
    {
        private readonly IResolutionRoot _resolutionRoot;

        public NinjectJobFactory(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _resolutionRoot.Get<IJob>();
        }

        public void ReturnJob(IJob job)
        {
            _resolutionRoot.Release(job);
        }
    }
}