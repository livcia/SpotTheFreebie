using System.Net.Http.Json;
using SpotTheFreebie.Models;
using SpotTheFreebie.Models.ViewModels;

namespace SpotTheFreebie.Services;

public class GameService
{
    private readonly HttpClient _http;
    private int _totalPageCount = 1;
    private bool _pageCountLoaded = false;

    public GameService(HttpClient http)
    {
        _http = http;
    }

    public async Task<FreeGame?> GetRandomFreeGameAsync()
    {
        var games = await _http.GetFromJsonAsync<List<FreeGame>>("https://www.freetogame.com/api/games");
        if (games == null || games.Count == 0) return null;
        return games[Random.Shared.Next(games.Count)];
    }

    public async Task<PaidGame?> GetRandomPaidGameAsync()
    {
        await EnsurePageCountLoadedAsync();
        int page = Random.Shared.Next(0, _totalPageCount);
        var response = await _http.GetAsync($"https://www.cheapshark.com/api/1.0/deals?pageNumber={page}");

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            response = await _http.GetAsync("https://www.cheapshark.com/api/1.0/deals");

        response.EnsureSuccessStatusCode();
        var games = await response.Content.ReadFromJsonAsync<List<PaidGame>>();
        if (games == null || games.Count == 0) return null;
        return games[Random.Shared.Next(games.Count)];
    }

    public async Task<List<GameCardViewModel>> GetGameCardsAsync()
    {
        await EnsurePageCountLoadedAsync();
        int page = Random.Shared.Next(0, _totalPageCount);

        var freeTask = _http.GetFromJsonAsync<List<FreeGame>>("https://www.freetogame.com/api/games");
        var paidResponse = await _http.GetAsync($"https://www.cheapshark.com/api/1.0/deals?pageNumber={page}");

        if (paidResponse.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            paidResponse = await _http.GetAsync("https://www.cheapshark.com/api/1.0/deals");

        paidResponse.EnsureSuccessStatusCode();

        var freeGames = await freeTask;
        var paidGames = await paidResponse.Content.ReadFromJsonAsync<List<PaidGame>>();

        if (freeGames == null || paidGames == null) return new();

        var freeGame = freeGames[Random.Shared.Next(freeGames.Count)];
        var cards = paidGames
            .OrderBy(_ => Random.Shared.Next())
            .Take(3)
            .Select(GameCardViewModel.FromPaidGame)
            .ToList();

        int insertAt = Random.Shared.Next(0, 4);
        cards.Insert(insertAt, GameCardViewModel.FromFreeGame(freeGame));

        return cards;
    }

    public async Task<GameDetails?> GetGameDetailsAsync(string gameId)
    {
        return await _http.GetFromJsonAsync<GameDetails>($"https://www.cheapshark.com/api/1.0/games?id={gameId}");
    }

    public async Task<int> GetTotalPageCountAsync()
    {
        await EnsurePageCountLoadedAsync();
        return _totalPageCount;
    }

    private async Task EnsurePageCountLoadedAsync()
    {
        if (_pageCountLoaded) return;

        try
        {
            var response = await _http.GetAsync("https://www.cheapshark.com/api/1.0/deals");
            if (response.IsSuccessStatusCode &&
                response.Headers.TryGetValues("X-Total-Page-Count", out var values) &&
                int.TryParse(values.FirstOrDefault(), out int count))
            {
                _totalPageCount = count;
            }
        }
        catch
        {
            _totalPageCount = 50;
        }
        finally
        {
            _pageCountLoaded = true;
        }
    }
}
