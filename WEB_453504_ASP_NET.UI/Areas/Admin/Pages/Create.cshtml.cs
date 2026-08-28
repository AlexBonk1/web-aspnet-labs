using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.UI.Services.InstrumentService;
using WEB_453504_ASP_NET.UI.Services.CategoryService;

namespace WEB_453504_ASP_NET.UI.Areas_Admin_Pages
{
    public class CreateModel : PageModel
    {
        private readonly IInstrumentService _instrumentService;
        private readonly ICategoryService _categoryService;
        [BindProperty]
        public IFormFile? Image { get; set; }

        public CreateModel(IInstrumentService instrumentService, ICategoryService categoryService)
        {
            _instrumentService = instrumentService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> OnGet()
        {
            ViewData["CategoryId"] = new SelectList((await _categoryService.GetCategoryListAsync()).Data, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public MusicalInstrument MusicalInstrument { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("MusicalInstrument.Category");
            ModelState.Remove("MusicalInstrument.ImageUrl");
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            var result = await _instrumentService.CreateInstrumentAsync(MusicalInstrument, Image);
            if (result == null || !result.Successfull)
            {
                ModelState.AddModelError(string.Empty, result?.ErrorMessage ?? "Failed to create instrument");
                await LoadCategoriesAsync();
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task LoadCategoriesAsync()
        {
            ViewData["CategoryId"] = new SelectList((await _categoryService.GetCategoryListAsync()).Data,"Id","Name");
        }
    }
}
