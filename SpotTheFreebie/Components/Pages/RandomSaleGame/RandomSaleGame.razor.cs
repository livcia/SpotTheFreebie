using Microsoft.AspNetCore.Components;
using SpotTheFreebie.Models;
using SpotTheFreebie.Models.ViewModels;
using SpotTheFreebie.Services;

namespace SpotTheFreebie.Components.Pages.RandomSaleGame;

public partial class RandomSaleGame
{
    [Inject] private GameService GameService { get; set; } = default!;
    [Inject] private StoreService StoreService { get; set; } = default!;

    private PaidGame? game;
    private List<StoreDealViewModel> currentDeals = new();
    private bool isLoadingDeals = false;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadRandomGameAsync();
    }

    private async Task LoadRandomGameAsync()
    {
        try
        {
            errorMessage = null;
            game = null;
            currentDeals.Clear();
            isLoadingDeals = true;
            await Task.Delay(300);

            game = await GameService.GetRandomPaidGameAsync();

            if (game == null)
            {
                errorMessage = "Błąd serwera CheapShark przy losowaniu gry.";
                return;
            }

            await LoadGameDealsAsync(game.GameID);
        }
        catch (Exception ex)
        {
            errorMessage = "Wystąpił problem z połączeniem.";
            Console.WriteLine(ex.Message);
        }
        finally
        {
            isLoadingDeals = false;
        }
    }

    private async Task LoadGameDealsAsync(string gameId)
    {
        try
        {
            var details = await GameService.GetGameDetailsAsync(gameId);
            if (details == null) return;

            foreach (var deal in details.Deals)
            {
                var store = await StoreService.GetStoreByIdAsync(deal.StoreID);
                currentDeals.Add(new StoreDealViewModel { Deal = deal, StoreInfo = store });
            }
        }
        catch (Exception ex)
        {
            errorMessage = "Wystąpił błąd przy pobieraniu szczegółów gry.";
            Console.WriteLine(ex.Message);
        }
    }
}
