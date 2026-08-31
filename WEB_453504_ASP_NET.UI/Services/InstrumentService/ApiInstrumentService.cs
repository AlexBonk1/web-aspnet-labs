
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text;
using System.Text.Json;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;
using WEB_453504_ASP_NET.UI.Controllers;
using WEB_453504_ASP_NET.UI.Services.CategoryService;
using WEB_453504_ASP_NET.UI.Services.Authentification;

namespace WEB_453504_ASP_NET.UI.Services.InstrumentService
{
    public class ApiInstrumentService :  IInstrumentService
    {
        HttpClient _httpClient;
        JsonSerializerOptions _serializerOptions;
        string _pageSize;
        ILogger<ApiInstrumentService> _logger;
        ITokenAccessor _tokenAccessor;
        
        public ApiInstrumentService(HttpClient httpClient,
                                      IConfiguration configuration,
                                      ILogger<ApiInstrumentService> logger,
                                      ITokenAccessor tokenAccessor)
        {
            _httpClient = httpClient;
            _pageSize = configuration.GetSection("ItemsPerPage").Value;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
            _tokenAccessor = tokenAccessor;
        }

        public async Task<ResponseData<MusicalInstrument>> CreateInstrumentAsync(MusicalInstrument instrument, IFormFile? formFile)
        {
            try
            {
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
            }
            catch (Exception e)
            {
                return ResponseData<MusicalInstrument>
                    .Error($"Объект не добавлен. Error: {e.Message}");
            }

            instrument.ImageUrl = "images/no-image.jpg";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = _httpClient.BaseAddress
            };
            var content = new MultipartFormDataContent();
            // Добавить файл изображения
            if (formFile != null)
            {
                var streamContent = new StreamContent(formFile.OpenReadStream());
                content.Add(streamContent, "file", formFile.FileName);
            }
            content.Add(new StringContent(instrument.Name), "name");
            content.Add(new StringContent(instrument.Description), "description");
            content.Add(new StringContent(instrument.CategoryId.ToString()), "categoryId");
            content.Add(new StringContent(instrument.Price.ToString()), "price");
            content.Add(new StringContent(instrument.WeightKg.ToString()), "weightKg");
            if (formFile != null)
            {
                var fileContent = new StreamContent(formFile.OpenReadStream());
                content.Add(fileContent, "file", formFile.FileName);
            }
            request.Content = content;
            var response = await _httpClient.SendAsync(request,CancellationToken.None);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<ResponseData<MusicalInstrument>>(_serializerOptions);
                return responseData ?? ResponseData<MusicalInstrument>.Error("Пустой ответ от API.");
            }
            _logger.LogError($"-----> object not created. Error:{response.StatusCode.ToString()}");
            return ResponseData<MusicalInstrument>.Error($"Объект не добавлен. Error:{response.StatusCode.ToString()}");
        }

        public async Task DeleteInstrumentAsync(int id)
        {
            try
            {
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
            }
            catch (Exception e)
            {
                _logger.LogError($"-----> object not deleted. Error: {e.Message}");
                return;
            }

            var urlString = $"{_httpClient.BaseAddress}{id}";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(urlString)
            };
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"----> object {id} update succesfully");
                return;
            }
            _logger.LogError($"-----> object not deleted. Error:{response.StatusCode.ToString()}");
        }

        public async Task<ResponseData<MusicalInstrument>> GetInstrumentByIdAsync(int id)
        {
            var urlString = $"{_httpClient.BaseAddress}{id}";
            var response = await _httpClient.GetAsync(urlString);
            var respData = new ResponseData<MusicalInstrument>();
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    respData.Successfull = true;
                    var result =  await response.Content.ReadFromJsonAsync<MusicalInstrument>(_serializerOptions);
                    respData.Data = result;
                    return respData;
                }
                catch (JsonException ex)
                {
                    respData.Successfull = false;
                    respData.ErrorMessage = ex.Message;
                    _logger.LogError($"-----> Ошибка: {ex.Message}");
                    return ResponseData<MusicalInstrument>.Error($"Ошибка: {ex.Message}");
                }
            }
            _logger.LogError($"-----> Данные не получены от сервера. Error: {response.StatusCode.ToString()}");
            return ResponseData<MusicalInstrument>.Error($"Данные не получены от сервера. Error: {response.StatusCode.ToString()}");
        }

        public async Task<ResponseData<ListModel<MusicalInstrument>>> GetInstrumentListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}");
            // добавить категорию в маршрут
            var query = QueryString.Empty;
            if (categoryNormalizedName != null)
            {
                query = query.Add(QueryString.Create("category", categoryNormalizedName));
            }

            if (pageNo > 1)
            {
                query = query.Add(QueryString.Create("pageNo", pageNo.ToString()));
            }

            if (!_pageSize.Equals("3"))
            {
                query = query.Add(QueryString.Create("pageSize", _pageSize));
            }
            urlString.Append(query);
            _logger.LogInformation($"-----> Запрос к API: {urlString.ToString()}");
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
        public async Task UpdateInstrumentAsync(int id, MusicalInstrument instrument, IFormFile? formFile)
        {
            try
            {
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
            }
            catch (Exception e)
            {
                _logger.LogError($"-----> object not updated. Error: {e.Message}");
                return;
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{ _httpClient.BaseAddress}{id}")
            };
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(instrument.Name), "name");
            content.Add(new StringContent(instrument.Description), "description");
            content.Add(new StringContent(instrument.CategoryId.ToString()), "categoryId");
            content.Add(new StringContent(instrument.Price.ToString()), "price");
            content.Add(new StringContent(instrument.WeightKg.ToString()), "weightKg");

            if (formFile != null)
            {
                var fileContent = new StreamContent(formFile.OpenReadStream());
                content.Add(fileContent, "file", formFile.FileName);
            }

            request.Content = content;
            var response = await _httpClient.SendAsync(request);

            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"----> object {id} update succesfully");
                return;
            }
            _logger.LogError($"-----> object not created. Error:{response.StatusCode.ToString()}");
            return;
        }
    }
}
