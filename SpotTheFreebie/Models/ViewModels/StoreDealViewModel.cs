namespace SpotTheFreebie.Models.ViewModels;

public class StoreDealViewModel
{
    public GameDeal Deal { get; set; } = new();
    public Store? StoreInfo { get; set; }
}
