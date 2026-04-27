namespace SpotTheFreebie.Models.ViewModels;

public class GameCardViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public string NormalPrice { get; set; } = string.Empty;
    public string SalePrice { get; set; } = string.Empty;
    public string Savings { get; set; } = string.Empty;
    public string GameID { get; set; } = string.Empty;
    public bool IsFree => NormalPrice == "Free";

    public static GameCardViewModel FromFreeGame(FreeGame game) => new()
    {
        Title = game.Title,
        Thumbnail = game.Thumbnail,
        NormalPrice = "Free",
        SalePrice = "Free",
        Savings = "100"
    };

    public static GameCardViewModel FromPaidGame(PaidGame game) => new()
    {
        Title = game.Title,
        Thumbnail = game.Thumb,
        NormalPrice = game.NormalPrice,
        SalePrice = game.SalePrice,
        Savings = game.Savings,
        GameID = game.GameID
    };
}
