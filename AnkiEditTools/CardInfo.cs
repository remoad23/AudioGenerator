using System.Text.Json.Serialization;

namespace AnkiEditTools;

public class CardInfo
{
        [JsonPropertyName("note")]
        public long Note { get; set; }

        [JsonPropertyName("cardId")]
        public long CardId { get; set; }

        [JsonPropertyName("deckName")]
        public string DeckName { get; set; }

        [JsonPropertyName("fields")]
        public Dictionary<string, FieldValue> Fields { get; set; }
}

public class FieldValue
{
        [JsonPropertyName("value")]
        public string Value { get; set; }
}