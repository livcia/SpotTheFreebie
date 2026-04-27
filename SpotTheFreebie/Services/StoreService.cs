using System.Net.Http.Json;
using SpotTheFreebie.Models;

namespace SpotTheFreebie.Services;

public class StoreService
{
    private readonly HttpClient _http;
    private List<Store>? _cachedStores = null;

    public StoreService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Store>> GetStoresAsync()
    {
        if (_cachedStores == null)
        {
            try
            {
                string url = "https://www.cheapshark.com/api/1.0/stores";
                var response = await _http.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    _cachedStores = await response.Content.ReadFromJsonAsync<List<Store>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd pobierania sklepów: {ex.Message}");
            }
        }
        
        return _cachedStores ?? new List<Store>();
    }

    public async Task<Store?> GetStoreByIdAsync(string storeId)
    {
        var stores = await GetStoresAsync();
        return stores.FirstOrDefault(s => s.StoreID == storeId);
    }
}
