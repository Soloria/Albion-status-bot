namespace ASB.Bot.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Storage;
    using Telegram.Bot.Types;

    public class StatusCommand : BotCommand
    {
        public StatusCommand() : base("/get_status", "/start" /* 'start' command required by telegram bot standard */) { }

        private LocalContext _storage;
        private TelegramBot _core;

        /// <summary>
        /// Execute command statament
        /// </summary>
        /// <param name="metadata">
        /// Telegram metadata update event
        /// </param>
        /// <param name="token">
        /// async\await token
        /// </param>
        /// @awaitable
        protected override async Task ExecuteImpAsync(Update metadata, CancellationToken token)
        {
            var status = await _storage.GetLast();
            await _core.SendServerStatusMessage(status, metadata.Message.Chat.Id);
        }

        /// <summary>
        /// Awake start before call <see cref="BotCommand.ExecuteImpAsync"/>
        /// </summary>
        protected override Task AwakeAsync(IServiceProvider provider, CancellationToken token)
        {
            _storage = provider.GetService<LocalContext>();
            _core = provider.GetService<TelegramBot>();
            return Task.CompletedTask;
        }
    }
}