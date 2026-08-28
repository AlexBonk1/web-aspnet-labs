using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.UI;
using WEB_453504_ASP_NET.UI.Services.InstrumentService;

namespace WEB_453504_ASP_NET.UI.Areas_Admin_Pages
{
    public class IndexModel : PageModel
    {
        private readonly  IInstrumentService _instrumentService;
        public IndexModel(IInstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
            //ViewData["page"] = 1;
        }

        public IList<MusicalInstrument> MusicalInstrument { get;set; } = default!;

        public async Task OnGetAsync(int pageNo =1)
        {
           var t = await _instrumentService.GetInstrumentListAsync(null, pageNo);
            MusicalInstrument = t.Data.Items;
            ViewData["PageNo"] = t.Data.CurrentPage;
            ViewData["TotalPages"] = t.Data.TotalPages;

        }
    }
}
