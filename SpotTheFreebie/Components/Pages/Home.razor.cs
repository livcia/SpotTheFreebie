using Microsoft.AspNetCore.Components;
using SpotTheFreebie.Models;
using SpotTheFreebie.Models.ViewModels;
using SpotTheFreebie.Services;
using BlazorBootstrap;

namespace SpotTheFreebie.Components.Pages;

public partial class Home
{
    [Inject] private GameService GameService { get; set; } = default!;

    private int? selectedCard = null;
    private List<GameCardViewModel>? gameCards;
    private bool isChecked = false;

    private GameCardViewModel? selectedTabGame = null;
    private GameDetails? selectedTabGameDetails = null;
    private bool isLoadingChart = false;
    private BarChart barChart = default!;
    private BarChartOptions barChartOptions = default!;
    private BlazorBootstrap.ChartData blazorChartData = default!;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            InitBarChartOptions();
            await LoadGamesAsync();
        }
        catch (Exception ex)
        {
            errorMessage = "Wystąpił błąd podczas inicjalizacji aplikacji.";
            Console.WriteLine(ex.Message);
        }
    }

    private void InitBarChartOptions()
    {
        barChartOptions = new BarChartOptions { Responsive = true, MaintainAspectRatio = false };
        barChartOptions.Plugins.Legend.Display = false;

        barChartOptions.Scales.X ??= new();
        barChartOptions.Scales.X.Ticks ??= new();
        barChartOptions.Scales.X.Ticks.Color = "#ffffff";
        barChartOptions.Scales.X.Grid ??= new();
        barChartOptions.Scales.X.Grid.Color = "#333333";

        barChartOptions.Scales.Y ??= new();
        barChartOptions.Scales.Y.Ticks ??= new();
        barChartOptions.Scales.Y.Ticks.Color = "#ffffff";
        barChartOptions.Scales.Y.Grid ??= new();
        barChartOptions.Scales.Y.Grid.Color = "#333333";

        barChartOptions.Plugins ??= new();
        barChartOptions.Plugins.Title ??= new();
        barChartOptions.Plugins.Title.Display = true;
        barChartOptions.Plugins.Title.Text = "Zestawienie cen (USD)";
        barChartOptions.Plugins.Title.Color = "#f48fb1";
    }

    private async Task LoadGamesAsync()
    {
        try
        {
            errorMessage = null;
            gameCards = await GameService.GetGameCardsAsync();
        }
        catch (Exception ex)
        {
            errorMessage = "Błąd połączenia z serwerem API.";
            Console.WriteLine(ex.Message);
        }
    }

    private void SelectCard(int index)
    {
        if (isChecked) return;
        selectedCard = index;
    }

    private async Task CheckAnswer()
    {
        isChecked = true;
        var firstPaid = gameCards?.FirstOrDefault(g => !g.IsFree && !string.IsNullOrEmpty(g.GameID));
        if (firstPaid != null)
            await SelectTabGame(firstPaid);
    }

    private async Task SelectTabGame(GameCardViewModel card)
    {
        selectedTabGame = card;
        selectedTabGameDetails = null;
        isLoadingChart = true;

        try
        {
            selectedTabGameDetails = await GameService.GetGameDetailsAsync(card.GameID);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("API Error: " + ex.Message);
        }

        isLoadingChart = false;
        await InvokeAsync(StateHasChanged);
        await RenderChartAsync();
    }

    private async Task RenderChartAsync()
    {
        if (selectedTabGame == null || selectedTabGameDetails == null) return;

        var paid = new PaidGame
        {
            NormalPrice = selectedTabGame.NormalPrice,
            SalePrice = selectedTabGame.SalePrice
        };
        var cd = GamePriceData.From(paid, selectedTabGameDetails);

        var dataset = new BarChartDataset
        {
            Label = "Cena w $",
            Data = new List<double?> { cd.NormalPrice, cd.SalePrice, cd.CheapestPrice },
            BackgroundColor = new List<string> { "rgba(108, 117, 125, 0.7)", "rgba(0, 230, 118, 0.7)", "rgba(23, 162, 184, 0.7)" },
            BorderColor = new List<string> { "rgb(108, 117, 125)", "rgb(0, 230, 118)", "rgb(23, 162, 184)" },
            BorderWidth = new List<double> { 1, 1, 1 }
        };

        blazorChartData = new BlazorBootstrap.ChartData
        {
            Labels = new List<string> { "Cena Detaliczna", "Obecna Promocja", "Historyczne Minimum" },
            Datasets = new List<IChartDataset> { dataset }
        };

        if (barChart != null)
            await barChart.InitializeAsync(blazorChartData, barChartOptions);
    }

    private async Task RestartGameAsync()
    {
        isChecked = false;
        selectedCard = null;
        gameCards = null;
        selectedTabGame = null;
        selectedTabGameDetails = null;
        await LoadGamesAsync();
    }
}
