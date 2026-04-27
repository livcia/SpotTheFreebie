using System.Globalization;

namespace SpotTheFreebie.Models.ViewModels;

public class GamePriceData
{
    public double NormalPrice { get; set; }
    public double SalePrice { get; set; }
    public double CheapestPrice { get; set; }
    public double MaxPrice { get; set; }

    public static GamePriceData From(PaidGame game, GameDetails details)
    {
        var data = new GamePriceData
        {
            NormalPrice = ParsePrice(game.NormalPrice),
            SalePrice = ParsePrice(game.SalePrice),
            CheapestPrice = ParsePrice(details.CheapestPriceEver.Price)
        };

        data.MaxPrice = Math.Max(data.NormalPrice, Math.Max(data.SalePrice, data.CheapestPrice));
        if (data.MaxPrice == 0) data.MaxPrice = 1;

        return data;
    }

    private static double ParsePrice(string raw)
    {
        if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            return value;
        if (double.TryParse(raw.Replace(".", ","), out double value2))
            return value2;
        return 0;
    }
}
