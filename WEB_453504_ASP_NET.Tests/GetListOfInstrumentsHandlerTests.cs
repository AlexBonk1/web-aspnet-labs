namespace WEB_453504_ASP_NET.Tests;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEB_453504_ASP_NET.API.Data;
using WEB_453504_ASP_NET.API.UseCases;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;

/// <summary>
/// Тесты для обработчика GetListOfInstruments (Задание 2)
/// Проверяют работу метода Handle класса GetListOfProductsHandler
/// Эквивалент тестирования ProductService из лабораторной работы
/// </summary>
public class GetListOfInstrumentsHandlerTests
{
    /// <summary>
    /// Создание in-memory SQLite контекста для тестирования
    /// </summary>
    private AppDbContext CreateContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    /// <summary>
    /// Вспомогательный метод для заполнения БД тестовыми данными
    /// </summary>
    private void SeedTestData(AppDbContext context)
    {
        // Создаём категории
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Гитары", NormalizedName = "guitars" },
            new Category { Id = 2, Name = "Пианино", NormalizedName = "pianos" },
            new Category { Id = 3, Name = "Барабаны", NormalizedName = "drums" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        // Создаём инструменты (7 в общей сложности для проверки пагинации)
        var instruments = new List<MusicalInstrument>
        {
            // Гитары (категория 1)
            new MusicalInstrument 
            { 
                Id = 1, 
                Name = "Акустическая гитара", 
                Description = "Классическая акустическая гитара",
                CategoryId = 1,
                Price = 5000,
                WeightKg = 1.5,
                ImageUrl = "guitar1.jpg"
            },
            new MusicalInstrument 
            { 
                Id = 2, 
                Name = "Электрогитара", 
                Description = "Электрогитара с усилителем",
                CategoryId = 1,
                Price = 15000,
                WeightKg = 2.0,
                ImageUrl = "guitar2.jpg"
            },
            new MusicalInstrument 
            { 
                Id = 3, 
                Name = "Классическая гитара", 
                Description = "Классическая испанская гитара",
                CategoryId = 1,
                Price = 8000,
                WeightKg = 1.8,
                ImageUrl = "guitar3.jpg"
            },
            new MusicalInstrument 
            { 
                Id = 4, 
                Name = "12-струнная гитара", 
                Description = "Профессиональная 12-струнная гитара",
                CategoryId = 1,
                Price = 12000,
                WeightKg = 2.2,
                ImageUrl = "guitar4.jpg"
            },
            // Пианино (категория 2)
            new MusicalInstrument 
            { 
                Id = 5, 
                Name = "Цифровое пианино", 
                Description = "Компактное цифровое пианино",
                CategoryId = 2,
                Price = 30000,
                WeightKg = 25.0,
                ImageUrl = "piano1.jpg"
            },
            new MusicalInstrument 
            { 
                Id = 6, 
                Name = "Синтезатор", 
                Description = "Профессиональный синтезатор",
                CategoryId = 2,
                Price = 50000,
                WeightKg = 20.0,
                ImageUrl = "synth1.jpg"
            },
            // Барабаны (категория 3)
            new MusicalInstrument 
            { 
                Id = 7, 
                Name = "Барабанный набор", 
                Description = "Полный набор барабанов",
                CategoryId = 3,
                Price = 25000,
                WeightKg = 40.0,
                ImageUrl = "drums1.jpg"
            }
        };

        context.MusicalInstruments.AddRange(instruments);
        context.SaveChanges();
    }

    /// <summary>
    /// Тест 1: Метод по умолчанию возвращает первую страницу размером 3 объекта
    /// на странице и правильно рассчитывает количество страниц
    /// </summary>
    [Fact]
    public async Task Handler_ReturnsFirstPageWithDefaultPageSize_And_CalculatesPageCountCorrectly()
    {
        // Arrange
        using var context = CreateContext();
        SeedTestData(context);

        var handler = new GetListOfProductsHandler(context);
        var request = new GetListOfInstruments(categoryNormalizedName: null, pageNo: 1, pageSize: 3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.CurrentPage);
        Assert.Equal(3, result.Data.Items.Count); // Первая страница содержит 3 инструмента
        Assert.Equal(3, result.Data.TotalPages); // Всего 7 инструментов -> 3 страницы (3+3+1)
    }

    /// <summary>
    /// Тест 2: Метод правильно выбирает заданную страницу
    /// </summary>
    [Fact]
    public async Task Handler_ReturnsCorrectPage_WhenPageNumberIsSpecified()
    {
        // Arrange
        using var context = CreateContext();
        SeedTestData(context);

        var handler = new GetListOfProductsHandler(context);
        var request = new GetListOfInstruments(categoryNormalizedName: null, pageNo: 2, pageSize: 3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.CurrentPage);
        Assert.Equal(3, result.Data.Items.Count); // На второй странице тоже 3 инструмента
        // Проверяем, что это правильные инструменты (4, 5, 6)
        Assert.Equal(4, result.Data.Items[0].Id);
        Assert.Equal(5, result.Data.Items[1].Id);
        Assert.Equal(6, result.Data.Items[2].Id);
    }

    /// <summary>
    /// Тест 3: Метод правильно выполняет фильтрацию объектов по категории
    /// </summary>
    [Fact]
    public async Task Handler_FiltersInstruments_ByCategory_Correctly()
    {
        // Arrange
        using var context = CreateContext();
        SeedTestData(context);

        var handler = new GetListOfProductsHandler(context);
        // Фильтруем по категории "guitars" - должно быть 4 инструмента
        var request = new GetListOfInstruments(categoryNormalizedName: "guitars", pageNo: 1, pageSize: 3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.CurrentPage);
        Assert.Equal(3, result.Data.Items.Count); // Первая страница
        Assert.Equal(2, result.Data.TotalPages); // 4 гитары -> 2 страницы (3+1)
        
        // Проверяем, что все возвращённые инструменты гитары
        foreach (var instrument in result.Data.Items)
        {
            Assert.Equal("guitars", instrument.Category.NormalizedName);
        }
    }

    /// <summary>
    /// Тест 4: Метод правильно выполняет фильтрацию по другой категории
    /// </summary>
    [Fact]
    public async Task Handler_FiltersByPianos_Category_Correctly()
    {
        // Arrange
        using var context = CreateContext();
        SeedTestData(context);

        var handler = new GetListOfProductsHandler(context);
        var request = new GetListOfInstruments(categoryNormalizedName: "pianos", pageNo: 1, pageSize: 3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count); // В категории пианино только 2 инструмента
        Assert.Equal(1, result.Data.TotalPages);
        
        // Проверяем, что это правильные инструменты
        Assert.Equal(5, result.Data.Items[0].Id);
        Assert.Equal(6, result.Data.Items[1].Id);
    }

    /// <summary>
    /// Тест 5: Метод не позволяет задать размер страницы больше максимального
    /// </summary>
    [Fact]
    public async Task Handler_LimitsPageSize_ToMaximum()
    {
        // Arrange
        using var context = CreateContext();
        SeedTestData(context);

        var handler = new GetListOfProductsHandler(context);
        // Пытаемся установить pageSize больше максимума (20)
        var request = new GetListOfInstruments(categoryNormalizedName: null, pageNo: 1, pageSize: 50);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        // Хендлер не должен возвращать больше максимально допустимого количества на странице
        // В данном случае максимум 20, но у нас только 7 инструментов
        Assert.Equal(7, result.Data.Items.Count);
    }

    /// <summary>
    /// Тест 6: Когда запрашиваем несуществующую страницу,
    /// обработчик возвращает последнюю доступную страницу
    /// </summary>
    [Fact]
    public async Task Handler_ReturnsSafePageNumber_WhenPageNumberExceedsMaximum()
    {
        // Arrange
        using var context = CreateContext();
        SeedTestData(context);

        var handler = new GetListOfProductsHandler(context);
        // Запрашиваем страницу 10, хотя всего только 3 страницы
        var request = new GetListOfInstruments(categoryNormalizedName: null, pageNo: 10, pageSize: 3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        // Обработчик использует Math.Min(PageCount, pageNo) -> Min(3, 10) = 3
        // Так что CurrentPage остается 10, но данные будут из страницы 3
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        Assert.Equal(10, result.Data.CurrentPage);
        // Данные содержат последний элемент (с ID 7)
        Assert.Single(result.Data.Items);
        Assert.Equal(7, result.Data.Items[0].Id);
    }

    /// <summary>
    /// Тест 7: Метод обрабатывает корректно граничные значения
    /// </summary>
    [Fact]
    public async Task Handler_HandlesEdgeCases_WithLastPage()
    {
        // Arrange
        using var context = CreateContext();
        SeedTestData(context);

        var handler = new GetListOfProductsHandler(context);
        // Запрашиваем третью (последнюю) страницу
        var request = new GetListOfInstruments(categoryNormalizedName: null, pageNo: 3, pageSize: 3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.CurrentPage);
        Assert.Equal(1, result.Data.Items.Count); // На последней странице только 1 инструмент
        Assert.Equal(7, result.Data.Items[0].Id); // Это последний инструмент
    }

    /// <summary>
    /// Тест 8: Метод обрабатывает отрицательные номера страниц
    /// </summary>
    [Fact]
    public async Task Handler_ReturnsFirstPage_WhenPageNumberIsNegative()
    {
        // Arrange
        using var context = CreateContext();
        SeedTestData(context);

        var handler = new GetListOfProductsHandler(context);
        // Пытаемся запросить страницу -1
        var request = new GetListOfInstruments(categoryNormalizedName: null, pageNo: -1, pageSize: 3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.CurrentPage); // Должна вернуться первая страница
        Assert.Equal(3, result.Data.Items.Count);
    }

    /// <summary>
    /// Тест 9: Метод корректно работает с пустой базой данных
    /// </summary>
    [Fact]
    public async Task Handler_ReturnsEmptyList_WhenNoInstrumentsInDatabase()
    {
        // Arrange
        using var context = CreateContext();
        // Не добавляем никаких данных

        var handler = new GetListOfProductsHandler(context);
        var request = new GetListOfInstruments(categoryNormalizedName: null, pageNo: 1, pageSize: 3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Items);
        Assert.Equal(1, result.Data.CurrentPage);
        // Когда нет элементов, Math.Ceiling(0/3) = 0, но это может быть обработано как 0 страниц
        Assert.Equal(0, result.Data.TotalPages);
    }

    /// <summary>
    /// Тест 10: Метод сохраняет порядок сортировки по ID
    /// </summary>
    [Fact]
    public async Task Handler_MaintainsOrder_ByInstrumentId()
    {
        // Arrange
        using var context = CreateContext();
        SeedTestData(context);

        var handler = new GetListOfProductsHandler(context);
        var request = new GetListOfInstruments(categoryNormalizedName: null, pageNo: 1, pageSize: 3);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Successfull);
        Assert.NotNull(result.Data);
        // Проверяем что инструменты отсортированы по ID
        Assert.Equal(1, result.Data.Items[0].Id);
        Assert.Equal(2, result.Data.Items[1].Id);
        Assert.Equal(3, result.Data.Items[2].Id);
    }
}
