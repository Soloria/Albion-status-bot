namespace ASB
{
    using API;
    using Bot;
    using DotNetEnv;
    using Job;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using NLog.Extensions.Logging;
    using Quartz;
    using Storage;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Bot.Commands;
    using LogLevel = Microsoft.Extensions.Logging.LogLevel;

    internal static class Program
    {
        public static async Task Main() => await new HostBuilder()
            .ConfigureHostConfiguration(x =>
            {
                Env.Load();
                x.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"bot_token", Env.GetString("BOT_TOKEN")},
                    {"notification_channel", Env.GetString("NOTIFICATION_CHANNEL")}
                });
            })
            .ConfigureServices(services =>
            {
                services.AddLogging(x =>
                {
                    x.ClearProviders();
                    // TODO, move set log level to config
                    x.SetMinimumLevel(LogLevel.Trace);
                    x.AddNLog();
                });

                services.AddScoped<AlbionApiClient>();
                
                services.AddSingleton<ClassicJobFactory>();
                services.AddSingleton<Scheduler>();
                services.AddSingleton<TelegramBot>();
                services.AddSingleton<CommandFactory>();

                services.AddTransient<IJob, UpdateStatusJob>();
                services.AddTransient<LocalContext>();

                services.AddHostedService<WarmUpService>();
                services.AddDistributedMemoryCache();
            })
            .Build()
            .RunAsync();
    }
}