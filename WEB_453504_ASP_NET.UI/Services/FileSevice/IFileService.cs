namespace WEB_453504_ASP_NET.UI.Services.FileSevice;


public interface IFileService {
/// <summary>
/// Сохраняет данные в файле
/// </summary>
/// <param name="file">данные для сохранения</param>
/// <returns>адрес сохраненного файла</returns>
Task<string> SaveFileAsync(IFormFile file);

}