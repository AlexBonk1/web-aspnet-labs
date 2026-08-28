using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.UI.Services.CategoryService;
using WEB_453504_ASP_NET.UI.Services.InstrumentService;

namespace WEB_453504_ASP_NET.UI.Areas_Admin_Pages
{
    public class DetailsModel : PageModel
    {
        private readonly IInstrumentService _instrumentService;
        private readonly ICategoryService _categoryService;

        public DetailsModel(IInstrumentService instrumentService, ICategoryService categoryService)
        {
            _instrumentService = instrumentService;
            _categoryService = categoryService;
        }

        public MusicalInstrument MusicalInstrument { get; set; } = default!;
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _instrumentService.GetInstrumentByIdAsync(id.Value);
            if (result == null || !result.Successfull || result.Data == null)
            {
                return NotFound();
            }

            MusicalInstrument = result.Data;
            //допилить метод get по id
            MusicalInstrument.Category = (await _categoryService.GetCategoryListAsync()).Data.FirstOrDefault(c => c.Id == MusicalInstrument.CategoryId) ?? new Category { Id = MusicalInstrument.CategoryId, Name = "Unknown" };
            return Page();
        }
    }
}
