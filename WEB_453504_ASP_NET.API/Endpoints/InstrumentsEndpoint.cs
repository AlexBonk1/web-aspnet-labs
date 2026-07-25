using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using WEB_453504_ASP_NET.API.Data;
using WEB_453504_ASP_NET.Domain.Entities;
using WEB_453504_ASP_NET.Domain.Models;
using MediatR;
using WEB_453504_ASP_NET.API.UseCases;
namespace WEB_453504_ASP_NET.API.Endpoints;

public static class InstrumentsEndpoint
{
    public static void MapMusicalInstrumentEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/instruments").WithTags(nameof(MusicalInstrument));

        group.MapGet("/{category?}", async (IMediator mediator, string? category, int pageNo = 1) =>
        {

            var data = mediator.Send(new GetListOfInstruments(category, pageNo));
            return Results.Ok(data);

        })
        .WithName("GetAllMusicalInstruments");

        group.MapGet("/{id:required}", async Task<Results<Ok<MusicalInstrument>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.MusicalInstruments.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is MusicalInstrument model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetMusicalInstrumentById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, MusicalInstrument musicalInstrument, AppDbContext db) =>
        {
            var affected = await db.MusicalInstruments
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, musicalInstrument.Id)
                    .SetProperty(m => m.Name, musicalInstrument.Name)
                    .SetProperty(m => m.Description, musicalInstrument.Description)
                    .SetProperty(m => m.CategoryId, musicalInstrument.CategoryId)
                    .SetProperty(m => m.ImageUrl, musicalInstrument.ImageUrl)
                    .SetProperty(m => m.Price, musicalInstrument.Price)
                    .SetProperty(m => m.WeightKg, musicalInstrument.WeightKg)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateMusicalInstrument");

        group.MapPost("/", async (MusicalInstrument musicalInstrument, AppDbContext db) =>
        {
            db.MusicalInstruments.Add(musicalInstrument);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/MusicalInstrument/{musicalInstrument.Id}",musicalInstrument);
        })
        .WithName("CreateMusicalInstrument");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.MusicalInstruments
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteMusicalInstrument");
    }
}
