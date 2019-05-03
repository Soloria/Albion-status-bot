namespace AlbionStatusBot.Bot
{
    using System.Linq;
    using System.Net;
    using AlbionApi;
    using Microsoft.Extensions.Configuration;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class TelegramBot : TelegramBotClient
    {
        private readonly AlbionApiClient _albionApiClient;

        public TelegramBot(
            IConfiguration configuration,
            AlbionApiClient albionApiClient,
            IWebProxy webProxy
        ) : base(configuration["bot_token"], webProxy)
        {
            _albionApiClient = albionApiClient;
            OnUpdate += Tick;
        }

        public void Run()
        {
            StartReceiving();
        }

        private async void ExecuteCommand(string command, Update update)
        {
            if (command == "/get_status")
            {
                var status = await _albionApiClient.GetServerStatus();

                var apiMessage = char.ToUpper(status.Message[0]) + status.Message.Substring(1);

                var message = $"<b>{apiMessage}</b>\n\n" + 
                              $"( server status: <b>{status.CurrentStatus}</b> )";

                await SendTextMessageAsync(
                    update.Message.Chat.Id,
                    message,
                    ParseMode.Html
                );
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

            if (!messageEntities.Any()) return;

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