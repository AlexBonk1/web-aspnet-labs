using System.Net.Http.Json;
using WEB_453504_ASP_NET.Domain.Entities;

namespace WEB_453504_ASP_NET.BlazorWasm.Services;

public class DataService : IDataService
{
    HttpClient _httpClient;
    public List<Category> Categories { get; set; } = new();
    public List<MusicalInstrument> Instruments { get; set; } = new();
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public Category SelectedCategory { get; set; }

    public event Action DataLoaded;

    public DataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        CurrentPage = 1;
    }
    public async Task GetCategoryListAsync()
    {
        var response = await _httpClient.GetAsync("api/categories");
        if(!response.IsSuccessStatusCode)
        {
            ErrorMessage = $"Ошибка при получении списка категорий: {response.ReasonPhrase}";
            Success = false;
            return;
        }
        Categories = response.Content.ReadFromJsonAsync<List<Category>>().Result;
    }

    public async Task GetInstrumentsListAsync(int pageNo = 1)
    {
        var response = await _httpClient.GetAsync($"api/instruments?page={pageNo}");
        if(!response.IsSuccessStatusCode)
        {
            ErrorMessage = $"Ошибка при получении списка объектов: {response.ReasonPhrase}";
            Success = false;
            return;
        }
        Instruments = response.Content.ReadFromJsonAsync<List<MusicalInstrument>>().Result ?? new();
    }

}