namespace AlbionStatusBot
{
    using System.Collections.Generic;
    using System.Net;
    using AlbionApi;
    using Bot;
    using DotNetEnv;
    using Job;
    using LiteDB;
    using Microsoft.Extensions.Configuration;
    using Ninject.Modules;
    using Quartz;

    public class AlbionBotModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConfiguration>().ToMethod(context =>
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"bot_token", Env.GetString("BOT_TOKEN")},
                        {"notification_channel", Env.GetString("NOTIFICATION_CHANNEL")}
                    }).Build()
            );

            Bind<LiteDatabase>().ToMethod(context => new LiteDatabase("albion.kukarek"));

            Bind<NinjectJobFactory>().ToSelf();
            Bind<IJob>().To<UpdateStatusJob>();
            Bind<Scheduler>().ToSelf();

            Bind<IWebProxy>().ToMethod(context => new WebProxy());
            Bind<AlbionApiClient>().ToSelf();
            Bind<TelegramBot>().ToSelf();
        }
    }
}