namespace WEB_453504_ASP_NET.Tests;

using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WEB_453504_ASP_NET.UI.Services; 
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;
using WEB_453504_ASP_NET.UI.Services.CategoryService;
using WEB_453504_ASP_NET.UI.Services.InstrumentService;
using WEB_453504_ASP_NET.UI.Controllers;

/// <summary>
/// Тесты для контроллера Product (Задание 1)
/// Проверяют работу метода Index контроллера
/// </summary>
public class ProductControllerTests
{
    private readonly ICategoryService _categoryService;
    private readonly IInstrumentService _instrumentService;
    private readonly Product _controller;

    public ProductControllerTests()
    {
        // Настройка (Arrange) общих зависимостей для всех тестов
        _categoryService = Substitute.For<ICategoryService>();
        _instrumentService = Substitute.For<IInstrumentService>();

        _controller = new Product(_instrumentService, _categoryService)
        {
            // Настройка контекста контроллера для предотвращения NullReferenceException
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    /// <summary>
    /// Тест 1: Метод возвращает код 404 при неуспешном получении списка инструментов
    /// </summary>
    [Fact]
    public async Task Index_ReturnsNotFound_WhenInstrumentListFails()
    {
        // Arrange
        var categories = new List<Category> { new Category { Id = 1, Name = "Гитары", NormalizedName = "guitars" } };
        var categoryResponse = new ResponseData<List<Category>> { Successfull = true, Data = categories };
        
        var instrumentErrorResponse = new ResponseData<ListModel<MusicalInstrument>> 
        { 
            Successfull = false, 
            ErrorMessage = "Ошибка получения инструментов" 
        };

        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoryResponse));
        _instrumentService.GetInstrumentListAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(Task.FromResult(instrumentErrorResponse));

        // Act
        var result = await _controller.Index(null, 1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Ошибка получения инструментов", notFoundResult.Value);
    }

    /// <summary>
    /// Тест 2: Метод возвращает код 404 при неуспешном получении списка категорий
    /// </summary>
    [Fact]
    public async Task Index_ReturnsNotFound_WhenCategoryListFails()
    {
        // Arrange
        var instrumentResponse = new ResponseData<ListModel<MusicalInstrument>> 
        { 
            Successfull = true, 
            Data = new ListModel<MusicalInstrument> { Items = new List<MusicalInstrument>() } 
        };
        
        var categoryErrorResponse = new ResponseData<List<Category>> 
        { 
            Successfull = false, 
            ErrorMessage = "Ошибка получения категорий" 
        };

        _instrumentService.GetInstrumentListAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(Task.FromResult(instrumentResponse));
        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoryErrorResponse));

        // Act
        var result = await _controller.Index(null, 1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Null(notFoundResult.Value);
    }

    /// <summary>
    /// Тест 3: При успешном получении - модель содержит список категорий
    /// </summary>
    [Fact]
    public async Task Index_PassesCategoriesList_InModel_WhenSuccessful()
    {
        // Arrange
        var categories = new List<Category> 
        { 
            new Category { Id = 1, Name = "Гитары", NormalizedName = "guitars" },
            new Category { Id = 2, Name = "Пианино", NormalizedName = "pianos" }
        };
        var categoryResponse = new ResponseData<List<Category>> { Successfull = true, Data = categories };
        
        var instruments = new List<MusicalInstrument> 
        { 
            new MusicalInstrument { Id = 1, Name = "Акустическая гитара" } 
        };
        var instrumentResponse = new ResponseData<ListModel<MusicalInstrument>> 
        { 
            Successfull = true, 
            Data = new ListModel<MusicalInstrument> 
            { 
                Items = instruments,
                CurrentPage = 1,
                TotalPages = 1
            } 
        };

        _instrumentService.GetInstrumentListAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(Task.FromResult(instrumentResponse));
        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoryResponse));

        // Act
        var result = await _controller.Index(null, 1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CartViewModel>(viewResult.Model);
        Assert.NotNull(model.categories);
        Assert.Equal(2, model.categories.Count);
    }

    /// <summary>
    /// Тест 4: Во ViewData передано правильное значение текущей категории («Все», если категория не указана)
    /// </summary>
    [Fact]
    public async Task Index_SetsCurrentCategoryToAll_WhenCategoryIsNull()
    {
        // Arrange
        var categories = new List<Category> 
        { 
            new Category { Id = 1, Name = "Гитары", NormalizedName = "guitars" } 
        };
        var categoryResponse = new ResponseData<List<Category>> { Successfull = true, Data = categories };
        
        var instrumentResponse = new ResponseData<ListModel<MusicalInstrument>> 
        { 
            Successfull = true, 
            Data = new ListModel<MusicalInstrument> { Items = new List<MusicalInstrument>() } 
        };

        _instrumentService.GetInstrumentListAsync(null, 1).Returns(Task.FromResult(instrumentResponse));
        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoryResponse));

        // Act
        var result = await _controller.Index(null, 1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Все", viewResult.ViewData["currentCategory"]);
    }

    /// <summary>
    /// Тест 5: Во ViewData передано правильное значение текущей категории (переданное в метод имя категории)
    /// </summary>
    [Fact]
    public async Task Index_SetsCurrentCategoryToProvidedCategory_WhenCategoryIsSpecified()
    {
        // Arrange
        var categoryName = "Гитары";
        var normalizedCategory = "guitars";
        
        var categories = new List<Category> 
        { 
            new Category { Id = 1, Name = categoryName, NormalizedName = normalizedCategory } 
        };
        var categoryResponse = new ResponseData<List<Category>> { Successfull = true, Data = categories };
        
        var instruments = new List<MusicalInstrument> 
        { 
            new MusicalInstrument { Id = 1, Name = "Акустическая гитара" } 
        };
        var instrumentResponse = new ResponseData<ListModel<MusicalInstrument>> 
        { 
            Successfull = true, 
            Data = new ListModel<MusicalInstrument> 
            { 
                Items = instruments,
                CurrentPage = 1,
                TotalPages = 1
            } 
        };

        _instrumentService.GetInstrumentListAsync(normalizedCategory, 1).Returns(Task.FromResult(instrumentResponse));
        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoryResponse));

        // Act
        var result = await _controller.Index(normalizedCategory, 1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(categoryName, viewResult.ViewData["currentCategory"]);
    }

    /// <summary>
    /// Тест 6: В представление передана модель – CartViewModel со списком объектов
    /// </summary>
    [Fact]
    public async Task Index_PassesCartViewModelToView_WithInstrumentsAndCategories()
    {
        // Arrange
        var categories = new List<Category> 
        { 
            new Category { Id = 1, Name = "Гитары", NormalizedName = "guitars" } 
        };
        var categoryResponse = new ResponseData<List<Category>> { Successfull = true, Data = categories };
        
        var instruments = new List<MusicalInstrument> 
        { 
            new MusicalInstrument { Id = 1, Name = "Акустическая гитара" },
            new MusicalInstrument { Id = 2, Name = "Электрогитара" }
        };
        var instrumentResponse = new ResponseData<ListModel<MusicalInstrument>> 
        { 
            Successfull = true, 
            Data = new ListModel<MusicalInstrument> 
            { 
                Items = instruments,
                CurrentPage = 1,
                TotalPages = 1
            } 
        };

        _instrumentService.GetInstrumentListAsync(null, 1).Returns(Task.FromResult(instrumentResponse));
        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoryResponse));

        // Act
        var result = await _controller.Index(null, 1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CartViewModel>(viewResult.Model);
        Assert.NotNull(model.instruments);
        Assert.Equal(2, model.instruments.Count);
        Assert.NotNull(model.categories);
        Assert.Equal(1, model.categories.Count);
    }
}


