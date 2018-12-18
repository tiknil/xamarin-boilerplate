using System;
using Newtonsoft.Json;

namespace XamarinBoilerplate.Models
{
    public enum ErrorCodes
    {
        NetworkUnreachable = 101,
        NetworkTimeout = 102,
        ServerError = 500,
    }

    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
