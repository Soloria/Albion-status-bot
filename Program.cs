namespace AlbionStatusBot
{
    using System.Threading;
    using Bot;
    using DotNetEnv;
    using Ninject;

    public static class Program
    {
        public static void Main()
        {
            Env.Load();

            var kernel = new StandardKernel(new AlbionBotModule());
            var bot = kernel.Get<TelegramBot>();

            bot.Run();
            
            Thread.CurrentThread.Join();
        }
    }
}