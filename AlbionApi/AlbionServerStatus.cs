namespace AlbionStatusBot.AlbionApi
{
    using System;
    using LiteDB;
    using Newtonsoft.Json;

    public class AlbionServerStatus
    {
        public ObjectId Id { get; set; }
        [JsonProperty("created_at")] public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("current_status")] public string CurrentStatus { get; set; }

        [JsonProperty("message")] public string Message { get; set; }

        [JsonProperty("comment")] public object Comment { get; set; }
    }
}