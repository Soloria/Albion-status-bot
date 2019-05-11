namespace ASB
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using API;
    using Bot;
    using DotNetEnv;
    using Job;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Quartz;
    using Storage;

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
                services.AddScoped<AlbionApiClient>();

                services.AddSingleton<ClassicJobFactory>();
                services.AddSingleton<Scheduler>();
                services.AddSingleton<TelegramBot>();

                services.AddTransient<IJob, UpdateStatusJob>();
                services.AddTransient<LocalContext>();

                services.AddHostedService<WarmUpService>();
                services.AddDistributedMemoryCache();
            })
            .Build()
            .RunAsync();
    }
}