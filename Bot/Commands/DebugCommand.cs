namespace ASB.Bot.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Storage;
    using Telegram.Bot.Types;
    /// <summary>
    /// get raw json from db
    /// </summary>
    public class DebugCommand : BotCommand
    {
        public DebugCommand() : base("/debug_status") { }

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

            await _core.SendDebugStatusMessage(status, metadata.Message.Chat.Id)
                .ContinueWith(x => x.Status, token); // skip error when exist)0
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