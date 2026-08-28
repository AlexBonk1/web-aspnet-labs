using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Metrics;
using WEB_453504_ASP_NET.API.Data;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;

namespace WEB_453504_ASP_NET.API.UseCases
{
    public sealed record GetListOfInstruments(string? categoryNormalizedName,int pageNo = 1,int pageSize = 3) : IRequest<ResponseData<ListModel<MusicalInstrument>>>;
    public class GetListOfProductsHandler(AppDbContext db) : IRequestHandler<GetListOfInstruments, ResponseData<ListModel<MusicalInstrument>>>
    {
        private readonly int _maxPageSize = 20;

        int _filteredCount = 0;
        int _pageSize = 3;
        private int PageCount
        {
            get
            {
                int count = _filteredCount;
                var ceil = Math.Ceiling((decimal)count / _pageSize);
                return (int)ceil;
            }
        }

        public async Task<ResponseData<ListModel<MusicalInstrument>>> Handle(GetListOfInstruments request, CancellationToken cancellationToken)
        {
            var pageNo = request.pageNo;
            if (request.pageNo <= 0) { pageNo = 1; }

            var data = new ResponseData<ListModel<MusicalInstrument>>();
            try
            {

                var filtered = db.MusicalInstruments.Where(i => request.categoryNormalizedName == null || i.Category.NormalizedName == request.categoryNormalizedName).OrderBy(i => i.Id);
                _filteredCount = filtered.Count();
                _pageSize = request.pageSize;
                var pcount = Math.Min(PageCount, pageNo);
                var pagination = filtered.Skip((pcount - 1) * _pageSize).Take(_pageSize);
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
            return data;
        }
    }
}
