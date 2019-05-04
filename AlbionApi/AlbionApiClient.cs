namespace AlbionStatusBot.AlbionApi
{
    using System.Linq;
    using System.Threading.Tasks;
    using Flurl.Http;

    public class AlbionApiClient
    {
        private readonly string _endpoint = "https://api.albionstatus.com/current/";

        public async Task<AlbionResponse> GetServerStatus()
        {
            var data = await _endpoint
                .GetAsync()
                .ReceiveJson<AlbionResponse[]>();

            return data.First();
        }
    }
}