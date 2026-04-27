using System.Text.Json.Serialization;

namespace SpotTheFreebie.Models;

public class Store
{
    [JsonPropertyName("storeID")]
    public string StoreID { get; set; } = string.Empty;

    [JsonPropertyName("storeName")]
    public string StoreName { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public StoreImages Images { get; set; } = new();
}

public class StoreImages
{
    [JsonPropertyName("logo")]
    public string Logo { get; set; } = string.Empty;
}
