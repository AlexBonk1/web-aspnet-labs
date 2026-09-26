using WEB_453504_ASP_NET.Domain.Entities;
namespace WEB_453504_ASP_NET.BlazorWasm.Services;
public interface IDataService
{
// Событие, генерируемое при изменении данных
event Action DataLoaded;
// Список категорий объектов
List<Category> Categories { get; set; }
//Список объектов
List<MusicalInstrument> Instruments { get; set; }
// Признак успешного ответа на запрос к Api
bool Success { get; set; }
// Сообщение об ошибке
string ErrorMessage { get; set; }
// Количество страниц списка
int TotalPages { get; set; }
// Номер текущей страницы
int CurrentPage { get; set; }
// Фильтр по категории
Category SelectedCategory { get; set; }
/// <summary>
/// Получение списка всех объектов
/// </summary>
/// <param name="pageNo">номер страницы списка</param>
/// <returns></returns>
public Task GetInstrumentsListAsync(int pageNo = 1);
/// <summary>
/// Получение списка категорий
/// </summary>
/// <returns></returns>
public Task GetCategoryListAsync();
}