namespace AlbionStatusBot
{
    using System;
    using System.Linq;
    using System.Threading;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using MihaZupan;
    using System.Text;
    using Flurl;
    using Flurl.Http;
    using Newtonsoft.Json;
    

    public partial class Response
    {
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("current_status")]
        public string CurrentStatus { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("comment")]
        public object Comment { get; set; }
    }


    public class Program
    {
        public static ITelegramBotClient botClient;
        public static DateTime start { get; set; }

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Config.StartUp();
            var proxy = new HttpToSocks5Proxy(Config.GetValue<string>("proxy_ip"), Config.GetValue<int>("proxy_port"), Config.GetValue<string>("username"), Config.GetValue<string>("password"));
            botClient = new TelegramBotClient(Config.GetValue<string>("token"), proxy);
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Bot {me.Username} started."
            );
            botClient.MessageOffset = 0;
            botClient.OnUpdate += Bot_OnUpdate;
            botClient.StartReceiving();
            start = DateTime.UtcNow;
            while (true)
                Thread.Sleep(200);
        }

        public static async void Bot_OnUpdate(object sender, UpdateEventArgs e)
        {

            var fc = "https://api.albionstatus.com"
                .AppendPathSegment("current/")
                .GetAsync()
                .ReceiveJson<Response[]>();
            Console.WriteLine(fc.Result.First().CurrentStatus);
            if (e.Update.Message?.Text == null || !e.Update.Message.Text.Contains("/helpkukarek") ||
                e.Update.Message.Date <= start) return;
            Console.WriteLine("Detect help calling!");
            await botClient.SendTextMessageAsync(
                chatId: e.Update.Message.Chat,
                text: "Server status is: " + fc.Result.First().CurrentStatus   + "\nMessage: " + fc.Result.First().Message);
        }
    }
}
