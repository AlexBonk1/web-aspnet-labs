using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.UI.Services.InstrumentService;

namespace WEB_453504_ASP_NET.UI.Areas_Admin_Pages
{
    public class DeleteModel : PageModel
    {
        private readonly IInstrumentService _instrumentService;

        public DeleteModel(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _instrumentService.DeleteInstrumentAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
