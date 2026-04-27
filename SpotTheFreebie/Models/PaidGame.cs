using System.Text.Json.Serialization;

namespace SpotTheFreebie.Models;

public class PaidGame
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("thumb")]
    public string Thumb { get; set; } = string.Empty;

    [JsonPropertyName("salePrice")]
    public string SalePrice { get; set; } = string.Empty;

    [JsonPropertyName("normalPrice")]
    public string NormalPrice { get; set; } = string.Empty;

    [JsonPropertyName("savings")]
    public string Savings { get; set; } = string.Empty;

    [JsonPropertyName("isOnSale")]
    public string IsOnSale { get; set; } = string.Empty;

    [JsonPropertyName("steamRatingText")]
    public string SteamRatingText { get; set; } = string.Empty;

    [JsonPropertyName("steamRatingPercent")]
    public string SteamRatingPercent { get; set; } = string.Empty;

    [JsonPropertyName("dealID")]
    public string DealID { get; set; } = string.Empty;

    [JsonPropertyName("gameID")]
    public string GameID { get; set; } = string.Empty;
}