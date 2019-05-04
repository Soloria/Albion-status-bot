namespace AlbionStatusBot
{
    using System.Collections.Generic;
    using System.Net;
    using AlbionApi;
    using Bot;
    using DotNetEnv;
    using Microsoft.Extensions.Configuration;
    using Ninject;
    using Ninject.Modules;

    public class AlbionBotModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConfiguration>().ToMethod(context =>
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"bot_token", Env.GetString("BOT_TOKEN")}
                    }).Build()
            );

            Bind<IWebProxy>().ToMethod(context => new WebProxy());
            Bind<AlbionApiClient>().ToSelf();
            Bind<TelegramBot>().ToSelf();
        }
    }
}