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
    public class EditModel : PageModel
    {
        private readonly IInstrumentService _instrumentService;
        private readonly ICategoryService _categoryService;
        [BindProperty]
        public IFormFile? Image { get; set; }
        public EditModel(IInstrumentService instrumentService, ICategoryService categoryService)
        {
            _instrumentService = instrumentService;
            _categoryService = categoryService;
        }

        [BindProperty]
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
            ViewData["CategoryId"] = new SelectList((await _categoryService.GetCategoryListAsync()).Data, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("MusicalInstrument.Category");
            ModelState.Remove("MusicalInstrument.ImageUrl");
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList((await _categoryService.GetCategoryListAsync()).Data, "Id", "Name");
                return Page();
            }

            try
            {
                await _instrumentService.UpdateInstrumentAsync(MusicalInstrument.Id, MusicalInstrument, Image);
            }
            catch (Exception)
            {
                if (!await MusicalInstrumentExists(MusicalInstrument.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> MusicalInstrumentExists(int id)
        {
            var result = await _instrumentService.GetInstrumentByIdAsync(id);
            return result != null && result.Successfull && result.Data != null;
        }
    }
}
