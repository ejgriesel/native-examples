using System.Text.Json.Serialization;

namespace HermesPrtgPoc.Dto
{
    // 1. Represents the "BestBid" and "BestOffer" nested objects
    public class PriceVolumeData
    {
        [JsonPropertyName("Value")]
        public decimal Value { get; set; }

        [JsonPropertyName("Volume")]
        public long Volume { get; set; }
    }

    // 2. Represents the "data" object inside the root JSON
    public class MarketData
    {
        public string InstrCode { get; set; }
        public string Exchange { get; set; }
        public PriceVolumeData BestBid { get; set; }
        public PriceVolumeData BestOffer { get; set; }
        public decimal ClosingPrice { get; set; }
        public decimal LastPrice { get; set; }
        public string PeriodName { get; set; }
    }

    // 3. Represents the Root JSON object
    public class MarketResponse
    {
        public MarketData data { get; set; }
    }
}
