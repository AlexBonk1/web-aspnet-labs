using System;
using System.Collections.Generic;
using System.Text;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;

namespace WEB_453504_ASP_NET.Domain.Services.CategoryService
{
    public interface ICategoryService
    {
        /// <summary>
        /// Получение списка всех категорий
        /// </summary>
        /// <returns></returns>
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}
