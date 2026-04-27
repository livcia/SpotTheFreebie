using System.Text.Json.Serialization;

namespace SpotTheFreebie.Models;

public class FreeGame
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; } = string.Empty;

    [JsonPropertyName("short_description")]
    public string ShortDescription { get; set; } = string.Empty;

    [JsonPropertyName("genre")]
    public string Genre { get; set; } = string.Empty;

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = string.Empty;

    [JsonPropertyName("release_date")]
    public string ReleaseDate { get; set; } = string.Empty;
}
