using Microsoft.AspNetCore.Components;
using SpotTheFreebie.Models;
using SpotTheFreebie.Services;

namespace SpotTheFreebie.Components.Pages.RandomFreeGame;

public partial class RandomFreeGame
{
    [Inject] private GameService GameService { get; set; } = default!;

    private FreeGame? game;
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
            await Task.Delay(300);
            game = await GameService.GetRandomFreeGameAsync();

            if (game == null)
                errorMessage = "Nie udało się skontaktować z serwerem FreeToGame.";
        }
        catch (Exception ex)
        {
            errorMessage = "Problem z połączeniem internetowym.";
            Console.WriteLine(ex.Message);
        }
    }
}
