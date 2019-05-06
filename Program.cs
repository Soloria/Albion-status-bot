namespace AlbionStatusBot
{
    using System.Threading;
    using Bot;
    using DotNetEnv;
    using Job;
    using Ninject;

    public static class Program
    {
        public static void Main()
        {
            Env.Load();

            var kernel = new StandardKernel(new AlbionBotModule());

            var bot = kernel.Get<TelegramBot>();
            var scheduler = kernel.Get<Scheduler>();

            bot.Run();
            scheduler.Run();

            Thread.CurrentThread.Join();
        }
    }
}