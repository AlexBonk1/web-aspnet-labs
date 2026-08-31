using System.Text.Json;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;
using WEB_453504_ASP_NET.UI.Services.Authentification;
using WEB_453504_ASP_NET.UI.Services.InstrumentService;

namespace WEB_453504_ASP_NET.UI.Services.CategoryService
{
    public class ApiCategoryService : ICategoryService
    {
        HttpClient _httpClient;
        JsonSerializerOptions _serializerOptions;
        ILogger<ApiCategoryService> _logger;

        ITokenAccessor _tokenAccessor;
        public ApiCategoryService(HttpClient httpClient,
                                      IConfiguration configuration,
                                      ILogger<ApiCategoryService> logger,
                                      ITokenAccessor tokenAccessor)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
            _tokenAccessor = tokenAccessor;
        }

        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var responseData = new ResponseData<List<Category>>();

            var response = await _httpClient.GetAsync(_httpClient.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                responseData = await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>(_serializerOptions);
                return responseData;
            }
            _logger.LogError($"-----> object not created. Error:{response.StatusCode.ToString()}");
            return ResponseData<List<Category>>.Error($"Cannot get category list. Error:{response.StatusCode.ToString()}");
        }

    }
}
