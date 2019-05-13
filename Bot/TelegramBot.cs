namespace ASB.Bot
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using API;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Storage;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class TelegramBot : TelegramBotClient
    {
        private readonly long _notificationChannel;
        private readonly LocalContext _storage;
        private readonly ILogger<TelegramBot> _logger;

        public TelegramBot(IConfiguration configuration, LocalContext storage, ILogger<TelegramBot> logger) 
            : base(configuration["bot_token"]/*, new WebProxy("51.38.71.101", 8080)*/)
        {
            _storage = storage;
            _logger = logger;
            // TODO Need to safe get value
            _notificationChannel = long.Parse(configuration["notification_channel"]);
            OnUpdate += Tick;
        }

        public Task SendServerStatusMessage(ServerStatus status, long chatId = default)
        {
            if (chatId == default) chatId = _notificationChannel;
            if (status == default) return Task.CompletedTask;

            var message = $"Current server status: <b>{status.CurrentStatus}</b>";

            return SendTextMessageAsync(
                chatId,
                message,
                ParseMode.Html
            );
        }

        private async Task SendDebugStatusMessage(ServerStatus status, long chatId)
        {
            await SendTextMessageAsync(
                chatId,
                $"```\n{JsonConvert.SerializeObject(status)}\n```",
                ParseMode.Markdown
            );
        }

        public void Run() => StartReceiving();

        private async void ExecuteCommand(string command, Update update)
        {
            _logger.LogTrace($"[{nameof(ExecuteCommand)}] ({command}) ID:{update.Id}, from @{update.Message.From.Username}");
            // 'start' command required by telegram bot standard
            if (command == "/get_status" || command == "/start")
            {
                var status = await _storage.GetLast();

                await SendServerStatusMessage(status, update.Message.Chat.Id);
            }
            // get raw json from db
            if (command == "/debug_status")
            {
                var status = await _storage.GetLast();

                await SendDebugStatusMessage(status, update.Message.Chat.Id)
                    .ContinueWith(x => x.Status); // skip error when exist)0
            }
        }

        private void Tick(object sender, UpdateEventArgs e)
        {
            if (e.Update.Message?.Entities == null)
                return;

            var entities = e.Update.Message.Entities.Where(x => x.Type == MessageEntityType.BotCommand);

            var messageEntities = entities as MessageEntity[] ?? entities.ToArray();

            if (!messageEntities.Any())
                return;

            foreach (var messageEntity in messageEntities)
            {
                var command = e.Update.Message.Text.Substring(
                    messageEntity.Offset,
                    messageEntity.Length
                );

                ExecuteCommand(command, e.Update);
            }
        }
    }
}