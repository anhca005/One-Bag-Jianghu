using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OneBagJianghu.Net
{
    public static class ProtocolJson
    {
        private static readonly JsonSerializer Serializer = JsonSerializer.CreateDefault();

        public static Envelope Build(string type, long id, object payload)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Envelope type is required.", nameof(type));
            }

            return new Envelope
            {
                Type = type,
                Id = id,
                Payload = payload == null ? new JObject() : JToken.FromObject(payload, Serializer)
            };
        }

        public static string Serialize(Envelope envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            return JsonConvert.SerializeObject(envelope, Formatting.None);
        }

        public static Envelope Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new FormatException("Envelope JSON is empty.");
            }

            try
            {
                Envelope envelope = JsonConvert.DeserializeObject<Envelope>(json);
                if (envelope == null || string.IsNullOrWhiteSpace(envelope.Type))
                {
                    throw new FormatException("Envelope type is required.");
                }

                envelope.Payload = envelope.Payload ?? new JObject();
                return envelope;
            }
            catch (JsonException exception)
            {
                throw new FormatException("Invalid envelope JSON: " + exception.Message, exception);
            }
        }
    }
}
