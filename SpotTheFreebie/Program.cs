using SpotTheFreebie.Components;
using SpotTheFreebie.Models;
using System.Net.Http.Json;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName, client => 
{
    client.DefaultRequestHeaders.Add("User-Agent", "SpotTheFreebie/1.0");
});
builder.Services.AddScoped<SpotTheFreebie.Services.StoreService>();
builder.Services.AddScoped<SpotTheFreebie.Services.GameService>();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddBlazorBootstrap();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapGet("/api/games/stats", async (IHttpClientFactory httpClientFactory) =>
{
    var http = httpClientFactory.CreateClient();

    try
    {
        var freeResponse = await http.GetAsync("https://www.freetogame.com/api/games");
        if (!freeResponse.IsSuccessStatusCode)
            return Results.Problem($"FreeToGame API zwróciło błąd: {(int)freeResponse.StatusCode}", statusCode: 502);

        var freeGames = await freeResponse.Content.ReadFromJsonAsync<List<FreeGame>>();

        var dealsResponse = await http.GetAsync("https://www.cheapshark.com/api/1.0/deals?sortBy=Savings&pageSize=60");
        if (!dealsResponse.IsSuccessStatusCode)
            return Results.Problem($"CheapShark API zwróciło błąd: {(int)dealsResponse.StatusCode}", statusCode: 502);

        var deals = await dealsResponse.Content.ReadFromJsonAsync<List<PaidGame>>();

        var topGenres = freeGames?
            .GroupBy(g => g.Genre)
            .Select(g => new { Genre = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToList();

        var topDeals = deals?
            .Where(d => double.TryParse(d.Savings, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
            .OrderByDescending(d => double.Parse(d.Savings, CultureInfo.InvariantCulture))
            .Take(10)
            .Select(d => new
            {
                d.Title,
                d.SalePrice,
                d.NormalPrice,
                SavingsPercent = Math.Round(double.Parse(d.Savings, CultureInfo.InvariantCulture), 1)
            })
            .ToList();

        var avgSalePrice = deals?
            .Where(d => double.TryParse(d.SalePrice, NumberStyles.Any, CultureInfo.InvariantCulture, out double v) && v > 0)
            .Select(d => double.Parse(d.SalePrice, CultureInfo.InvariantCulture))
            .DefaultIfEmpty(0)
            .Average();

        return Results.Ok(new
        {
            FreeGamesTotal = freeGames?.Count ?? 0,
            TopGenres = topGenres,
            TopDealsBySavings = topDeals,
            AverageSalePriceUsd = Math.Round(avgSalePrice ?? 0, 2)
        });
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem($"Błąd połączenia z zewnętrznym API: {ex.Message}", statusCode: 503);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Nieoczekiwany błąd serwera: {ex.Message}", statusCode: 500);
    }
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
