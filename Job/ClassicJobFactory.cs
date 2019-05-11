namespace ASB.Job
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Quartz.Spi;

    public class ClassicJobFactory : IJobFactory
    {
        /// <summary>
        /// Microsoft DI Service Container
        /// </summary>
        private readonly IServiceProvider _resolutionRoot;

        public ClassicJobFactory(IServiceProvider resolutionRoot) 
            => _resolutionRoot = resolutionRoot;

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler) 
            => _resolutionRoot.GetService<IJob>();

        /// <summary>
        /// Clearing jobs when possible
        /// </summary>
        public void ReturnJob(IJob job)
        {
            if (job is IDisposable di)
                di.Dispose();
        }
    }
}