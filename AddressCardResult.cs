using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    public class AddressCardResult
    {
        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("houseNumber")]
        public int HouseNumber { get; set; }

        [JsonProperty("zipCode")]
        public int ZipCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }
}
