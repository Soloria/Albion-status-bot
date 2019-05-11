namespace ASB.API
{
    using System.Linq;
    using System.Threading.Tasks;
    using Flurl.Http;

    public class AlbionApiClient
    {
        private readonly string _endpoint = "https://api.albionstatus.com/current/";

        public async Task<ServerStatus> GetServerStatus()
        {
            var data = await _endpoint
                .GetAsync()
                .ReceiveJson<ServerStatus[]>();

            return data.First();
        }
    }
}