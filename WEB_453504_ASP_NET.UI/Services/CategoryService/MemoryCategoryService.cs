using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;
using WEB_453504_ASP_NET.Domain.Services.CategoryService;

namespace WEB_453504_ASP_NET.UI.Services.CategoryService
{
    public class MemoryCategoryService : ICategoryService
    {
        public Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var categories = new List<Category>
            {
            new Category {Id=1, Name="Гитары",
            NormalizedName="guitars"},
            new Category {Id=2, Name="Басс гитары",NormalizedName="bass-guitars"},
            new Category {Id=3, Name="Скрипки", NormalizedName="violins"},
            new Category {Id=4, Name="Акустические гитары", NormalizedName="acoustic-guitars"},
            new Category {Id=5, Name="Барабаны", NormalizedName="drums"},
            };
            var result = ResponseData<List<Category>>.Success(categories);
            return Task.FromResult(result);
        }
    }
}
