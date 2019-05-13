namespace ASB.API
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;

    public class ServerStatus
    {
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("current_status")]
        public string CurrentStatus { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("comment")]
        [NotMapped] // Need data-type of this prop?
        public object Comment { get; set; }


        #region efc
        /// <summary>
        /// EF-Core Unique ID
        /// </summary>
        [Key, JsonIgnore]
        public Guid UID { get; set; }
        #endregion
    }
}