using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OneBagJianghu.Net
{
    public sealed class Envelope
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long Id { get; set; }

        [JsonProperty("replyTo", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long ReplyTo { get; set; }

        [JsonProperty("payload")]
        public JToken Payload { get; set; }
    }
}
