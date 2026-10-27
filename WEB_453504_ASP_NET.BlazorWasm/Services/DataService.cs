using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using WEB_453504_ASP_NET.Domain.Entities;

namespace WEB_453504_ASP_NET.BlazorWasm.Services;

public class ResponseData<T>
{
    public T? Data { get; set; }
    public bool Successfull { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

public class ListModel<T>
{
    public List<T> Items { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
}

public class DataService : IDataService
{
    HttpClient _httpClient;
    IAccessTokenProvider _tokenProvider;
    public List<Category> Categories { get; set; } = new();
    public List<MusicalInstrument> Instruments { get; set; } = new();
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public Category SelectedCategory { get; set; }

    public event Action DataLoaded;

    public DataService(HttpClient httpClient, IAccessTokenProvider tokenProvider)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        CurrentPage = 1;
    }
        public async Task GetCategoryListAsync()
        {
            var response = await _httpClient.GetAsync("api/categories");
            if(!response.IsSuccessStatusCode)
            {
                ErrorMessage = $"Ошибка при получении списка категорий: {response.ReasonPhrase}";
                Success = false;
                DataLoaded?.Invoke();
                return;
            }
            var result = await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>();
            if (result != null && result.Successfull)
            {
                Categories = result.Data ?? new();
                Success = true;
            }
            else
            {
                Categories = new();
                Success = false;
            }
            DataLoaded?.Invoke();
        }

        public async Task GetInstrumentsListAsync(int pageNo = 1, string? categoryNormalizedName = null)
        {
            var tokenRequest = await _tokenProvider.RequestAccessToken();
            if (tokenRequest.TryGetToken(out var token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            }

            if (categoryNormalizedName != null)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.NormalizedName == categoryNormalizedName);
            }
            else 
            {
                SelectedCategory = null;
            }

            List<KeyValuePair<string, string>> queryData = new();

            if (SelectedCategory is not null)
            {
                queryData.Add(KeyValuePair.Create("category", SelectedCategory.NormalizedName));
            }

            if (pageNo > 1)
            {
                queryData.Add(KeyValuePair.Create("pageNo", pageNo.ToString()));
            }

            var query = string.Join("&", queryData.Select(q => $"{q.Key}={q.Value}"));
            var url = query.Length > 0 ? $"api/instruments?{query}" : "api/instruments";

            var response = await _httpClient.GetAsync(url);
            if(!response.IsSuccessStatusCode)
            {
                ErrorMessage = $"Ошибка при получении списка объектов: {response.ReasonPhrase}";
                Success = false;
                DataLoaded?.Invoke();
                return;
            }
            var result = await response.Content.ReadFromJsonAsync<ResponseData<ListModel<MusicalInstrument>>>();
            if (result == null || !result.Successfull)
            {
                ErrorMessage = result?.ErrorMessage ?? "Ошибка при получении списка объектов";
                Success = false;
                DataLoaded?.Invoke();
                return;
            }

            Success = true;
            Instruments = result.Data?.Items ?? new();
            TotalPages = result.Data?.TotalPages ?? 1;
            CurrentPage = result.Data?.CurrentPage ?? 1;
            DataLoaded?.Invoke();
        }

}