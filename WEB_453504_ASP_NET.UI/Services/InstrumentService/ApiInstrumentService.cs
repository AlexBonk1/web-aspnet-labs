
using System.Text;
using System.Text.Json;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;
using WEB_453504_ASP_NET.UI.Controllers;
using WEB_453504_ASP_NET.UI.Services.CategoryService;

namespace WEB_453504_ASP_NET.UI.Services.InstrumentService
{
    public class ApiInstrumentService :  IInstrumentService
    {
        HttpClient _httpClient;
        JsonSerializerOptions _serializerOptions;
        string _pageSize;
        ILogger<ApiInstrumentService> _logger;
        public ApiInstrumentService(HttpClient httpClient,
                                      IConfiguration configuration,
                                      ILogger<ApiInstrumentService> logger)
        {
            _httpClient = httpClient;
            _pageSize = configuration.GetSection("ItemsPerPage").Value;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
        }
        public async Task<ResponseData<MusicalInstrument>> CreateInstrumentAsync(MusicalInstrument instrument, IFormFile? formFile)
        {
            instrument.ImageUrl = "Images/no-image.jpg";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = _httpClient.BaseAddress
            };
            request.Content = new StringContent(JsonSerializer.Serialize(instrument));
            var response = await _httpClient.SendAsync(request,CancellationToken.None);
            if (response.IsSuccessStatusCode)
            {
           
            var responseData = await response.Content.ReadFromJsonAsync<ResponseData<MusicalInstrument>> (_serializerOptions);
            return responseData;
            }
            _logger.LogError($"-----> object not created. Error:{response.StatusCode.ToString()}");
            return ResponseData<MusicalInstrument>.Error($"Объект не добавлен. Error:{response.StatusCode.ToString()}");
    }

        public Task DeleteInstrumentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<MusicalInstrument>> GetInstrumentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseData<ListModel<MusicalInstrument>>> GetInstrumentListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}");
            // добавить категорию в маршрут
            if (categoryNormalizedName != null)
            {
                urlString.Append($"{categoryNormalizedName}/");
            }
            if (pageNo > 1)
            {
                urlString.Append($"page{pageNo}");
            }
            ;
            // добавить размер страницы в строку запроса
            if (!_pageSize.Equals("3"))
            {
                urlString.Append(QueryString.Create("pageSize", _pageSize));
            }
            // отправить запрос к API
            var response = await _httpClient.GetAsync(
            new Uri(urlString.ToString()));
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<ListModel<MusicalInstrument>>>(_serializerOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<ListModel<MusicalInstrument>>
                    .Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"-----> Данные не получены от сервера. Error: {response.StatusCode.ToString()}");
            return ResponseData<ListModel<MusicalInstrument>>.Error($"Данные не получены от сервера. Error: {response.StatusCode.ToString()}");
        }
        public Task UpdateInstrumentAsync(int id, MusicalInstrument instrument, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }
    }
}
