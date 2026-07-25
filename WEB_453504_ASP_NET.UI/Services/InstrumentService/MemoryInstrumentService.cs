using Microsoft.AspNetCore.Mvc;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;
using WEB_453504_ASP_NET.Domain.Services.CategoryService;
using WEB_453504_ASP_NET.Domain.Services.InstrumentService;

namespace WEB_453504_ASP_NET.UI.Services.InstrumentService
{
    public class MemoryInstrumentService : IInstrumentService
    {

        List<MusicalInstrument> _instruments;
        List<Category> _categories;
        List<MusicalInstrument> _filtered;
        int pageNo;
        int pageSize;
        private int PageCount {get {
                int count = _filtered.Count;
                var ceil = Math.Ceiling((decimal)count/ pageSize);
                return (int)ceil;
            }}
        public MemoryInstrumentService([FromServices] IConfiguration config, ICategoryService categoryService)
        {
                _categories = categoryService.GetCategoryListAsync()
                .Result
                .Data;
            pageSize = int.Parse(config["ItemsPerPage"]);
            SetupData();
        }
        private void SetupData()
        {
            var g = (Func<string, Category?>)(n => _categories.Find(c => c.NormalizedName == n));

            _instruments = new List<MusicalInstrument>
            {
                new MusicalInstrument
                {
                    Id = 1,
                    Name = "Fender Stratocaster",
                    Description = "Классическая электрогитара",
                    ImageUrl = "fender-sracocaster.jpg",
                    Category = g("guitars"),
                    CategoryId = g("guitars")?.Id ?? 0,
                    Price = 1299.99m,
                    WeightKg = 3.5
                },
                new MusicalInstrument
                {
                    Id = 11,
                    Name = "Fender Stratocaster",
                    Description = "Классическая электрогитара",
                    ImageUrl = "fender-sracocaster.jpg",
                    Category = g("guitars"),
                    CategoryId = g("guitars")?.Id ?? 0,
                    Price = 1299.99m,
                    WeightKg = 3.5
                },
                new MusicalInstrument
                {
                    Id = 12,
                    Name = "Fender Stratocaster",
                    Description = "Классическая электрогитара",
                    ImageUrl = "fender-sracocaster.jpg",
                    Category = g("guitars"),
                    CategoryId = g("guitars")?.Id ?? 0,
                    Price = 1299.99m,
                    WeightKg = 3.5
                },
                new MusicalInstrument
                {
                    Id = 2,
                    Name = "Gibson Les Paul",
                    Description = "Легендарная электрогитара",
                    ImageUrl = "lespaul.jpg",
                    Category = g("guitars"),
                    CategoryId = g("guitars")?.Id ?? 0,
                    Price = 2499.50m,
                    WeightKg = 4.2
                },
                new MusicalInstrument
                {
                    Id = 3,
                    Name = "Ibanez SR500",
                    Description = "Современная бас-гитара",
                    ImageUrl = "ibanez_bass.jpg",
                    Category = g("bass-guitars"),
                    CategoryId = g("bass-guitars")?.Id ?? 0,
                    Price = 699.00m,
                    WeightKg = 3.8
                },
                new MusicalInstrument
                {
                    Id = 4,
                    Name = "Fender Precision Bass",
                    Description = "Классический бас",
                    ImageUrl = "precision_bass.jpg",
                    Category = g("bass-guitars"),
                    CategoryId = g("bass-guitars")?.Id ?? 0,
                    Price = 899.00m,
                    WeightKg = 4.0
                },
                new MusicalInstrument
                {
                    Id = 5,
                    Name = "Stentor Student II",
                    Description = "Учебная скрипка",
                    ImageUrl = "stentor_violin.jpg",
                    Category = g("violins"),
                    CategoryId = g("violins")?.Id ?? 0,
                    Price = 199.99m,
                    WeightKg = 0.6
                },
                new MusicalInstrument
                {
                    Id = 6,
                    Name = "Cremona SV-500",
                    Description = "Качественная скрипка среднего уровня",
                    ImageUrl = "cremona_violin.jpg",
                    Category = g("violins"),
                    CategoryId = g("violins")?.Id ?? 0,
                    Price = 749.00m,
                    WeightKg = 0.7
                },
                new MusicalInstrument
                {
                    Id = 7,
                    Name = "Yamaha FG800",
                    Description = "Надёжная акустическая гитара",
                    ImageUrl = "acoustic_fg800.jpg",
                    Category = g("acoustic-guitars"),
                    CategoryId = g("acoustic-guitars")?.Id ?? 0,
                    Price = 199.00m,
                    WeightKg = 2.1
                },
                new MusicalInstrument
                {
                    Id = 8,
                    Name = "Takamine GD30",
                    Description = "Акустическая гитара среднего класса",
                    ImageUrl = "takamine.jpg",
                    Category = g("acoustic-guitars"),
                    CategoryId = g("acoustic-guitars")?.Id ?? 0,
                    Price = 349.00m,
                    WeightKg = 2.3
                },
                new MusicalInstrument
                {
                    Id = 9,
                    Name = "Pearl Export",
                    Description = "Ударная установка для репетиций",
                    ImageUrl = "pearl_drums.jpg",
                    Category = g("drums"),
                    CategoryId = g("drums")?.Id ?? 0,
                    Price = 599.00m,
                    WeightKg = 18.0
                },
                new MusicalInstrument
                {
                    Id = 10,
                    Name = "Ludwig Accent",
                    Description = "Комплект барабанов для начинающих",
                    ImageUrl = "ludwig_drums.jpg",
                    Category = g("drums"),
                    CategoryId = g("drums")?.Id ?? 0,
                    Price = 449.00m,
                    WeightKg = 16.5
                }
            };
        }
        
        public Task<ResponseData<MusicalInstrument>> CreateInstrumentAsync(MusicalInstrument instrument, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteInstrumentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<MusicalInstrument>> GetInstrumentByIdAsync(int id)
        {
            var data = new ResponseData<MusicalInstrument>();
            try
            {
                var instrument = _instruments[id];
                data.Successfull = true;
                data.Data = instrument;
            }
            catch(NullReferenceException ex)
            {
                data.Successfull = false;
                data.ErrorMessage = ex.Message;
            }
            return Task.FromResult(data);
        }

        public Task<ResponseData<ListModel<MusicalInstrument>>> GetInstrumentListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            this.pageNo = pageNo;
            var data = new ResponseData<ListModel<MusicalInstrument>>();
            try
            {
                    var filtered = _instruments.Where(i => categoryNormalizedName == null ? true : i.Category.NormalizedName == categoryNormalizedName);
                this._filtered = filtered.ToList();
                    if(pageNo <=0) { pageNo = 1; }    
                    var pcount = Math.Min(PageCount,pageNo);
                    var pagination = filtered.Skip((pcount - 1) * pageSize).Take(pageSize);
                    var list = new ListModel<MusicalInstrument>()
                    {
                        Items = pagination.ToList(),
                        CurrentPage = pageNo,
                        TotalPages = PageCount
                    };
                    data.Successfull = true;
                    data.Data = list;
                }
            catch (Exception ex)
            {
                data.Successfull = false;
                data.ErrorMessage = ex.Message;
            }
            return Task.FromResult(data);

        }

        public Task UpdateInstrumentAsync(int id, MusicalInstrument instrument, IFormFile? formFile)
        {
            var data = new ResponseData<MusicalInstrument>();
            try
            {
                _instruments[id] = instrument;
                data.Successfull = true;
                data.Data = instrument;
            }
            catch (NullReferenceException ex)
            {
                data.Successfull = false;
                data.ErrorMessage = ex.Message;
            }
            return Task.FromResult(data);
        }
    }
}
