using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using WEB_453504_ASP_NET.API.Data;
using WEB_453504_ASP_NET.API.UseCases;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;
namespace WEB_453504_ASP_NET.API.Endpoints;

public static class InstrumentsEndpoint
{
    public record CreateInstrumentDto(string Name, string Description, int CategoryId, decimal Price, double WeightKg)
    {
        public IFormFile? File {get;set;}
    }

    public record UpdateInstrumentDto(string? Name, string? Description, int? CategoryId, decimal? Price, double? WeightKg)
    {
        public IFormFile? File {get;set;}
    }

    public static void MapMusicalInstrumentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/instruments")
            .WithTags(nameof(MusicalInstrument))
            .DisableAntiforgery();

        group.MapGet("/", async (IMediator mediator, [FromQuery] string? category, int pageNo = 1) =>
        {
            var data = await mediator.Send(new GetListOfInstruments(category, pageNo));
            return Results.Ok(data);
        })
        .WithName("GetAllMusicalInstruments");


        group.MapGet("/{id:int}", async Task<Results<Ok<MusicalInstrument>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.MusicalInstruments.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is MusicalInstrument model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetMusicalInstrumentById");

        group.MapPut("/{id}", async Task<Results<Ok<ResponseData<MusicalInstrument>>, NotFound, BadRequest<string>>> (
            int id, 
            [FromForm] UpdateInstrumentDto dto,
            IMediator mediator,
            IWebHostEnvironment wh,
            AppDbContext db) =>
        {
            var instrument = await db.MusicalInstruments.FirstOrDefaultAsync(x => x.Id == id);
            if (instrument == null)
                return TypedResults.NotFound();

            if (dto.CategoryId.HasValue && !await db.Categories.AnyAsync(x => x.Id == dto.CategoryId.Value))
                return TypedResults.BadRequest($"Категория с идентификатором {dto.CategoryId} не существует.");

            if (dto.Name != null && !string.IsNullOrWhiteSpace(dto.Name))       
                instrument.Name = dto.Name;
            if (dto.Description != null && !string.IsNullOrWhiteSpace(dto.Description))
                instrument.Description = dto.Description;
            if (dto.CategoryId.HasValue)
                instrument.CategoryId = dto.CategoryId.Value;
            if (dto.Price.HasValue)
                instrument.Price = dto.Price.Value;
            if (dto.WeightKg.HasValue)
                instrument.WeightKg = dto.WeightKg.Value;

            if (dto.File != null)
            {
                try
                {
                    var newImageName = await mediator.Send(new SaveImage(dto.File));

                    if (!string.IsNullOrEmpty(instrument.ImageUrl) && instrument.ImageUrl != "images/no-image.jpg")
                    {
                        var oldPath = Path.Combine(wh.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), instrument.ImageUrl);
                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                    }

                    instrument.ImageUrl = $"images/{newImageName}";
                }
                catch (Exception ex)
                {
                    return TypedResults.BadRequest($"Ошибка загрузки файла: {ex.Message}");
                }
            }

            db.MusicalInstruments.Update(instrument);
            await db.SaveChangesAsync();

            return TypedResults.Ok<ResponseData<MusicalInstrument>>(ResponseData<MusicalInstrument>.Success(instrument));
        })
        .WithName("UpdateMusicalInstrument");

        group.MapPost("/", async Task<Results<Created<ResponseData<MusicalInstrument>>, NotFound, BadRequest<string>>>  (
            [FromForm] CreateInstrumentDto dto,
            IMediator mediator, 
            IWebHostEnvironment wh, 
            AppDbContext db) => 
        {
            try
            {
                var instrument = new MusicalInstrument
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    CategoryId = dto.CategoryId,
                    Price = dto.Price,
                    WeightKg = dto.WeightKg,
                    ImageUrl = "images/no-image.jpg"
                };

                // Если прикреплён файл изображения
                if (dto.File != null)
                {
                    var imageName = await mediator.Send(new SaveImage(dto.File));
                    instrument.ImageUrl = $"images/{imageName}";
                }

                db.MusicalInstruments.Add(instrument);
                await db.SaveChangesAsync();

                var data = ResponseData<MusicalInstrument>.Success(instrument);
                return TypedResults.Created($"/api/instruments/{instrument.Id}", data);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"Ошибка создания инструмента: {ex.Message}");
            }
        })
        .WithName("CreateMusicalInstrument");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (
            [FromRoute] int id, 
            IWebHostEnvironment wh, 
            AppDbContext db) =>
        {
            var instrument = await db.MusicalInstruments.FirstOrDefaultAsync(i => i.Id == id);
            if (instrument == null)
                return TypedResults.NotFound();

            if (!string.IsNullOrEmpty(instrument.ImageUrl) && instrument.ImageUrl != "images/no-image.jpg")
            {
                var filePath = Path.Combine(wh.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), instrument.ImageUrl);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            db.MusicalInstruments.Remove(instrument);
            await db.SaveChangesAsync();

            return TypedResults.Ok();
        })
        .WithName("DeleteMusicalInstrument");
    }
}
