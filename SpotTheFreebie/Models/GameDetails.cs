using System.Text.Json.Serialization;

namespace SpotTheFreebie.Models;

public class GameDetails
{
    [JsonPropertyName("deals")]
    public List<GameDeal> Deals { get; set; } = new();

    [JsonPropertyName("cheapestPriceEver")]
    public CheapestPrice CheapestPriceEver { get; set; } = new();
}

public class CheapestPrice
{
    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;

    [JsonPropertyName("date")]
    public long Date { get; set; }
}

public class GameDeal
{
    [JsonPropertyName("storeID")]
    public string StoreID { get; set; } = string.Empty;

    [JsonPropertyName("dealID")]
    public string DealID { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;

    [JsonPropertyName("retailPrice")]
    public string RetailPrice { get; set; } = string.Empty;

    [JsonPropertyName("savings")]
    public string Savings { get; set; } = string.Empty;
}
