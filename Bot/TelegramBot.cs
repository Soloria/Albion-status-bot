namespace AlbionStatusBot.Bot
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AlbionApi;
    using Microsoft.Extensions.Configuration;
    using Storage;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class TelegramBot : TelegramBotClient
    {
        private readonly long _notificationChannel;
        private readonly StatusStorage _storage;

        public TelegramBot(
            IConfiguration configuration,
            IWebProxy webProxy, StatusStorage storage
        ) : base(configuration["bot_token"], webProxy)
        {
            _storage = storage;
            _notificationChannel = long.Parse(configuration["notification_channel"]);
            OnUpdate += Tick;
        }

        public Task SendServerStatusMessage(AlbionServerStatus status, long chatId = default)
        {
            if (chatId == default)
            {
                chatId = _notificationChannel;
            }

            var message = $"Current server status: <b>{status.CurrentStatus}</b>";

            return SendTextMessageAsync(
                chatId,
                message,
                ParseMode.Html
            );
        }

        public void Run()
        {
            StartReceiving();
        }

        private async void ExecuteCommand(string command, Update update)
        {
            if (command == "/get_status")
            {
                var status = _storage.GetLast();

                await SendServerStatusMessage(status, update.Message.Chat.Id);
            }
        }

        private void Tick(object sender, UpdateEventArgs e)
        {
            if (e.Update.Message?.Entities == null)
            {
                return;
            }

            var entities = e.Update.Message.Entities.Where(x => x.Type == MessageEntityType.BotCommand);

            var messageEntities = entities as MessageEntity[] ?? entities.ToArray();

            if (!messageEntities.Any())
            {
                return;
            }

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